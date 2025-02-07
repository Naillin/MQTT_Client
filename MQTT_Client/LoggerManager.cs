using NLog;
using System;

namespace MQTT_Client
{
	internal class LoggerManager
	{
		private Logger _logger;
		private string _moduleName = string.Empty;

		public LoggerManager(Logger logger, string moduleName = "class")
		{
			this._logger = logger;
			this._moduleName = moduleName;
		}

		public void Info(string text)
		{
			_logger.Info(text);
			Console.WriteLine($"{_moduleName} - {text}");
		}

		public void Warn(string text)
		{
			_logger.Warn(text);
			Console.WriteLine($"{_moduleName} - {text}");
		}

		public void Error(string text)
		{
			_logger.Error(text);
			Console.WriteLine($"{_moduleName} - {text}");
		}

		public void Trace(string text)
		{
			_logger.Trace(text);
			Console.WriteLine($"{_moduleName} - {text}");
		}
	}
}
