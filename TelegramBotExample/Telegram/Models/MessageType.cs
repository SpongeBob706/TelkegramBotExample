namespace TelegramBotExample.Telegram.Models;

/// <summary>
///     Тип сообщения
/// </summary>
public enum MessageType
{
    /// <summary>
    ///     Простое текстовое сообщение
    /// </summary>
    Default,

    /// <summary>
    ///     Сообщение с инлайн-кнопкой
    /// </summary>
    WithInlineButton,

    /// <summary>
    ///     Сообщение с оплатой
    /// </summary>
    WithPayment,

    /// <summary>
    ///     Сообщение с разметкой для клавиатуры
    /// </summary>
    WithReplyKeyboardMarkup,

    /// <summary>
    ///     Сообщение с удалением разметки для клавиатуры
    /// </summary>
    ReplyKeyboardRemove,

    /// <summary>
    ///     С картинкой
    /// </summary>
    WithImage,

    /// <summary>
    ///     С документом
    /// </summary>
    WithDocument,
}
