namespace TelegramBotExample.Telegram.Builder;

/// <summary>
///     Директор над строителями
/// </summary>
public class Director
{
    private TelegramMessageBuilder _builder;

    /// <inheritdoc cref="Director"/>
    public Director(TelegramMessageBuilder builder)
    {
        _builder = builder;
    }

    /// <summary>
    ///     Сменить строителя
    /// </summary>
    public void ChangeBuilder(TelegramMessageBuilder builder)
    {
        _builder = builder;
    }

    /// <summary>
    ///     Создать сообщение с удалением разметки клавиатуры
    /// </summary>
    public void MakeKeyboardRemovalMessage(long chatId, string text)
    {
        _builder
            .Reset()
            .SetChatId(chatId)
            .SetMessageText(text)
            .SetReplyKeyboardRemove();
    }

    /// <summary>
    ///     Создать сообщение с информацией о меню бота
    /// </summary>
    public void MakeMenuMessage(long chatId)
    {
        _builder
            .Reset()
            .SetChatId(chatId)
            .SetMessageText("Выберите действие:")
            .AddInlineButtonWithCallbackData("Каталог новостроек", "/catalog", true)
            .AddInlineButtonWithCallbackData("Акции застройщиков", "/equity", true)
            .AddInlineButtonWithCallbackData("Спросить Боба", "/bob", true)
            .AddInlineButtonWithCallbackData("Сториз", "стори", true);
    }

    /// <summary>
    ///     Добавить инлайн кнопку с возвратом назад
    /// </summary>
    public void AddBackButton()
    {
        _builder.AddInlineButtonWithCallbackData("Отмена", "/menu", true);
    }
}
