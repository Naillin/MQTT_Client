# MQTT Client Application

[Русская версия](README.ru.md)
---
[Linux](https://github.com/Naillin/MQTT_Rules.git)

MQTT_client is an application that allows you to connect to MQTT brokers, subscribe to topics, publish data to them, and link Firebase/Firestore fields to MQTT topics using rules.

## Installation and Setup

1. First, you need to obtain a secret key if you plan to use Firebase. If you intend to use Firestore, you must obtain a `.json` file from the Firebase Admin SDK and enable the Cloud Firestore API for your project. You can use both databases simultaneously if needed.
2. After the first launch, the program will create a `config.ini` file in the root directory.
3. In the `config.ini` file, you need to specify the following data:
   - Your MQTT broker details (address, port).
   - Login and password for connecting to the MQTT broker.
   - Firebase database URL.
   - Secret key for connecting to Firebase.
   - Project identifier.
   - Path to the Firebase Admin SDK JSON file.
4. After configuring `config.ini`, restart the application.

## Main Features

### "MQTT Client" Tab

- **Connecting to an MQTT Broker**: Specify the broker details in `config.ini`, and the program will automatically connect to it.
- **Subscribing to Topics**: Add topics in the format `path/to/topic`. The interface displays topics in a tree structure for easy navigation.
- **Publishing Data**: You can send messages to any topic you are subscribed to.

### "Firebase Rules/Firestore Rules" Tab

- **Rule Creation**: Link Firebase/Firestore database fields with MQTT topics. Rules allow automatic data synchronization between Firebase/Firestore and MQTT. All rules are stored in the root file `rulesFirebase.json`/`rulesFirestore.json`. (Firebase field references should start with the `/` symbol, e.g., `/switch1/data`).
- **Creating a New Field**: For a rule, you can configure the ability to create a new field for data received from the MQTT broker. Use the `Create new field` property for this. If this option is enabled, the direction for this rule will be set towards Firebase/Firestore.
- **Timestamp**: For a rule, you can configure the ability to add a timestamp to data sent to Firebase/Firestore. Use the `Add timestamp` property for this. This option is available only if the `Create new field` property is active.
- **IMPORTANT!!!**: Rules only work with string-type fields! You cannot use collection paths or root paths in Firebase/Firestore for topics! This may lead to errors or unexpected behavior in rule execution! Also, note that the program works with data in string representation. If you create a field with a numeric type, after rewriting the field using a rule, the field will have a string type.

## Example Usage

1. Launch the application and configure `config.ini`.
2. Go to the "MQTT Client" tab and add topics to subscribe to.
3. Use the "Firebase Rules/Firestore Rules" tab to create rules linking Firebase and MQTT.
4. Create a rule using the `Add Rule` button and edit its parameters (Firebase/Firestore paths, data direction, MQTT topic).
5. Click the `Start` button and try changing the tracked field or publishing data to the topic.
6. Click the `Stop` button to stop the rules from running.

## License

MIT License.

## Donate

[Thank you so much!❤️](https://boosty.to/naillin/donate)
