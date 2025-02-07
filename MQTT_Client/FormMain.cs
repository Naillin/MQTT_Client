using IniParser;
using IniParser.Model;
using NLog;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
		private const string filePathConfig = "config.ini";

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
			}
			else
			{
				IniData data = new IniData();
				data.Sections.AddSection("Settings");
				data["Settings"]["ADDRESS"] = ADDRESS;
				data["Settings"]["PORT"] = PORT.ToString();
				data["Settings"]["LOGIN"] = LOGIN;
				data["Settings"]["PASSWORD"] = PASSWORD;
				parser.WriteFile(filePathConfig, data);
			}

			configTextDefault = $"ADDRESS={ADDRESS}\r\n" +
								$"PORT={PORT.ToString()}\r\n" +
								$"LOGIN={LOGIN}\r\n" +
								$"PASSWORD={PASSWORD}";
		}

		public FormMain()
		{
			InitializeComponent();

			this.MaximumSize = new Size(866, 589);
			this.MinimumSize = new Size(866, 589);
			buttonAddTopic.Focus();
		}

		MqttBrokerClient mqttBrokerClient;
		string textBuffer = string.Empty;
		private void FormMain_Load(object sender, EventArgs e)
		{
			textBuffer = textBuffer + Properties.Resources.connecting_string + Environment.NewLine;
			textBoxInfo.Text = textBuffer;
			labelTopic.Text = Properties.Resources.topic_string;

			try
			{
				initConfig();

				mqttBrokerClient = new MqttBrokerClient(IPAddress.Parse(ADDRESS), PORT, LOGIN, PASSWORD);
				//mqttBrokerClient.Topic = TOPIC;
				mqttBrokerClient.Connect();

				mqttBrokerClient.MessageReceived += (senderMQTT, eMQTT) =>
				{
					string prePostString = Properties.Resources.notification_string + $"Topic: [{eMQTT.Topic}] Message: [{eMQTT.Payload}]";
					lock (textBuffer) { textBuffer = textBuffer + prePostString + Environment.NewLine; }

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
				};

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
				lock (textBuffer) { textBuffer = textBuffer + Properties.Resources.subscribe_topic_string + currentTopic + Environment.NewLine; }
				lock (textBoxInfo) { textBoxInfo.Text = textBuffer; }

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
				lock (textBuffer) { textBuffer = textBuffer + Properties.Resources.disconnecting_string + Environment.NewLine; }
				lock (textBoxInfo) { textBoxInfo.Text = textBuffer; }

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
							

							lock (textBuffer) { textBuffer = textBuffer + Properties.Resources.connected_string + Environment.NewLine; }
							lock (textBoxInfo) { textBoxInfo.Invoke(new Action(() => textBoxInfo.Text = textBuffer)); }

							status = false;
						}
					}
					else
					{
						pictureBoxStatus.Invoke(new Action(() => pictureBoxStatus.BackgroundImage = Properties.Resources.red));
						labelStatus.Invoke(new Action(() => labelStatus.Text = Properties.Resources.not_connected_string));

						lock (textBuffer)
						{
							textBuffer = textBuffer + Properties.Resources.not_connected_string + Environment.NewLine;
							textBuffer = textBuffer + Properties.Resources.reconnect_string + Environment.NewLine;
						}
						lock (textBoxInfo) { textBoxInfo.Invoke(new Action(() => textBoxInfo.Text = textBuffer)); }
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
	}
}
