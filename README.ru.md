# Приложение MQTT Client

[English version](README.md)
---
[Linux](https://github.com/Naillin/MQTT_Rules.git)

MQTT_client — это приложение, которое позволяет подключаться к MQTT-брокерам, подписываться на топики, публиковать в них данные, а также связывать поля Firebase/Firestore с MQTT-топиками с помощью правил.

## Установка и настройка

1. Для начала вам требуется получить секретный ключ если вы собираетесь использовать Firebase. Если вы планируете использовать Firestore то, вы должны получить `.json` файл Firebase Admin SDK и включить Cloud Firestore API для своего проекта. Ничто не мешает вам использовать обе базы данных одновременно.
2. После первого запуска программа создаст файл `config.ini` в корневой директории.
3. В файле `config.ini` необходимо указать следующие данные:
   - Данные вашего MQTT-брокера (адрес, порт).
   - Логин и пароль для подключения к MQTT-брокеру.
   - URL базы данных Firebase.
   - Секретный ключ для подключения к Firebase.
   - Идентификатор проекта.
   - Путь до `json` файла Firebase Admin SDK.
4. После настройки `config.ini` перезапустите приложение.

## Основные возможности

### Вкладка "MQTT Client"

- **Подключение к MQTT-брокеру**: Укажите данные брокера в `config.ini`, и программа автоматически подключится к нему.
- **Подписка на топики**: Добавляйте топики в формате `path/to/topic`. Интерфейс отображает топики в виде дерева для удобства навигации.
- **Публикация данных**: Вы можете отправлять сообщения в любой топик, на который подписаны.

### Вкладка "Firebase Rules/Firestore Rules"

- **Создание правил**: Связывайте поля базы данных Firebase/Firestore с MQTT-топиками. Правила позволяют автоматически синхронизировать данные между Firebase/Firestore и MQTT. Все правила хранятся в корне в файле `rulesFirebase.json`/`rulesFirestore.json`. (ссылки на поля Firebase следует начинать с символа `/`, то есть `/switch1/data`)
- **Создание нового поля**: Для правила можно настроить возможность создавать новое поля для новых данных полученных из MQTT-брокера для этого используйте свойство `Create new field`. Если активирована эта возможность, то направление для этого правила будет установлено в сторону Firebase/Firestore.
- **Временная метка**: Для правила можно настроить возможность добовлять временную метку к данным отправляемым в Firebase/Firestore для этого используйте свойство `Add timestamp`. Эта возможность доступна если активно свойство `Create new field`.
- **ВАЖНО!!!**: Правила работают только с строчными типами в полях! Вы не можете использовать пути из коллекций или корней в Firebase/Firestore до топиков! Это может привести к ошибкам или неожиданным результатам в работе правил! Так же учитывайте, программа работает с данными в строчном представлении. Если вы создадите поле с численным типом, то после перезаписи этого поля с помощью правила, поле будет иметь строчный тип.

## Пример использования

1. Запустите приложение и настройте `config.ini`.
2. Перейдите на вкладку "MQTT Client" и добавьте топики для подписки.
3. Используйте вкладку "Firebase Rules/Firestore Rules" для создания правил, связывающих Firebase и MQTT.
4. Создайте правило с помощью кнопки `Add Rule` и отредактируйте его парамтеры (Firebase/Firestore пути, направление данных, MQTT-топик).
5. Нажмите кнопку Start и попробуйте изменить отслеживаемое поле или опубликовать данные в топик.
6. Нажмите кнопку Stop, что бы остановить работу правил.

## Лицензия

MIT License.

## Донат

[Спасибо большое!❤️](https://boosty.to/naillin/donate)
