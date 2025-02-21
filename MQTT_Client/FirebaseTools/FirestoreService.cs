using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using MQTT_Client;
using MQTT_Client.FirebaseTools;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		await documentRef.SetAsync(data, SetOptions.MergeAll);
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
		try
		{
			var documentRef = _firestoreDb.Collection(path.Path).Document(path.Document);
			await documentRef.UpdateAsync(updates);
		}
		catch (Exception ex)
		{
			logger.Error($"Error {ex.Message}");
		}
	}

	// Удаление документа
	public async Task DeleteDataAsync(FirestorePath path)
	{
		var documentRef = _firestoreDb.Collection(path.Path).Document(path.Document);
		await documentRef.DeleteAsync();
	}

	public async Task<string> ConvertFieldToCollectionAsync<T>(FirestorePath path)
	{
		try
		{
			// Генерируем уникальное имя для документа (например, используя GUID)
			string documentName = Guid.NewGuid().ToString("N"); // Уникальное имя документа

			// Создаем новый объект (документ внутри коллекции)
			var newDocument = new Dictionary<string, object>
		{
			{ "count", "0" } // Добавляем поле count
        };

			// Записываем новый объект в Firebase по пути path.Path/path.Document/path.Field/documentName
			var documentReference = _firestoreDb
				.Collection(path.Path) // Путь к коллекции
				.Document(path.Document) // Имя документа
				.Collection(path.Field) // Поле, которое станет коллекцией
				.Document(documentName); // Уникальное имя документа

			await documentReference.SetAsync(newDocument); // Записываем данные

			// Удаляем исходное поле из документа
			var parentDocumentReference = _firestoreDb
				.Collection(path.Path) // Путь к коллекции
				.Document(path.Document); // Имя документа

			await parentDocumentReference.UpdateAsync(new Dictionary<string, object>
		{
			{ path.Field, FieldValue.Delete } // Удаляем поле
        });

			// Возвращаем имя созданного документа
			return documentName;
		}
		catch (Exception ex)
		{
			// Логируем ошибку, если необходимо
			logger.Error($"Exception: {ex.Message}");
			throw; // Пробрасываем исключение дальше
		}
	}

	public async Task<int> AddCountFieldToDocumentAsync(FirestorePath path)
	{
		try
		{
			// Получаем ссылку на документ по указанному пути
			var documentReference = _firestoreDb
				.Collection(path.Path) // Путь к коллекции
				.Document(path.Document); // Имя документа

			// Получаем снимок документа
			var documentSnapshot = await documentReference.GetSnapshotAsync();

			// Если документ существует
			if (documentSnapshot.Exists)
			{
				// Получаем данные документа в виде словаря
				var data = documentSnapshot.ToDictionary();

				// Считаем количество полей (исключая поле "count", если оно уже есть)
				int result = data.Keys.Count(k => k != "count");

				// Добавляем или обновляем поле "count" с количеством полей
				data["count"] = result.ToString();

				// Обновляем документ в Firestore
				await documentReference.SetAsync(data, SetOptions.MergeAll);

				return result;
			}
			else
			{
				throw new Exception("Документ не найден по указанному пути.");
			}
		}
		catch (Exception ex)
		{
			// Логируем ошибку, если необходимо
			logger.Error($"Exception: {ex.Message}");
			throw; // Пробрасываем исключение дальше
		}
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
