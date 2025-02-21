using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json.Linq;
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

		public async Task<bool> IsNodeACollectionAsync(string path)
		{
			try
			{
				// Получаем данные по указанному пути как JToken
				var data = await _firebase
					.Child(path)
					.OnceSingleAsync<JToken>();

				// Проверяем, является ли data объектом (JObject)
				if (data != null && data.Type == JTokenType.Object)
				{
					return true; // Это коллекция (объект)
				}
				else
				{
					return false; // Это поле с данными (примитив)
				}
			}
			catch (FirebaseException ex)
			{
				// Логируем ошибку, если необходимо
				logger.Error($"FirebaseException: {ex.Message}");
				return false; // В случае ошибки считаем, что это не коллекция
			}
		}

		public async Task ConvertFieldToCollectionAsync<T>(string path)
		{
			// Получаем текущее значение поля
			var currentValue = await _firebase
				.Child(path)
				.OnceSingleAsync<T>();

			// Создаем новый объект (коллекцию)
			var newCollection = new Dictionary<string, object>
			{
				{ "count", "0" }            // Добавляем поле count
			};

			// Записываем новый объект обратно в Firebase
			await _firebase
				.Child(path)
				.PutAsync(newCollection);
		}

		public async Task<int> AddCountFieldToCollectionAsync(string path)
		{
			int result = 0;

			// Получаем текущую коллекцию по указанному пути
			var collection = await _firebase
				.Child(path)
				.OnceSingleAsync<Dictionary<string, object>>();

			// Если коллекция существует
			if (collection != null)
			{
				// Считаем количество полей (исключая поле "count", если оно уже есть)
				result = collection.Keys.Count(k => k != "count");

				// Добавляем или обновляем поле "count" с количеством полей
				collection["count"] = result.ToString();

				// Записываем обновленную коллекцию обратно в Firebase
				await _firebase
					.Child(path)
					.PutAsync(collection);

				return result;
			}
			else
			{
				throw new Exception("Коллекция не найдена по указанному пути.");
			}
		}

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
