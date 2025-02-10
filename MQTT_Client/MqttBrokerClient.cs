using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;
using System.Net;
using NLog;

namespace MQTT_Client
{
	internal class MqttBrokerClient
	{
		private static readonly string moduleName = "MqttBrokerClient";
		private static readonly Logger baseLogger = LogManager.GetLogger(moduleName);
		private static readonly LoggerManager logger = new LoggerManager(baseLogger, moduleName);

		private string _clientId;
		private string _username;          // Имя пользователя
		private string _password;          // Пароль

		private MqttClient _mqttClient;

		// Событие для получения входящих сообщений
		public event EventHandler<MqttMessageReceivedEventArgs> MessageReceived;

		public MqttBrokerClient(string brokerAddress, int port, string username, string password)
		{
			_clientId = Guid.NewGuid().ToString();
			_username = username;
			_password = password;

			// Создание клиента MQTT
			_mqttClient = new MqttClient(brokerAddress, port, false, null, null, MqttSslProtocols.None);

			// Настройка обработчиков событий
			_mqttClient.MqttMsgPublishReceived += OnMqttMsgPublishReceived;
			_mqttClient.ConnectionClosed += OnConnectionClosed;
		}

		private bool _isReconnecting = true;
		public bool isReconnecting
		{
			get
			{
				return _isReconnecting;
			}
			set
			{
				_isReconnecting = value;	
			}
		}

		// Подключение к брокеру
		public void Connect()
		{
			if (!_mqttClient.IsConnected)
			{
				try
				{
					logger.Info("Connecting to MQTT broker...");
					_mqttClient.Connect(_clientId, _username, _password);
					_isReconnecting = true;
					logger.Info("Connected to MQTT broker.");
				}
				catch (Exception ex)
				{
					logger.Error($"Connection failed: {ex.Message}");
				}
			}
		}

		// Отключение от брокера
		public void Disconnect()
		{
			if (_mqttClient.IsConnected)
			{
				logger.Info("Disconnecting from MQTT broker...");
				_isReconnecting = false;
				_mqttClient.Disconnect();
				logger.Info("Disconnected from MQTT broker.");
			}
		}

		// Переподключение к брокеру
		private void Reconnect()
		{
			while (!_mqttClient.IsConnected)
			{
				try
				{
					logger.Info("Reconnecting to MQTT broker...");
					_mqttClient.Connect(_clientId);
					logger.Info("Reconnected to MQTT broker.");
					_isReconnecting = true;
				}
				catch (Exception ex)
				{
					logger.Error($"Reconnection failed: {ex.Message}");
					System.Threading.Thread.Sleep(5000); // Ждем 0.300 секунд перед повторной попыткой
				}
			}
		}

		// Проверка подключения
		public bool IsConnected()
		{
			return _mqttClient.IsConnected;
		}

		// Подписка на топик
		public void Subscribe(string topic)
		{
			if (_mqttClient.IsConnected)
			{
				logger.Info($"Subscribing to topic: {topic}");
				_mqttClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
				logger.Info($"Subscribed to topic: {topic}");
			}
			else
			{
				logger.Error("Client is not connected. Cannot subscribe.");
			}
		}

		// Подписка на несколько топиков
		public void Subscribe(string[] topics)
		{
			if (_mqttClient.IsConnected)
			{
				byte[] qosLevels = new byte[topics.Length];
				for (int i = 0; i < topics.Length; i++)
				{
					qosLevels[i] = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
					logger.Info($"Subscribing to topic: {topics[i]}");
				}

				_mqttClient.Subscribe(topics, qosLevels);
				logger.Info("Subscribed to all topics.");
			}
			else
			{
				logger.Error("Client is not connected. Cannot subscribe.");
			}
		}

		// Отписка от топика
		public void Unsubscribe(string topic)
		{
			if (_mqttClient.IsConnected)
			{
				logger.Info($"Unsubscribing from topic: {topic}");
				_mqttClient.Unsubscribe(new string[] { topic });
				logger.Info($"Unsubscribed from topic: {topic}");
			}
			else
			{
				logger.Error("Client is not connected. Cannot unsubscribe.");
			}
		}

		// Отписка от нескольких топиков
		public void Unsubscribe(string[] topics)
		{
			if (_mqttClient.IsConnected)
			{
				logger.Info("Unsubscribing from topics:");
				foreach (var topic in topics)
				{
					logger.Info($"- {topic}");
				}

				_mqttClient.Unsubscribe(topics);
				logger.Info("Unsubscribed from all specified topics.");
			}
			else
			{
				logger.Error("Client is not connected. Cannot unsubscribe.");
			}
		}

		// Отправка сообщения в топик
		public void Publish(string topic, string payload)
		{
			if (_mqttClient.IsConnected)
			{
				_mqttClient.Publish(topic, Encoding.UTF8.GetBytes(payload), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
				logger.Info($"Message published to topic: {topic}");
			}
			else
			{
				logger.Error("Client is not connected. Cannot publish.");
			}
		}

		// Обработчик события получения сообщения
		private void OnMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
		{
			var payload = Encoding.UTF8.GetString(e.Message);
			logger.Info($"Received message: {payload} from topic: {e.Topic}");

			// Вызов события для обработки сообщения
			MessageReceived?.Invoke(this, new MqttMessageReceivedEventArgs
			{
				Topic = e.Topic,
				Payload = payload
			});
		}

		// Обработчик события закрытия соединения
		private void OnConnectionClosed(object sender, EventArgs e)
		{
			logger.Info("Disconnected from MQTT broker.");
			if (!_mqttClient.IsConnected && _isReconnecting)
			{
				Reconnect();
			}
		}
	}

	// Класс для передачи аргументов события
	public class MqttMessageReceivedEventArgs : EventArgs
	{
		public string Topic { get; set; }
		public string Payload { get; set; }
	}
}
