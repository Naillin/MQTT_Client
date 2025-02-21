
namespace MQTT_Client.FormElements
{
	internal class RuleUnit
	{
		public string FirebaseReference { get; set; } = string.Empty;
		public string MQTT_topic { get; set; } = string.Empty;
		public bool Direction { get; set; } = false;
		public bool NewField { get; set; } = false;
		public bool Timestamp { get; set; } = false;

		public RuleUnit(string FirebaseReference, string MQTT_topic, bool Direction, bool NewField, bool Timestamp)
		{
			this.FirebaseReference = FirebaseReference;
			this.MQTT_topic = MQTT_topic;
			this.Direction = Direction;
			this.NewField = NewField;
			this.Timestamp = Timestamp;
		}
	}
}
