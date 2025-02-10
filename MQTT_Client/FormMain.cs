using IniParser;
using IniParser.Model;
using MQTT_Client.FormElements;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//элементы контейнера съебываются в ебеня
namespace MQTT_Client
{
	public partial class FormMain : Form
	{
		private static readonly string moduleName = "FormMain";
		private static readonly Logger baseLogger = LogManager.GetLogger(moduleName);
		private static readonly LoggerManager logger = new LoggerManager(baseLogger, moduleName);

		private string ADDRESS = "127.0.0.1";
		private int PORT = 1883;
		private string LOGIN = "user_login";
		private string PASSWORD = "user_password";
		private string URL_FIREBASE = string.Empty;
		private string SECRET_FIREBASE = string.Empty;
		private const string filePathConfig = "config.ini";
		private const string filePathRules = "rules.json";

		string configTextDefault = string.Empty;
		public void initConfig()
		{
	
			FileIniDataParser parser = new FileIniDataParser();

			if (File.Exists(filePathConfig))
			{
				IniData data = parser.ReadFile(filePathConfig);

				string[] linesConfig = File.ReadAllLines(filePathConfig);
				ADDRESS = data["Settings"]["ADDRESS"];
				PORT = Convert.ToInt32(data["Settings"]["PORT"]);
				LOGIN = data["Settings"]["LOGIN"];
				PASSWORD = data["Settings"]["PASSWORD"];
				URL_FIREBASE = data["Settings"]["URL_FIREBASE"];
				SECRET_FIREBASE = data["Settings"]["SECRET_FIREBASE"];
			}
			else
			{
				IniData data = new IniData();
				data.Sections.AddSection("Settings");
				data["Settings"]["ADDRESS"] = ADDRESS;
				data["Settings"]["PORT"] = PORT.ToString();
				data["Settings"]["LOGIN"] = LOGIN;
				data["Settings"]["PASSWORD"] = PASSWORD;
				data["Settings"]["URL_FIREBASE"] = URL_FIREBASE;
				data["Settings"]["SECRET_FIREBASE"] = SECRET_FIREBASE;
				parser.WriteFile(filePathConfig, data);
			}

			configTextDefault = $"ADDRESS={ADDRESS}\r\n" +
								$"PORT={PORT.ToString()}\r\n" +
								$"LOGIN={LOGIN}\r\n" +
								$"PASSWORD={PASSWORD}\n\r" +
								$"URL_FIREBASE={URL_FIREBASE}\n\r" +
								$"SECRET_FIREBASE={SECRET_FIREBASE}";

			if (File.Exists(filePathRules))
			{
				string json = File.ReadAllText(filePathRules);
				List<RuleUnit> jsonRules = JsonConvert.DeserializeObject<List<RuleUnit>>(json);
				foreach (RuleUnit rule in jsonRules)
				{
					AddRule(rule.FirebaseReference, rule.MQTT_topic, rule.Direction);
				}

				if (ruleControls.Count != 0)
				{
					buttonStartStop.Enabled = true;
				}
			}
		}

		public FormMain()
		{
			InitializeComponent();

			this.MaximumSize = new Size(886, 932);
			this.MinimumSize = new Size(886, 932);
			buttonAddTopic.Focus();
			buttonStartStop.Enabled = false;
		}

		private string textBuffer = string.Empty;
		private void AddDataToText(string text)
		{
			lock (textBuffer) { textBuffer = textBuffer + text + Environment.NewLine; }

			if (textBoxInfo.InvokeRequired)
			{
				lock (textBoxInfo)
				{
					textBoxInfo.Invoke(new Action(() =>
					{
						textBoxInfo.Text = textBuffer;
						textBoxInfo.SelectionStart = textBoxInfo.Text.Length;
						textBoxInfo.ScrollToCaret();
					}));
				}
			}
			else
			{
				lock (textBoxInfo)
				{
					textBoxInfo.Text = textBuffer;
					textBoxInfo.SelectionStart = textBoxInfo.Text.Length;
					textBoxInfo.ScrollToCaret();
				}
			}
		}

		private MqttBrokerClient mqttBrokerClient = null;
		private FirebaseService firebaseService = null;
		private Dictionary<string, string> subscriptions = new Dictionary<string, string>();
		private void FormMain_Load(object sender, EventArgs e)
		{
			textBuffer = textBuffer + Properties.Resources.connecting_string + Environment.NewLine;
			textBoxInfo.Text = textBuffer;
			labelTopic.Text = Properties.Resources.topic_string;

			try
			{
				initConfig();

				//MQTT
				mqttBrokerClient = new MqttBrokerClient(ADDRESS, PORT, LOGIN, PASSWORD);
				mqttBrokerClient.Connect();

				Task.Run(() =>
				{
					mqttBrokerClient.MessageReceived += async (senderMQTT, eMQTT) =>
					{
						string postString = Properties.Resources.notification_string + $"Topic: [{eMQTT.Topic}] Message: [{eMQTT.Payload}]";
						AddDataToText(postString);

						RuleControl rule;
						lock (ruleControls)
						{
							List<RuleControl> rules = ruleControls.Where(r => r.Direction == true).ToList();
							rule = rules.FirstOrDefault(r => r.MQTT_topic == eMQTT.Topic);
						}

						if (rule != null)
							await firebaseService.UpdateDataAsync<string>(rule.FirebaseReference, eMQTT.Payload);
					};
				});

				//Firebase
				if (!string.IsNullOrEmpty(URL_FIREBASE) && !string.IsNullOrEmpty(SECRET_FIREBASE))
				{
					firebaseService = new FirebaseService(URL_FIREBASE, SECRET_FIREBASE);
					Task.Run(async () =>
					{
						while (true && !disconnect)
						{
							if (switcher)
							{
								List<RuleControl> rules;
								lock (ruleControls)
								{
									rules = ruleControls.Where(r => r.Direction == false).ToList(); // Создаём копию списка
								}
								foreach (RuleControl rule in rules)
								{
									string firebaseData = await firebaseService.GetDataAsync<string>(rule.FirebaseReference);
									if (!subscriptions.TryGetValue(rule.FirebaseReference, out string oldValue) || oldValue != firebaseData)
									{
										subscriptions[rule.FirebaseReference] = firebaseData;
										mqttBrokerClient.Publish(rule.MQTT_topic, firebaseData);
									}
								}
								await Task.Delay(3000);
							}
						}
					});
				}

				Task.Run(() => CheckConnection());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Properties.Resources.error_string);
				Environment.Exit(0);
			}
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			string message = textBoxMessage.Text;
			textBoxMessage.Text = string.Empty;
			if (mqttBrokerClient.IsConnected())
			{
				if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(currentTopic))
				{
					try
					{
						mqttBrokerClient.Publish(currentTopic, message);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, Properties.Resources.error_string);
						Environment.Exit(0);
					}
				}
				else
				{
					MessageBox.Show(Properties.Resources.empty_message_string, Properties.Resources.error_string);
				}
			}
			else
			{
				MessageBox.Show(Properties.Resources.not_connected_string, Properties.Resources.error_string);
			}
		}

		private void textBoxMessage_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				buttonSend_Click(sender, e);
			}
		}

		private void buttonAddTopic_Click(object sender, EventArgs e)
		{
			InputBox inputBox = new InputBox(Properties.Resources.topic_adding_string, Properties.Resources.enter_topic_name_string);
			if (inputBox.ShowDialog().Equals(DialogResult.OK))
			{
				string topic = inputBox.InputText;
				if (!string.IsNullOrEmpty(topic))
				{
					TreeViewFiller(ref treeViewMain, topic);
				}
			}
		}

		private void TreeViewFiller(ref TreeView treeView, string topic)
		{
			string[] topicParts = topic.Split('/');
			TreeNode currentNode = null;

			foreach (string part in topicParts)
			{
				if (!string.IsNullOrEmpty(part) && !part.Equals(" "))
				{
					if (currentNode == null) // Работаем с корневым узлом
					{
						// Проверяем, существует ли корневой узел
						TreeNode foundNode = treeView.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text == part);
						if (foundNode != null)
						{
							currentNode = foundNode; // Узел найден
						}
						else
						{
							currentNode = treeView.Nodes.Add(part); // Добавляем новый корневой узел
						}
					}
					else // Работаем с дочерними узлами
					{
						// Проверяем, существует ли дочерний узел
						TreeNode foundNode = currentNode.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text == part);
						if (foundNode != null)
						{
							currentNode = foundNode; // Узел найден
						}
						else
						{
							currentNode = currentNode.Nodes.Add(part); // Добавляем новый дочерний узел
						}
					}
				}
			}
		}

		private string currentTopic = string.Empty;
		private void treeViewMain_AfterSelect(object sender, TreeViewEventArgs e)
		{
			pictureBoxStatus.BackgroundImage = Properties.Resources.red;
			labelStatus.Text = Properties.Resources.not_connected_string;

			try
			{
				if (!string.IsNullOrEmpty(currentTopic))
				{
					mqttBrokerClient.Unsubscribe(currentTopic);
				}

				TreeNode selectedNode = e.Node;
				currentTopic = GetFullPath(selectedNode);

				mqttBrokerClient.Subscribe(currentTopic);
				labelTopic.Text = Properties.Resources.topic_string + currentTopic;
				AddDataToText(Properties.Resources.subscribe_topic_string + currentTopic);

				Thread.Sleep(1000);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Properties.Resources.error_string);
				Environment.Exit(0);
			}
		}

		private string GetFullPath(TreeNode node)
		{
			string path = node.Text; // Начинаем с текущего узла
			while (node.Parent != null) // Поднимаемся вверх по дереву
			{
				node = node.Parent;
				path = node.Text + "/" + path; // Добавляем родительский узел
			}
			return path;
		}

		bool disconnect = false;
		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				AddDataToText(Properties.Resources.disconnecting_string);

				List<RuleUnit> ruleUnits = new List<RuleUnit>();
				foreach (RuleControl rule in ruleControls)
				{
					ruleUnits.Add(new RuleUnit(rule.FirebaseReference, rule.MQTT_topic, rule.Direction));
				}
				// Сериализуем список в JSON
				string json = JsonConvert.SerializeObject(ruleUnits, Formatting.Indented);
				// Записываем JSON в файл
				File.WriteAllText(filePathRules, json);

				mqttBrokerClient.Disconnect();
				disconnect = true;
				Thread.Sleep(3000);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Properties.Resources.error_string);
			}
			finally
			{
				Environment.Exit(0);
			}
		}

		bool status = true;
		private void CheckConnection()
		{
			try
			{
				while (!disconnect)
				{
					if (mqttBrokerClient.IsConnected())
					{

						pictureBoxStatus.Invoke(new Action(() => pictureBoxStatus.BackgroundImage = Properties.Resources.green));
						labelStatus.Invoke(new Action(() => labelStatus.Text = Properties.Resources.connected_string));
						if (status)
						{
							AddDataToText(Properties.Resources.connected_string);

							status = false;
						}
					}
					else
					{
						pictureBoxStatus.Invoke(new Action(() => pictureBoxStatus.BackgroundImage = Properties.Resources.red));
						labelStatus.Invoke(new Action(() => labelStatus.Text = Properties.Resources.not_connected_string));

						AddDataToText(Properties.Resources.not_connected_string);
						AddDataToText(Properties.Resources.reconnect_string);
						//mqttBrokerClient.Connect();

						status = true;
					}

					Thread.Sleep(3000);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Properties.Resources.error_string);
				Environment.Exit(0);
			}
			finally
			{
				mqttBrokerClient.Disconnect();
			}
		}

		//----------------------------------------------------- RULES -----------------------------------------------------

		private bool switcher = false;
		private void buttonStartStop_Click(object sender, EventArgs e)
		{
			if (ruleControls.Count != 0)
			{
				//если оставили процесс - отписаться от топиков и не чекать новые данные в fb
				if (switcher)
				{
					switcher = !switcher;
					buttonStartStop.Text = "Start";
					buttonAddRule.Enabled = true;
					flowLayoutPanelRules.Enabled = true;

					foreach (RuleControl rule in ruleControls)
					{
						mqttBrokerClient.Unsubscribe(rule.MQTT_topic);
						//firebaseService.Unsubscribe(rule.FirebaseReference);
						AddDataToText($"Unsubscribe on topic: {rule.MQTT_topic}. Unsubscribe on path: {rule.FirebaseReference}.");
					}
				}
				else
				{
					//если запустили процесс - подписаться на топики и fb 
					switcher = !switcher;
					buttonStartStop.Text = "Stop";
					buttonAddRule.Enabled = false;
					flowLayoutPanelRules.Enabled = false;

					foreach (RuleControl rule in ruleControls)
					{
						mqttBrokerClient.Subscribe(rule.MQTT_topic);
						//firebaseService.Subscribe(rule.FirebaseReference);
						AddDataToText($"Subscribe on topic: {rule.MQTT_topic}. Subscribe on path: {rule.FirebaseReference}.");
					}
				}
			}
		}

		private void buttonAddRule_Click(object sender, EventArgs e)
		{
			AddRule("", "", false);
			if (ruleControls.Count != 0)
			{
				buttonStartStop.Enabled = true;
			}
		}

		private List<RuleControl> ruleControls = new List<RuleControl>();
		private void AddRule(string FirebaseReference, string MQTT_topic, bool Direction)
		{
			// Создаем новый элемент управления
			RuleControl ruleControl = new RuleControl(FirebaseReference, MQTT_topic, Direction);
			// Добавляем его в контейнер
			flowLayoutPanelRules.Controls.Add(ruleControl);
			// Добавляем его в список для управления
			ruleControls.Add(ruleControl);
			// Подписываемся на событие удаления
			ruleControl.ButtonDeleteClicked += RuleControl_ButtonDeleteClicked;
		}

		private void RuleControl_ButtonDeleteClicked(object sender, EventArgs e)
		{
			// Удаляем элемент из контейнера и списка
			RuleControl ruleControl = sender as RuleControl;
			flowLayoutPanelRules.Controls.Remove(ruleControl);
			ruleControls.Remove(ruleControl);

			if (ruleControls.Count == 0)
			{
				buttonStartStop.Enabled = false;
			}
		}
	}
}
