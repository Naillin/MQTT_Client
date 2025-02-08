using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace MQTT_Client.FormElements
{
	public partial class RuleControl : UserControl
	{
		private string _firebaseReference = string.Empty;
		public string FirebaseReference
		{
			get
			{
				return _firebaseReference;
			}
			set
			{
				_firebaseReference = value;
				textBoxFirebaseReference.Text = value;
			}
		}
		private string _MQTT_topic = string.Empty;
		public string MQTT_topic
		{
			get
			{
				return _MQTT_topic;
			}
			set
			{
				_MQTT_topic = value;
				textBoxMQTTtopic.Text = value;
			}
		}
		private bool _direction = false;
		public bool Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
				if (value)
					buttonSwitch.Text = "<";
				else
					buttonSwitch.Text = ">";
			}
		}

		public RuleControl(string FirebaseReference, string MQTT_topic, bool Direction)
		{
			InitializeComponent();

			this.FirebaseReference = FirebaseReference;
			this.MQTT_topic = MQTT_topic;
			this.Direction = Direction;
		}

		public event EventHandler ButtonDeleteClicked;
		private void buttonDelete_Click(object sender, EventArgs e)
		{
			ButtonDeleteClicked?.Invoke(this, EventArgs.Empty);
		}

		private void buttonSwitch_Click(object sender, EventArgs e)
		{
			Direction = !Direction;
		}

		private void textBoxFirebaseReference_TextChanged(object sender, EventArgs e)
		{
			_firebaseReference = textBoxFirebaseReference.Text;
		}

		private void textBoxMQTTtopic_TextChanged(object sender, EventArgs e)
		{
			_MQTT_topic = textBoxMQTTtopic.Text;
		}
	}
}
