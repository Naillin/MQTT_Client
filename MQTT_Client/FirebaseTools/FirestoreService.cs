using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using MQTT_Client;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

internal class FirestoreService
{
	private static readonly string moduleName = "FirestoreService";
	private static readonly Logger baseLogger = LogManager.GetLogger(moduleName);
	private static readonly LoggerManager logger = new LoggerManager(baseLogger, moduleName);

	private readonly FirestoreDb _firestoreDb;
	private readonly ConcurrentDictionary<string, FirestoreChangeListener> _subscriptions;
	// Событие для уведомления о получении данных
	public event EventHandler<FirestoreReceivedEventArgs> OnMessage;

	public FirestoreService(string projectId, string serviceAccountJsonPath)
	{
		// Чтение JSON-файла с ключами сервисного аккаунта
		var jsonCredentials = File.ReadAllText(serviceAccountJsonPath);

		// Создание клиента Firestore с использованием JSON-ключа
		var clientBuilder = new FirestoreClientBuilder
		{
			JsonCredentials = jsonCredentials
		};
		var client = clientBuilder.Build();

		// Инициализация Firestore
		_firestoreDb = FirestoreDb.Create(projectId, client);

		// Инициализация словаря для хранения подписок
		_subscriptions = new ConcurrentDictionary<string, FirestoreChangeListener>();
	}

	// Добавление данных в коллекцию
	public async Task AddDataAsync<T>(FirestorePath path, T data)
	{
		var collectionRef = _firestoreDb.Collection(path.Path);
		var documentRef = collectionRef.Document(path.Document);
		await documentRef.SetAsync(data);
	}

	// Получение данных из документа
	public async Task<T> GetDataAsync<T>(FirestorePath path)
	{
		var documentRef = _firestoreDb.Collection(path.Path).Document(path.Document);
		var snapshot = await documentRef.GetSnapshotAsync();

		if (snapshot.Exists)
		{
			return snapshot.ConvertTo<T>();
		}

		throw new Exception("Document not found");
	}

	// Получение данных из конкретного поля документа
	public async Task<T> GetFieldAsync<T>(FirestorePath path)
	{
		var documentRef = _firestoreDb.Collection(path.Path).Document(path.Document);
		var snapshot = await documentRef.GetSnapshotAsync();

		if (snapshot.Exists)
		{
			// Проверяем, существует ли поле
			if (snapshot.ContainsField(path.Field))
			{
				// Получаем значение поля
				return snapshot.GetValue<T>(path.Field);
			}
			else
			{
				throw new Exception($"Field '{path.Field}' not found in document.");
			}
		}

		throw new Exception("Document not found");
	}

	// Обновление данных в документе
	public async Task UpdateDataAsync(FirestorePath path, Dictionary<string, object> updates)
	{
		var documentRef = _firestoreDb.Collection(path.Path).Document(path.Document);
		await documentRef.UpdateAsync(updates);
	}

	// Удаление документа
	public async Task DeleteDataAsync(FirestorePath path)
	{
		var documentRef = _firestoreDb.Collection(path.Path).Document(path.Document);
		await documentRef.DeleteAsync();
	}

	// Метод для добавления подписки на изменения документа
	public void Subscribe(FirestorePath path)
	{
		// Формируем ключ для подписки (путь + документ)
		string subscriptionKey = $"{path.Path}/{path.Document}";

		// Если подписка уже существует, выходим
		if (_subscriptions.ContainsKey(subscriptionKey))
		{
			return;
		}

		// Создаем ссылку на документ
		var documentRef = _firestoreDb.Collection(path.Path).Document(path.Document);

		// Подписываемся на изменения документа
		var listener = documentRef.Listen(snapshot =>
		{
			if (snapshot.Exists)
			{
				// Если указано поле, извлекаем его значение
				if (!string.IsNullOrEmpty(path.Field) && snapshot.ContainsField(path.Field))
				{
					var fieldValue = snapshot.GetValue<object>(path.Field);

					// Вызываем событие OnMessage
					OnMessage?.Invoke(this, new FirestoreReceivedEventArgs
					{
						Path = path,
						Data = fieldValue // Передаем только значение поля
					});
				}
				else
				{
					// Если поле не указано, передаем весь документ
					OnMessage?.Invoke(this, new FirestoreReceivedEventArgs
					{
						Path = path,
						Data = snapshot.ToDictionary()
					});
				}
			}
		});

		// Сохраняем подписку в словаре
		_subscriptions[subscriptionKey] = listener;
	}

	// Метод для удаления подписки
	public void Unsubscribe(FirestorePath path)
	{
		// Формируем ключ для подписки (путь + документ)
		string subscriptionKey = $"{path.Path}/{path.Document}";

		// Если подписка существует, останавливаем её и удаляем из словаря
		if (_subscriptions.TryRemove(subscriptionKey, out var listener))
		{
			listener.StopAsync();
		}
	}

	// Метод для удаления подписок
	public void UnsubscribeAll()
	{
		// Проходим по всем подпискам в словаре
		foreach (var subscription in _subscriptions)
		{
			// Останавливаем подписку
			subscription.Value.StopAsync();
		}

		// Очищаем словарь
		_subscriptions.Clear();
	}
}

public class FirestoreReceivedEventArgs : EventArgs
{
	public FirestorePath Path { get; set; }
	public object Data { get; set; }
}

public class FirestorePath
{
	private string _sourcePath = string.Empty;
	private string _path = string.Empty;
	private string _document = string.Empty;
	private string _field = string.Empty;

	public string SourcePath => _sourcePath;
	public string Path => _path;
	public string Document => _document;
	public string Field => _field;

	public FirestorePath(string path)
	{
		_sourcePath = path;

		// Убираем начальные и конечные слэши
		if (path.StartsWith("/"))
			path = path.Substring(1);
		if (path.EndsWith("/"))
			path = path.Substring(0, path.Length - 1);

		// Разделяем путь на части
		string[] pathParts = path.Split('/');

		// Обрабатываем путь в зависимости от количества частей
		if (pathParts.Length >= 3)
		{
			// Если частей 3 или больше:
			// - path: все части, кроме последних двух
			// - document: предпоследняя часть
			// - field: последняя часть
			_path = string.Join("/", pathParts, 0, pathParts.Length - 2);
			_document = pathParts[pathParts.Length - 2];
			_field = pathParts[pathParts.Length - 1];
		}
		else if (pathParts.Length == 2)
		{
			// Если частей 2:
			// - path: первая часть (коллекция)
			// - document: вторая часть (документ)
			// - field: пусто
			_path = pathParts[0];
			_document = pathParts[1];
			_field = string.Empty;
		}
		else if (pathParts.Length == 1)
		{
			// Если часть одна:
			// - path: пусто
			// - document: пусто
			// - field: единственная часть
			_path = string.Empty;
			_document = string.Empty;
			_field = pathParts[0];
		}
		else
		{
			// Если путь пустой
			_path = string.Empty;
			_document = string.Empty;
			_field = string.Empty;
		}
	}
}