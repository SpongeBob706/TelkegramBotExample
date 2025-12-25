using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;

namespace TelegramBotExample.Models;

public class UpdateModel
{
    public UpdateModel()
    { }

    public UpdateModel(Update update)
    {
        Text = update.Message?.Text ?? update.CallbackQuery?.Data ?? update.Message?.Caption ?? string.Empty;
        ChatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id ?? 0;
        Phone = update.Message?.Contact?.PhoneNumber ?? string.Empty;
        From = update.Message?.From ?? update.CallbackQuery?.From;
        PreCheckoutQuery = update.PreCheckoutQuery;
        MessageType = update.Message?.Type;

        if (update.Message?.Photo != null && update.Message.Photo.Length > 0)
        {
            Photo = update.Message.Photo
                .OrderByDescending(p => p.FileSize) // берем наибольшую по размеру
                .FirstOrDefault();
        }
    }

    public User From { get; set; }

    public long UserId => From.Id;

    public long ChatId { get; set; }

    public string Text { get; set; } = string.Empty;

    public string Phone { get; set; }

    public PreCheckoutQuery PreCheckoutQuery { get; set; }

    public MessageType? MessageType { get; set; }

    public PhotoSize? Photo { get; }
}
