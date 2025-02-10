# MQTT Client Application

MQTT_client is an application that allows you to connect to MQTT brokers, subscribe to topics, publish data to them, and link Firebase fields to MQTT topics using rules.

## Installation and Configuration

1. After the first launch, the program will create a `config.ini` file in the root directory.
2. In the `config.ini` file, you need to specify the following details:
   - Your MQTT broker details (address, port).
   - Login and password for connecting to the MQTT broker.
   - Firebase database URL.
   - Secret key for connecting to Firebase.
3. After configuring `config.ini`, restart the application.

## Key Features

### "MQTT Client" Tab

- **Connecting to an MQTT Broker**: Specify the broker details in `config.ini`, and the program will automatically connect to it.
- **Subscribing to Topics**: Add topics in the format `path/to/topic`. The interface displays topics in a tree structure for easier navigation.
- **Publishing Data**: You can send messages to any topic you are subscribed to.

### "Rules" Tab

- **Creating Rules**: Link Firebase database fields to MQTT topics. Rules allow automatic synchronization of data between Firebase and MQTT.

## Usage Example

1. Launch the application and configure `config.ini`.
2. Go to the "MQTT Client" tab and add topics to subscribe to.
3. Use the "Rules" tab to create rules linking Firebase and MQTT.

## License

MIT License.
