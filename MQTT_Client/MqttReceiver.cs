using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;
using System.Net;
using System.Runtime.Remoting.Contexts;

namespace MQTT_Client
{
	internal class MqttReceiver
	{
		private IPAddress _brokerAddress; // IP-адрес брокера
		private int _port;                 // Порт
		private string _username;          // Имя пользователя
		private string _password;          // Пароль
		private string _topic = "room";    // Тема для подписки
		private MqttClient client;         // MQTT-клиент
		private string _message = string.Empty; // Для хранения последнего сообщения

		internal MqttReceiver(IPAddress brokerAddress, int port, string username, string password)
		{
			_brokerAddress = brokerAddress;
			_port = port;
			_username = username;
			_password = password;
		}

		public string Topic
		{
			get { return _topic; }
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_topic = value;
				}
			}
		}

		public void Start()
		{
			// Создаем MQTT-клиент и подключаемся к брокеру
			client = new MqttClient(_brokerAddress.ToString(), _port, false, null, null, MqttSslProtocols.None);
			string clientId = Guid.NewGuid().ToString();
			try
			{
				client.Connect(clientId, _username, _password); // Используем имя пользователя и пароль
			}
			catch (Exception ex) { }

			client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
			// Подписываемся на тему
			client.Subscribe(new string[] { _topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
		}

		public void Reconnect()
		{
			Disconnect();
			client = null;

			// Создаем MQTT-клиент и подключаемся к брокеру
			client = new MqttClient(_brokerAddress.ToString(), _port, false, null, null, MqttSslProtocols.None);
			string clientId = Guid.NewGuid().ToString();
			try
			{
				client.Connect(clientId, _username, _password); // Используем имя пользователя и пароль
			}
			catch (Exception ex) { }

			client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
			// Подписываемся на тему
			client.Subscribe(new string[] { _topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
		}

		// Обработка полученных сообщений
		public event Action<string> OnMessageReceived; // Событие, которое будет вызываться при получении нового сообщения
		private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
		{
			_message = Encoding.UTF8.GetString(e.Message);
			OnMessageReceived?.Invoke(_message); // Вызываем событие с полученным сообщением
		}

		// Метод для получения последнего сообщения
		public string GetLatestMessage()
		{
			return _message; // Возвращаем последнее полученное сообщение
		}

		public void Disconnect()
		{
			if (client != null && client.IsConnected)
			{
				client.Disconnect(); // Отключаемся от брокера
			}
		}

		// Проверка соединения
		public bool IsConnected()
		{
			return client.IsConnected;
		}

		public void Publish(string message)
		{
			byte[] messageBytes = Encoding.UTF8.GetBytes(message);
			client.Publish(_topic, messageBytes);
		}
	}
}
