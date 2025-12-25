using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotExample.Telegram.Models;

/// <summary>
///     Кнопка ответа вместо клавиатуры 
/// </summary>
public class ReplyKeyboardMarkupModel
{
    /// <inheritdoc cref="ReplyKeyboardMarkupModel"/>
    public ReplyKeyboardMarkupModel(string text, bool requestContact)
    {
        Text = text;
        RequestContact = requestContact;
    }

    /// <inheritdoc cref="ReplyKeyboardMarkupModel"/>
    public ReplyKeyboardMarkupModel(string text, string url)
    {
        Text = text;
        Url = url;
        RequestContact = false;
    }

    public string Text { get; set; }

    public string Url { get; set; }

    public bool RequestContact { get; set; }

    public KeyboardButton GetKeyboardButton()
    {
        return RequestContact
            ? KeyboardButton.WithRequestContact(Text)
            : string.IsNullOrEmpty(Url)
                ? new KeyboardButton(Text)
                : KeyboardButton.WithWebApp(Text, new() { Url = Url });
    }
}
