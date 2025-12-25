using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotExample.Telegram.Models;

/// <summary>
///     Инлайн кнопка с ответом
/// </summary>
public class InlineKeyboardButtonWithCallback : InlineKeyboardButtonBase
{
    /// <summary>
    ///     Возвращаемый боту текст
    /// </summary>
    public string CallbackText { get; set; }

    public override InlineKeyboardButton GetInlineKeyboardButton()
        => InlineKeyboardButton.WithCallbackData(ButtonText, CallbackText);
}
