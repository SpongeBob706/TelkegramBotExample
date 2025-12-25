using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;

namespace TelegramBotExample.Telegram.Models;

/// <summary>
///     Модель сообщения для телеграмма
/// </summary>
public class TelegramMessageModel
{
    /// <summary>
    ///     Тип сообщения
    /// </summary>
    public MessageType Type { get; set; }

    /// <summary>
    ///     Id чата для отправки
    /// </summary>
    public long ChatId { get; set; }

    /// <summary>
    ///     Текст сообщения
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    ///     Путь к документу
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    ///     Название документа
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    ///     Описание документа
    /// </summary>
    public string FileCaption { get; set; }

    /// <summary>
    ///     Изображение
    /// </summary>
    public InputMediaPhoto[] Images { get; set; } = Array.Empty<InputMediaPhoto>();

    /// <summary>
    ///     Разметка клавиатуры для ответа
    /// </summary>
    public List<ReplyKeyboardMarkupModel> ReplyKeyboardMarkups { get; set; } = new();

    /// <summary>
    ///     Кнопки внизу сообщения
    /// </summary>
    public List<List<InlineKeyboardButtonBase>> InlineKeyboardButtons { get; set; } = new() { };

    internal void AddInlineKeyboardButtons(InlineKeyboardButtonBase inlineButton, bool onNewLine)
    {
        if (!InlineKeyboardButtons.Any())
        {
            InlineKeyboardButtons.Add(new List<InlineKeyboardButtonBase> { inlineButton });

            return;
        }

        if (onNewLine)
        {
            InlineKeyboardButtons.Add(new List<InlineKeyboardButtonBase> { inlineButton });
        }
        else
        {
            var lastList = InlineKeyboardButtons.LastOrDefault();
            lastList.Add(inlineButton);
        }
    }
}
