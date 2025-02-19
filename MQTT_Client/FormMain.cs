using Google.Protobuf.WellKnownTypes;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
		private string ID_FIRESTORE = string.Empty;
		private string PATH_FIRESTORE = string.Empty;
		private const string filePathConfig = "config.ini";
		private const string filePathFirebaseRules = "rulesFirebase.json";
		private const string filePathFirestoreRules = "rulesFirestore.json";

		private string configTextDefault = string.Empty;
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
				ID_FIRESTORE = data["Settings"]["ID_FIRESTORE"];
				PATH_FIRESTORE = data["Settings"]["PATH_FIRESTORE"];
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
				data["Settings"]["ID_FIRESTORE"] = ID_FIRESTORE;
				data["Settings"]["PATH_FIRESTORE"] = PATH_FIRESTORE;
				parser.WriteFile(filePathConfig, data);
			}

			configTextDefault = $"ADDRESS = [{ADDRESS}]\r\n" +
								$"PORT = [{PORT.ToString()}]\r\n" +
								$"LOGIN = [{LOGIN}]\r\n" +
								$"PASSWORD = [{PASSWORD}]\n\r" +
								$"URL_FIREBASE = [{URL_FIREBASE}]\n\r" +
								$"SECRET_FIREBASE = [{SECRET_FIREBASE}]\n\r" +
								$"ID_FIRESTORE = [{ID_FIRESTORE}]\n\r" +
								$"PATH_FIRESTORE = [{PATH_FIRESTORE}]";

			loadRules(filePathFirebaseRules, AddRuleFirebase);
			if (ruleControlsFirebase.Count != 0) buttonStartStopFirebase.Enabled = true;
			loadRules(filePathFirestoreRules, AddRuleFirestore);
			if (ruleControlsFirestore.Count != 0) buttonStartStopFirestore.Enabled = true;
		}

		private void loadRules(string path, Action<string, string, bool> AddRule)
		{
			if (File.Exists(path))
			{
				string json = File.ReadAllText(path);
				List<RuleUnit> jsonRules = JsonConvert.DeserializeObject<List<RuleUnit>>(json);
				if (jsonRules != null && jsonRules.Count != 0)
				{
					foreach (RuleUnit rule in jsonRules)
					{
						AddRule(rule.FirebaseReference, rule.MQTT_topic, rule.Direction);
					}
				}
			}
			else
			{
				File.Create(path).Close();
			}
		}

		public FormMain()
		{
			InitializeComponent();

			this.MaximumSize = new Size(886, 932);
			this.MinimumSize = new Size(886, 932);
			buttonAddTopic.Focus();
			buttonStartStopFirebase.Enabled = false;
			buttonStartStopFirestore.Enabled = false;
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
		private MqttBrokerClient mqttReciverClientFirebase = null;
		private Dictionary<string, string> subscriptionsFirebase = new Dictionary<string, string>();

		private FirestoreService firestoreService = null;
		private MqttBrokerClient mqttReciverClientFirestore = null;
		//private Dictionary<string, string> subscriptionsFirestore = new Dictionary<string, string>();
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
				mqttBrokerClient.MessageReceived += (senderMQTT, eMQTT) =>
				{
					string postString = Properties.Resources.notification_string + $"Topic: [{eMQTT.Topic}] Message: [{eMQTT.Payload}]";
					AddDataToText(postString);
				};

				
				if (!string.IsNullOrEmpty(URL_FIREBASE) && !string.IsNullOrEmpty(SECRET_FIREBASE))
				{
					//MQTTReciverFirebase
					mqttReciverClientFirebase = new MqttBrokerClient(ADDRESS, PORT, LOGIN, PASSWORD);
					mqttReciverClientFirebase.Connect();
					AddDataToText(Properties.Resources.ready_mqtt_firebase);
					mqttReciverClientFirebase.MessageReceived += async (senderMQTT, eMQTT) =>
					{
						string postString = Properties.Resources.notification_string + $"Topic: [{eMQTT.Topic}] Message: [{eMQTT.Payload}]";
						AddDataToText(postString);

						RuleControl rule;
						lock (ruleControlsFirebase)
						{
							List<RuleControl> rules = ruleControlsFirebase.Where(r => r.Direction == true).ToList();
							rule = rules.FirstOrDefault(r => r.MQTT_topic == eMQTT.Topic);
						}

						if (rule != null)
							await firebaseService.UpdateDataAsync<string>(rule.FirebaseReference, eMQTT.Payload);
					};

					//Firebase
					firebaseService = new FirebaseService(URL_FIREBASE, SECRET_FIREBASE);
					Task.Run(async () =>
					{
						while (true && !disconnect)
						{
							if (switcherFirebase)
							{
								List<RuleControl> rules;
								lock (ruleControlsFirebase)
								{
									rules = ruleControlsFirebase.Where(r => r.Direction == false).ToList(); // Создаём копию списка
								}
								foreach (RuleControl rule in rules)
								{
									string firebaseData = await firebaseService.GetDataAsync<string>(rule.FirebaseReference);
									if (!subscriptionsFirebase.TryGetValue(rule.FirebaseReference, out string oldValue) || oldValue != firebaseData)
									{
										string postString = Properties.Resources.notification_string + $"Firebase path: [{rule.FirebaseReference}] Message: [{firebaseData}]";
										AddDataToText(postString);

										subscriptionsFirebase[rule.FirebaseReference] = firebaseData;
										mqttReciverClientFirebase.Publish(rule.MQTT_topic, firebaseData);
									}
								}

								await Task.Delay(3000);
							}
						}
					});
				}
				else
				{
					buttonAddRuleFirebase.Enabled = false;
				}

				if (!string.IsNullOrEmpty(ID_FIRESTORE) && !string.IsNullOrEmpty(PATH_FIRESTORE))
				{
					//MQTTReciverFirestore
					mqttReciverClientFirestore = new MqttBrokerClient(ADDRESS, PORT, LOGIN, PASSWORD);
					mqttReciverClientFirestore.Connect();
					AddDataToText(Properties.Resources.ready_mqtt_firestore);
					mqttReciverClientFirestore.MessageReceived += async (senderMQTT, eMQTT) =>
					{
						string postString = Properties.Resources.notification_string + $"Topic: [{eMQTT.Topic}] Message: [{eMQTT.Payload}]";
						AddDataToText(postString);

						RuleControl rule;
						lock (ruleControlsFirebase)
						{
							List<RuleControl> rules = ruleControlsFirestore.Where(r => r.Direction == true).ToList();
							rule = rules.FirstOrDefault(r => r.MQTT_topic == eMQTT.Topic);
						}

						if (rule != null)
						{
							FirestorePath path = new FirestorePath(rule.FirebaseReference);
							var updates = new Dictionary<string, object>
							{
								{ path.Field, eMQTT.Payload }
							};
							await firestoreService.UpdateDataAsync(path, updates);
						}
					};

					//Firestore
					firestoreService = new FirestoreService(ID_FIRESTORE, PATH_FIRESTORE);
					firestoreService.OnMessage += (senderFirestore, eFirestore) =>
					{
						string postString = Properties.Resources.notification_string + $"Firestore path: [{eFirestore.Path.SourcePath}] Message: [{eFirestore.Data.ToString()}]";
						AddDataToText(postString);

						List<RuleControl> rules;
						lock (ruleControlsFirestore)
						{
							rules = ruleControlsFirestore.Where(r => r.Direction == false).ToList(); // Создаём копию списка
						}
						RuleControl rule = rules.FirstOrDefault(r => r.FirebaseReference == eFirestore.Path.SourcePath);

						if (rule != null)
							mqttReciverClientFirestore.Publish(rule.MQTT_topic, eFirestore.Data.ToString());
					};
				}
				else
				{
					buttonAddRuleFirestore.Enabled = false;
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

				List<RuleUnit> ruleUnitsFirebase = new List<RuleUnit>();
				foreach (RuleControl rule in ruleControlsFirebase)
				{
					ruleUnitsFirebase.Add(new RuleUnit(rule.FirebaseReference, rule.MQTT_topic, rule.Direction));
				}
				// Сериализуем список в JSON
				string jsonFirebase = JsonConvert.SerializeObject(ruleUnitsFirebase, Formatting.Indented);
				// Записываем JSON в файл
				File.WriteAllText(filePathFirebaseRules, jsonFirebase);

				List<RuleUnit> ruleUnitsFirestore = new List<RuleUnit>();
				foreach (RuleControl rule in ruleControlsFirestore)
				{
					ruleUnitsFirestore.Add(new RuleUnit(rule.FirebaseReference, rule.MQTT_topic, rule.Direction));
				}
				// Сериализуем список в JSON
				string jsonFirestore = JsonConvert.SerializeObject(ruleUnitsFirestore, Formatting.Indented);
				// Записываем JSON в файл
				File.WriteAllText(filePathFirestoreRules, jsonFirestore);

				disconnect = true;
				mqttBrokerClient.Disconnect();
				if (mqttReciverClientFirebase != null && mqttReciverClientFirebase.IsConnected())
					mqttReciverClientFirebase.Disconnect();
				if (mqttReciverClientFirestore != null && mqttReciverClientFirestore.IsConnected())
					mqttReciverClientFirestore.Disconnect();
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

		//----------------------------------------------------- FIREBASE RULES -----------------------------------------------------

		private List<RuleControl> ruleControlsFirebase = new List<RuleControl>();
		private void AddRuleFirebase(string FirebaseReference, string MQTT_topic, bool Direction)
		{
			// Создаем новый элемент управления
			RuleControl ruleControl = new RuleControl(FirebaseReference, MQTT_topic, Direction);
			// Добавляем его в контейнер
			flowLayoutPanelRulesFirebase.Controls.Add(ruleControl);
			// Добавляем его в список для управления
			ruleControlsFirebase.Add(ruleControl);
			// Подписываемся на событие удаления
			ruleControl.ButtonDeleteClicked += RuleControl_ButtonDeleteClickedFirebase;
		}

		private void RuleControl_ButtonDeleteClickedFirebase(object sender, EventArgs e)
		{
			// Удаляем элемент из контейнера и списка
			RuleControl ruleControl = sender as RuleControl;
			flowLayoutPanelRulesFirebase.Controls.Remove(ruleControl);
			ruleControlsFirebase.Remove(ruleControl);

			if (ruleControlsFirebase.Count == 0)
			{
				buttonStartStopFirebase.Enabled = false;
			}
		}

		private bool switcherFirebase = false;
		private void buttonStartStopFirebase_Click(object sender, EventArgs e)
		{
			if (ruleControlsFirebase.Count != 0)
			{
				//если оставили процесс - отписаться от топиков и не чекать новые данные в fb
				if (switcherFirebase)
				{
					switcherFirebase = !switcherFirebase;
					buttonStartStopFirebase.Text = "Start";
					buttonAddRuleFirebase.Enabled = true;
					flowLayoutPanelRulesFirebase.Enabled = true;

					foreach (RuleControl rule in ruleControlsFirebase)
					{
						mqttReciverClientFirebase.Unsubscribe(rule.MQTT_topic);
						string text = $"Deactivate rule: [{rule.FirebaseReference}] {(rule.Direction ? "<" : ">")} [{rule.MQTT_topic}].";
						AddDataToText(text);
					}
				}
				else
				{
					//если запустили процесс - подписаться на топики и fb 
					switcherFirebase = !switcherFirebase;
					buttonStartStopFirebase.Text = "Stop";
					buttonAddRuleFirebase.Enabled = false;
					flowLayoutPanelRulesFirebase.Enabled = false;

					foreach (RuleControl rule in ruleControlsFirebase)
					{
						if (rule.Direction)
							mqttReciverClientFirebase.Subscribe(rule.MQTT_topic);

						string text = $"Activate rule: [{rule.FirebaseReference}] {(rule.Direction ? "<" : ">")} [{rule.MQTT_topic}].";
						AddDataToText(text);
					}
				}
			}
		}

		private void buttonAddRuleFirebase_Click(object sender, EventArgs e)
		{
			AddRuleFirebase(string.Empty, string.Empty, false);
			if (ruleControlsFirebase.Count != 0)
			{
				buttonStartStopFirebase.Enabled = true;
			}
		}

		//----------------------------------------------------- FIRESTORE RULES -----------------------------------------------------

		private List<RuleControl> ruleControlsFirestore = new List<RuleControl>();
		private void AddRuleFirestore(string FirestoreReference, string MQTT_topic, bool Direction)
		{
			// Создаем новый элемент управления
			RuleControl ruleControl = new RuleControl(FirestoreReference, MQTT_topic, Direction);
			ruleControl.BackColor = Color.SkyBlue;
			// Добавляем его в контейнер
			flowLayoutPanelRulesFirestore.Controls.Add(ruleControl);
			// Добавляем его в список для управления
			ruleControlsFirestore.Add(ruleControl);
			// Подписываемся на событие удаления
			ruleControl.ButtonDeleteClicked += RuleControl_ButtonDeleteClickedFirestore;
		}

		private void RuleControl_ButtonDeleteClickedFirestore(object sender, EventArgs e)
		{
			// Удаляем элемент из контейнера и списка
			RuleControl ruleControl = sender as RuleControl;
			flowLayoutPanelRulesFirestore.Controls.Remove(ruleControl);
			ruleControlsFirestore.Remove(ruleControl);

			if (ruleControlsFirestore.Count == 0)
			{
				buttonStartStopFirestore.Enabled = false;
			}
		}

		private bool switcherFirestore = false;
		private void buttonStartStopFirestore_Click(object sender, EventArgs e)
		{
			if (ruleControlsFirestore.Count != 0)
			{
				//если оставили процесс - отписаться от топиков и не чекать новые данные в fs
				if (switcherFirestore)
				{
					switcherFirestore = !switcherFirestore;
					buttonStartStopFirestore.Text = "Start";
					buttonAddRuleFirestore.Enabled = true;
					flowLayoutPanelRulesFirestore.Enabled = true;

					firestoreService.UnsubscribeAll();
					foreach (RuleControl rule in ruleControlsFirestore)
					{
						mqttReciverClientFirestore.Unsubscribe(rule.MQTT_topic);
						string text = $"Deactivate rule: [{rule.FirebaseReference}] {(rule.Direction ? "<" : ">")} [{rule.MQTT_topic}].";
						AddDataToText(text);
					}
				}
				else
				{
					//если запустили процесс - подписаться на топики и fs
					switcherFirestore = !switcherFirestore;
					buttonStartStopFirestore.Text = "Stop";
					buttonAddRuleFirestore.Enabled = false;
					flowLayoutPanelRulesFirestore.Enabled = false;

					foreach (RuleControl rule in ruleControlsFirestore)
					{
						if (rule.Direction)
							mqttReciverClientFirestore.Subscribe(rule.MQTT_topic);
						else
							firestoreService.Subscribe(new FirestorePath(rule.FirebaseReference));

						string text = $"Activate rule: [{rule.FirebaseReference}] {(rule.Direction ? "<" : ">")} [{rule.MQTT_topic}].";
						AddDataToText(text);
					}
				}
			}
		}

		private void buttonAddRuleFirestore_Click(object sender, EventArgs e)
		{
			AddRuleFirestore(string.Empty, string.Empty, false);
			if (ruleControlsFirestore.Count != 0)
			{
				buttonStartStopFirestore.Enabled = true;
			}
		}
	}
}
