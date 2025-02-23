using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MQTT_Client
{
	public partial class InputBox : Form
	{
		public InputBox(string title, string prompt)
		{
			InitializeComponent();

			Text = title;
			labelInput.Text = prompt;
		}

		public string InputText { get; private set; } // Свойство для хранения введенного текста
		private void buttonInput_Click(object sender, EventArgs e)
		{
			string result = textBoxInput.Text; // Сохраняем текст из текстового поля
			if (!string.IsNullOrEmpty(result))
			{
				InputText = textBoxInput.Text;
				DialogResult = DialogResult.OK;     // Устанавливаем результат диалога
			}
			
			Close();                            // Закрываем форму
		}
	}
}
