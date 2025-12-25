using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotExample.Telegram.Models;

/// <summary>
///     Модель кнопки внизу сообщения
/// </summary>
public abstract class InlineKeyboardButtonBase
{
    #region Fields

    /// <summary>
    ///     Макимальная длина текста и callbackData инлайн кнопки
    /// </summary>
    public const int MaxInlineButtonTextLength = 64;

    #endregion

    #region .ctors

    /// <summary>
    ///     Создать модель кнопки внизу сообщения для перехода по ссылке
    /// </summary>
    /// <param name="text"> Текст кнопки </param>
    /// <param name="url"> Url, на который ведет кнопка </param>
    public static InlineKeyboardButtonBase WithUrl(string text, string url)
    {
        return new InlineKeyboardButtonWithUrl()
        {
            ButtonText = text,
            Url = url,
        };
    }

    /// <summary>
    ///     Создать модель кнопки внизу сообщения с текстом возврата
    /// </summary>
    /// <param name="text"> Текст кнопки </param>
    /// <param name="callbackText"> Возвращаемый боту текст </param>
    public static InlineKeyboardButtonBase WithCallbackData(string text, string callbackText)
    {
        return new InlineKeyboardButtonWithCallback()
        {
            ButtonText = text,
            CallbackText = callbackText
        };
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Текст кнопки
    /// </summary>
    public string ButtonText { get; set; }

    #endregion

    #region Public methods

    public abstract InlineKeyboardButton GetInlineKeyboardButton();

    #endregion
}
