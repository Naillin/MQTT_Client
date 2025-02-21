using System;
using System.Collections.Generic;

namespace MQTT_Client.FirebaseTools
{
	public class FirestorePath
	{
		private string _sourcePath = string.Empty;
		private string _path = string.Empty;
		private string _document = string.Empty;
		private string _field = string.Empty;

		public string SourcePath
		{
			get { return _sourcePath; }
			set { _sourcePath = value; }
		}
		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}
		public string Document
		{
			get { return _document; }
			set { _document = value; }
		}
		public string Field
		{
			get { return _field; }
			set { _field = value; }
		}

		public FirestorePath(string path = "")
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

		public string GetPathWithDocument()
		{
			var result = new List<string>();

			if (!string.IsNullOrEmpty(_path))
				result.Add(_path);

			if (!string.IsNullOrEmpty(_document))
				result.Add(_document);

			return string.Join("/", result);
		}

		// Метод для смещения значений
		public FirestorePath Shift(bool accept = false)
		{
			// Разделяем путь на части
			string[] pathParts = _sourcePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			if (pathParts.Length <= 2)
			{
				return this;
			}

			if (accept)
			{
				// Сохраняем текущее значение Document
				string previousDocument = _document;

				// Document принимает значение Field
				_document = _field;

				// Field становится пустым
				_field = string.Empty;

				// Path получает к себе прошлое значение Document через символ /
				if (!string.IsNullOrEmpty(previousDocument))
				{
					if (!string.IsNullOrEmpty(_path))
					{
						_path += "/" + previousDocument;
					}
					else
					{
						_path = previousDocument;
					}
				}

				return this;
			}
			else
			{
				return new FirestorePath(this.ToString()).Shift(true);
			}
		}

		public bool IsOdd()
		{
			// Разделяем путь на части
			string[] pathParts = _sourcePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

			// Проверяем, является ли количество элементов нечетным
			return pathParts.Length % 2 != 0;
		}

		public override string ToString()
		{
			var result = new List<string>();

			if (!string.IsNullOrEmpty(_path))
				result.Add(_path);

			if (!string.IsNullOrEmpty(_document))
				result.Add(_document);

			if (!string.IsNullOrEmpty(_field))
				result.Add(_field);

			return string.Join("/", result);
		}
	}
}
