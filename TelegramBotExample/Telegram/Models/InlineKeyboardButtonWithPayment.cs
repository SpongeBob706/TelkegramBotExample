using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotExample.Telegram.Models;

/// <summary>
///     Инлайн кнопка с оплатой
/// </summary>
public class InlineKeyboardButtonWithPayment : InlineKeyboardButtonBase
{
    /// <summary>
    ///     Название продукта
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Описание продукта
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Описание продукта
    /// </summary>
    public string Payload { get; set; }

    /// <summary>
    ///     Токен платежной системы
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    ///     Массив кратких названий услуг/товаров и цен за них в одном платеже
    /// </summary>
    public LabeledPrice[] LabeledPrices { get; set; }

    public override InlineKeyboardButton GetInlineKeyboardButton()
        => InlineKeyboardButton.WithPay(ButtonText);
}
