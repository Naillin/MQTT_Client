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
		private string ADDRESS = "127.0.0.1";
		private int PORT = 1883;
		private string LOGIN = "user_login";
		private string PASSWORD = "user_password";
		private const string filePathConfig = "config.txt";

		public FormMain()
		{
			InitializeComponent();

			this.MaximumSize = new Size(866, 589);
			this.MinimumSize = new Size(866, 589);
			buttonAddTopic.Focus();
		}

		MqttReceiver mqttReceiver;
		string textBuffer = string.Empty;
		private void FormMain_Load(object sender, EventArgs e)
		{
			textBuffer = textBuffer + Properties.Resources.connecting_string + Environment.NewLine;
			textBoxInfo.Text = textBuffer;
			labelTopic.Text = Properties.Resources.topic_string;

			try
			{
				initConfig();

				mqttReceiver = new MqttReceiver(IPAddress.Parse(ADDRESS), PORT, LOGIN, PASSWORD);
				//mqttReceiver.Topic = TOPIC;
				mqttReceiver.Start();

				mqttReceiver.OnMessageReceived += (message) =>
				{
					string prePostString = Properties.Resources.notification_string + message;
					lock (textBuffer) { textBuffer = textBuffer + prePostString + Environment.NewLine; }

					if (textBoxInfo.InvokeRequired)
					{
						lock (textBoxInfo) { textBoxInfo.Invoke(new Action(() => textBoxInfo.Text = textBuffer)); }
					}
					else
					{
						lock (textBoxInfo) { textBoxInfo.Text = textBuffer; }
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

		public void initConfig()
		{
			if (File.Exists(filePathConfig))
			{
				string[] linesConfig = File.ReadAllLines(filePathConfig);
				ADDRESS = linesConfig[0].Split('=')[1];
				PORT = Convert.ToInt32(linesConfig[1].Split('=')[1]);
				LOGIN = linesConfig[2].Split('=')[1];
				PASSWORD = linesConfig[3].Split('=')[1];
			}
			else
			{
				string configTextDefault = $"ADDRESS=127.0.0.1\r\n" +
										   $"PORT=1883\r\n" +
										   $"LOGIN=user_login\r\n" +
										   $"PASSWORD=user_password";
				File.WriteAllText(filePathConfig, configTextDefault);
			}
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			string message = textBoxMessage.Text;
			textBoxMessage.Text = string.Empty;
			if (mqttReceiver.IsConnected())
			{
				if (!string.IsNullOrEmpty(message))
				{
					try
					{
						mqttReceiver.Publish(message);
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

		private void buttonAddTopic_Click(object sender, EventArgs e)
		{
			InputBox inputBox = new InputBox(Properties.Resources.topic_adding_string, Properties.Resources.enter_topic_name_string);
			if (inputBox.ShowDialog().Equals(DialogResult.OK))
			{
				string topic = inputBox.InputText;
				if (!string.IsNullOrEmpty(topic))
				{
					TreeViewFiller(ref treeView1, topic);
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

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			pictureBoxStatus.BackgroundImage = Properties.Resources.red;
			labelStatus.Text = Properties.Resources.not_connected_string;

			try
			{
				TreeNode selectedNode = e.Node;
				string topic = GetFullPath(selectedNode);
				mqttReceiver.Topic = topic;

				mqttReceiver.Reconnect();
				labelTopic.Text = Properties.Resources.topic_string + topic;
				lock (textBuffer) { textBuffer = textBuffer + Properties.Resources.subscribe_topic_string + topic + Environment.NewLine; }
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

				mqttReceiver.Disconnect();
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
					if (mqttReceiver.IsConnected())
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
						mqttReceiver.Reconnect();

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
				mqttReceiver.Disconnect();
			}
		}
	}
}
