using Firebase.Database;
using Firebase.Database.Query;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTT_Client
{
	internal class FirebaseService
	{
		private static readonly string moduleName = "FirebaseService";
		private static readonly Logger baseLogger = LogManager.GetLogger(moduleName);
		private static readonly LoggerManager logger = new LoggerManager(baseLogger, moduleName);

		private readonly FirebaseClient _firebase;
		private readonly Dictionary<string, IDisposable> _subscriptions = new Dictionary<string, IDisposable>(); // Словарь для подписок

		// Событие для уведомления о получении данных
		public event EventHandler<FirebaseReceivedEventArgs> DataReceived;

		public FirebaseService(string firebaseDatabaseUrl, string secret)
		{
			_firebase = new FirebaseClient(
				firebaseDatabaseUrl,
				new FirebaseOptions
				{
					AuthTokenAsyncFactory = () => Task.FromResult(secret)
				});
		}

		// Добавление данных
		public async Task AddDataAsync<T>(string path, T data)
		{
			await _firebase
				.Child(path)
				.PostAsync(data);
		}

		// Получение данных
		public async Task<T> GetDataAsync<T>(string path)
		{
			var data = await _firebase
				.Child(path)
				.OnceSingleAsync<T>();

			return data;
		}

		// Обновление данных
		public async Task UpdateDataAsync<T>(string path, T data)
		{
			await _firebase
				.Child(path)
				.PutAsync(data);
		}

		// Удаление данных
		public async Task DeleteDataAsync(string path)
		{
			await _firebase
				.Child(path)
				.DeleteAsync();
		}

		// Получение списка данных
		public async Task<List<T>> GetListDataAsync<T>(string path)
		{
			var data = await _firebase
				.Child(path)
				.OnceAsync<T>();

			return data.Select(item => item.Object).ToList();
		}

		//КОРОЧЕ НИХРЕНА НЕ РАБОТАЕТ ЭТОТ СУБСКРАЙБ. пока что можно сделать опрашивалку раз в секунду по ключу с помощью GetDataAsync. данные fb старые хранить по <ключ=путь, значение> в словаре.
		// Подписка на изменения в данных
		public IDisposable Subscribe(string path)
		{
			// Если подписка уже существует, не создаем новую
			if (_subscriptions.ContainsKey(path))
			{
				return _subscriptions[path]; // Возвращаем существующую подписку
			}

			var subscription = _firebase
				.Child(path)
				.AsObservable<object>() // не срабатывает
				.Subscribe(change =>
				{
					OnDataReceived(change.Key, change.Object);
				}, ex =>
				{
					logger.Error($"Error: {ex.Message}");
				});

			_subscriptions[path] = subscription; // Сохраняем подписку в словарь
			return subscription;
		}

		// Подписка на изменения в данных с фильтрацией по ключу
		public IDisposable Subscribe(string path, string key)
		{
			// Если подписка уже существует, не создаем новую
			var fullPath = $"{path}/{key}"; // Путь с ключом
			if (_subscriptions.ContainsKey(fullPath))
			{
				return _subscriptions[fullPath]; // Возвращаем существующую подписку
			}

			var subscription = _firebase
				.Child(path)
				.Child(key)
				.AsObservable<object>()
				.Subscribe(change =>
				{
					OnDataReceived(change.Key, change.Object);
				});

			_subscriptions[fullPath] = subscription; // Сохраняем подписку в словарь
			return subscription;
		}

		// Метод для вызова события
		protected virtual void OnDataReceived(string key, object data)
		{
			DataReceived?.Invoke(this, new FirebaseReceivedEventArgs
			{
				Key = key,
				Data = data
			});
		}

		// Метод для отписки по пути
		public void Unsubscribe(string path)
		{
			if (_subscriptions.ContainsKey(path))
			{
				_subscriptions[path].Dispose(); // Отписка от подписки
				_subscriptions.Remove(path); // Удаляем подписку из словаря
			}
		}

		// Метод для отписки от всех подписок
		public void UnsubscribeAll()
		{
			foreach (var subscription in _subscriptions.Values)
			{
				subscription.Dispose(); // Отписка от всех подписок
			}
			_subscriptions.Clear(); // Очищаем словарь подписок
		}
	}

	// Класс для передачи аргументов события
	public class FirebaseReceivedEventArgs : EventArgs
	{
		public string Key { get; set; }
		public object Data { get; set; }
	}
}
