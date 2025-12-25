# TelegramBotExample — README

Кратко: этот проект — пример Telegram-бота на .NET. README объясняет, как создаются и подключаются обработчики (Handlers), и как работает система построения сообщений через `Director` и `TelegramMessageBuilder`.

**Быстрый старт**

- Сборка и запуск:

```bash
dotnet build
dotnet run --project TelegramBotExample
```

**Где смотреть код**

- Интерфейс обработчика: [TelegramBotExample/Handlers/IHandler.cs](TelegramBotExample/Handlers/IHandler.cs)
- Базовый класс обработчиков: [TelegramBotExample/Handlers/HandlerBase.cs](TelegramBotExample/Handlers/HandlerBase.cs)
- Пример конфигурации обработчиков: [TelegramBotExample/Program.cs](TelegramBotExample/Program.cs)
- Директор и строители сообщений: [TelegramBotExample/Telegram/Builder/Director.cs](TelegramBotExample/Telegram/Builder/Director.cs) и [TelegramBotExample/Telegram/Builder/TelegramMessageBuilder.cs](TelegramBotExample/Telegram/Builder/TelegramMessageBuilder.cs)

**Handlers — как создавать**

- Основные варианты:
  - Реализовать интерфейс `IHandler`.
  - Наследоваться от `HandlerBase` и переопределить `HandleUpdateAsync` (предпочтительный вариант).

- Обязательные моменты при реализации (при наследовании от `HandlerBase`):
  - Переопределите `protected override HandlerType Type { get; }` (если нужно маркировать тип для логики/мониторинга).
  - Реализуйте `public override async Task HandleUpdateAsync(UpdateModel update, CancellationToken cancellationToken)`.
  - Для передачи управления следующему обработчику вызовите `await NextHandlerAsync(update, cancellationToken);` в конце логики (если нужно передать дальше).

Пример простого обработчика:

```csharp
using System.Threading;
using System.Threading.Tasks;
using TelegramBotExample.Handlers;
using TelegramBotExample.Models;

public class MyHandler : HandlerBase
{
    protected override HandlerType Type => HandlerType.SomeType; // выберите подходящий

    public override async Task HandleUpdateAsync(UpdateModel update, CancellationToken cancellationToken)
    {
        // Ваша логика: проверка текста, callback и т.д.

        // при необходимости обработать и закончить — не вызывать NextHandlerAsync
        // чтобы передать обработку дальше:
        await NextHandlerAsync(update, cancellationToken);
    }
}
```

- Размещать новые обработчики удобно в папке `TelegramBotExample/Handlers/Impl`.

**Как подключаются обработчики (регистрация и порядок обработки)**

- В `Program.cs` есть функция `ConfigureHandlers`, которая создаёт экземпляры обработчиков и связывает их методом `SetNext`.
- Результатом `ConfigureHandlers` возвращается корневой (первый) обработчик, который регистрируется как `IHandler` в DI и затем используется в `BotHostedService`.

Ключевой момент: порядок установки через `SetNext` важен — от первого к следующему; обычно ставят `ExceptionHandler` первым для централизованной обработки ошибок.

Фрагмент из `Program.cs` (строго демонстративно):

```csharp
var exceptionHandler = new ExceptionHandler(telegramClient);
var startHandler = new StartHandler(telegramClient);
var menuHandler = new MenuHandler(telegramClient);

exceptionHandler
    .SetNext(startHandler)
    .SetNext(menuHandler);

return exceptionHandler; // корневой обработчик
```

**Director и TelegramMessageBuilder — как и зачем**

- `Director` ([TelegramBotExample/Telegram/Builder/Director.cs](TelegramBotExample/Telegram/Builder/Director.cs)) — утилитарный класс, который содержит высокоуровневые методы по созданию преднастроенных сообщений (меню, удаление клавиатуры, добавление "Назад" и т.п.).
- `TelegramMessageBuilder` ([TelegramBotExample/Telegram/Builder/TelegramMessageBuilder.cs](TelegramBotExample/Telegram/Builder/TelegramMessageBuilder.cs)) — класс "строитель", который пошагово формирует `TelegramMessageModel`.

Основные методы `TelegramMessageBuilder`:
- `Reset()` — сброс состояния строителя.
- `SetChatId(...)`, `SetMessageText(...)` — базовые сеттеры.
- `AddInlineButtonWithCallbackData(text, callbackData, onNewLine)` — добавить inline-кнопку.
- `AddKeyboardButton(...)` — добавить кнопку ответной клавиатуры.
- `SetReplyKeyboardRemove()` — пометить, что разметку клавиатуры нужно удалить.
- `SetDocument(...)`, `AddImage(...)` — для вложений.
- `GetResult()` — получить итоговую `TelegramMessageModel`.

Пример использования `Director` и `TelegramMessageBuilder`:

```csharp
var builder = new TelegramMessageBuilder();
var director = new Director(builder);

director.MakeMenuMessage(chatId);
var messageModel = builder.GetResult();

// messageModel теперь содержит готовую модель сообщения для отправки через ITelegramClient
```

Также у `TelegramMessageBuilder` есть утилиты для экранирования спецсимволов Markdown: `EscapeReservedSymbols` и `EscapeAllReservedSymbols`.

**Отправка сообщений**

- В проекте есть `ITelegramClient` (реализация в `TelegramBotExample/Telegram/Client/Impl/TelegramClient.cs`). Используйте его методы для отправки `TelegramMessageModel`, полученной от `TelegramMessageBuilder`.

**Где смотреть запуск бота**

- Основной цикл получения апдейтов реализован в `TelegramBotExample/Bots/Impl/BotHostedService.cs` — бот получает апдейты и передаёт их в зарегистрированный `IHandler`.
