using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotExample.Telegram.Models;

/// <summary>
///     Инлайн кнопка с ссылкой
/// </summary>
public class InlineKeyboardButtonWithUrl : InlineKeyboardButtonBase
{
    /// <summary>
    ///     Url, на который ведет кнопка
    /// </summary>
    public string Url { get; set; }

    public override InlineKeyboardButton GetInlineKeyboardButton()
        => InlineKeyboardButton.WithUrl(ButtonText, Url);
}
