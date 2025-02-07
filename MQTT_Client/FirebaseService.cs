using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTT_Client
{
	internal class FirebaseService
	{
		private readonly FirebaseClient _firebase;

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
	}
}
