using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotExample.Telegram.Models;
using MessageType=TelegramBotExample.Telegram.Models.MessageType;

namespace TelegramBotExample.Telegram.Client.Impl;

/// <inheritdoc cref="ITelegramClient"/>
public class TelegramClient : ITelegramClient
{
    #region Fields

    /// <summary>
    ///     Кол-во сообщений перед задержкой в группе
    /// </summary>
    private const int MessagesInGroup = 4;

    private const int TelegramTextLimit = 4096;

    // специально, чтобы не экранировать запрещенные символы
    private const ParseMode ParseMode = global::Telegram.Bot.Types.Enums.ParseMode.None;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ITelegramBotClient _client;

    // TODO: Obsolete
    private QueuedUpdateReceiver _updateReceiver = null!;

    #endregion

    #region .ctor

    /// <inheritdoc cref="ITelegramClient"/>
    public TelegramClient(string token)
    {
        _client = new TelegramBotClient(token);
    }

    #endregion

    #region Public methods

    /// <inheritdoc />
    public QueuedUpdateReceiver GetUpdateReceiver(UpdateType[] allowedUpdates)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = allowedUpdates
        };

        _updateReceiver = new QueuedUpdateReceiver(_client, receiverOptions);

        return _updateReceiver;
    }

    /// <inheritdoc />
    public async Task DeleteMessageAsync(long channelId, int messageId, CancellationToken cancellationToken) =>
        await _client.DeleteMessage(channelId, messageId, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> IsAdminAsync(long channelId, long userId, CancellationToken cancellationToken)
    {
        var admins = await _client.GetChatAdministrators(channelId, cancellationToken);

        return admins.Select(_ => _.User.Id).Contains(userId);
    }

    /// <inheritdoc />
    public async Task SendManyMessagesAsync(IEnumerable<TelegramMessageModel> models,
        CancellationToken cancellationToken)
    {
        var pagesCount = (int)Math.Ceiling(models.Count() / (double)MessagesInGroup);
        for (var page = 0; page < pagesCount; page++)
        {
            var modelsToSend = models
                .Skip(MessagesInGroup * page)
                .Take(MessagesInGroup)
                .ToArray();

            for (var i = 0; i < modelsToSend.Length; i++)
            {
                try
                {
                    await SendMessageAsync(modelsToSend[i], cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Failed to send message in group: {ex.Message}");
                }
            }

            // принудительная задержка чтобы телеграм не отбивал запросы
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task SendInvoiceAsync(
        long chatId,
        string title,
        string description,
        string payload,
        string provider,
        int amountStars,
        CancellationToken cancellationToken)
    {
        var prices = new[]
        {
            new LabeledPrice("VIP доступ на 30 дней", amountStars * 100) // сумма в минимальных единицах
        };

        await _client.SendInvoice(
            chatId: chatId,
            title: title,
            description: description,
            payload: payload,
            providerToken: provider,
            currency: "RUB",
            prices: prices,
            isFlexible: false,
            cancellationToken: cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task AnswerPreCheckoutQueryAsync(string preCheckoutQueryId, string errorMessage = "",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(errorMessage))
        {
            await _client.AnswerPreCheckoutQuery(preCheckoutQueryId, cancellationToken: cancellationToken);
        }
        else
        {
            await _client.AnswerPreCheckoutQuery(preCheckoutQueryId, errorMessage, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<TGFile> GetFileAsync(string fileId, CancellationToken cancellationToken = default)
    {
        return await _client.GetFile(fileId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DownloadFile(string filePath, Stream destination, CancellationToken cancellationToken = default)
    {
        await _client.DownloadFile(filePath, destination, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> SendMessageAsync(TelegramMessageModel messageModel, CancellationToken cancellationToken)
    {
        Logger.Trace($"Start sending message to {messageModel.ChatId}");
        var id = 0;

        try
        {
            switch (messageModel.Type)
            {
                case MessageType.Default:
                    id = (await SendTextMessageAsync(messageModel, cancellationToken)).MessageId;
                    break;
                case MessageType.WithInlineButton:
                    id = (await SendMessageWithInlineButtonAsync(messageModel, cancellationToken)).MessageId;
                    break;
                case MessageType.WithImage:
                    await SendMessageWithImageAsync(messageModel, cancellationToken);
                    break;
                case MessageType.WithReplyKeyboardMarkup:
                    id = (await SendMessageWithReplyKeyboardMarkupAsync(messageModel, cancellationToken)).MessageId;
                    break;
                case MessageType.ReplyKeyboardRemove:
                    await SendMessageWithReplyKeyboardRemoveAsync(messageModel, cancellationToken);
                    break;
                case MessageType.WithDocument:
                    id = (await SendMessageWithDocumentAsync(messageModel, cancellationToken)).MessageId;
                    break;
                default:
                    throw new NotImplementedException($"{nameof(messageModel.Type)} = {messageModel.Type}");
            }
        }
        catch (Exception ex)
        {
            Logger.Trace($"Failed to send message to {messageModel.ChatId}");
            Logger.Error(ex.ToString());

            throw;
        }

        Logger.Trace($"Successful send message to {messageModel.ChatId}");

        return id;
    }

    /// <inheritdoc />
    public async Task UpdateMessageWithInlineButtonAsync(
        int messageId,
        TelegramMessageModel messageModel,
        CancellationToken cancellationToken)
    {
        var inlineButtons = GetInlineButtons(messageModel);
        await _client.EditMessageText(
            messageModel.ChatId,
            messageId,
            text: messageModel.Text,
            parseMode: ParseMode,
            replyMarkup: new InlineKeyboardMarkup(inlineButtons),
            cancellationToken: cancellationToken);
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Отправить обычное текстовое сообщение
    /// </summary>
    private async Task<Message> SendTextMessageAsync(TelegramMessageModel messageModel,
        CancellationToken cancellationToken)
    {
        var messages = SplitMessage(messageModel.Text).ToArray();

        Message? lastMessage = null;

        foreach (var part in messages)
        {
            lastMessage = await _client.SendMessage(
                messageModel.ChatId,
                part,
                parseMode: ParseMode,
                cancellationToken: cancellationToken);

            // маленькая задержка чтобы телеграм не троттлил
            await Task.Delay(100, cancellationToken);
        }

        return lastMessage!;
    }

    /// <summary>
    ///     Отправить сообщение с встроенной кнопкой
    /// </summary>
    private async Task<Message> SendMessageWithInlineButtonAsync(TelegramMessageModel messageModel,
        CancellationToken cancellationToken)
    {
        var inlineButtons = GetInlineButtons(messageModel);

        var message = await _client.SendMessage(
            messageModel.ChatId,
            messageModel.Text,
            parseMode: ParseMode,
            replyMarkup: new InlineKeyboardMarkup(inlineButtons),
            cancellationToken: cancellationToken);

        return message;
    }

    /// <summary>
    ///     Сконфигурировать и получить inline кнопки
    /// </summary>
    private static List<List<InlineKeyboardButton>> GetInlineButtons(TelegramMessageModel messageModel)
    {
        var inlineButtons = new List<List<InlineKeyboardButton>>();
        foreach (var row in messageModel.InlineKeyboardButtons)
        {
            var list = new List<InlineKeyboardButton>();
            foreach (var item in row)
            {
                list.Add(item.GetInlineKeyboardButton());
            }

            inlineButtons.Add(list);
        }

        return inlineButtons;
    }

    /// <summary>
    ///     Отправить сообщение с изображением
    /// </summary>
    private async Task SendMessageWithImageAsync(TelegramMessageModel messageModel, CancellationToken cancellationToken)
    {
        var image = messageModel.Images.FirstOrDefault();
        if (image is not null)
        {
            image.Caption = messageModel.Text;
            image.ParseMode = ParseMode;
        }

        await _client.SendMediaGroup(
            messageModel.ChatId,
            messageModel.Images,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    ///     Отправить сообщение с удалением разметки для клавиатуры
    /// </summary>
    private async Task SendMessageWithReplyKeyboardRemoveAsync(TelegramMessageModel messageModel,
        CancellationToken cancellationToken)
    {
        await _client.SendMessage(
            messageModel.ChatId,
            messageModel.Text,
            parseMode: ParseMode,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    ///     Отправить сообщение с документом
    /// </summary>
    private async Task<Message> SendMessageWithDocumentAsync(TelegramMessageModel messageModel,
        CancellationToken cancellationToken)
    {
        await using Stream stream = File.OpenRead(messageModel.FilePath);

        var message = await _client.SendDocument(
            messageModel.ChatId,
            document: InputFile.FromStream(stream, fileName: messageModel.FileName),
            caption: messageModel.FileCaption,
            parseMode: ParseMode,
            cancellationToken: cancellationToken);

        return message;
    }

    /// <summary>
    ///     Отправить сообщение с клавиатурой в виде кнопок
    /// </summary>
    private async Task<Message> SendMessageWithReplyKeyboardMarkupAsync(TelegramMessageModel messageModel,
        CancellationToken cancellationToken)
    {
        var keyboardButtons = messageModel.ReplyKeyboardMarkups
            .Select(_ => new[] { _.GetKeyboardButton() })
            .ToArray();

        var message = await _client.SendMessage(
            messageModel.ChatId,
            messageModel.Text,
            parseMode: ParseMode,
            replyMarkup: new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            },
            cancellationToken: cancellationToken);

        return message;
    }

    /// <summary>
    /// Разбивает большой текст на части длиной не более TelegramTextLimit,
    /// стараясь резать по границам слов.
    /// </summary>
    private static IEnumerable<string> SplitMessage(string text)
    {
        if (string.IsNullOrEmpty(text))
            yield break;

        text = text.Replace("\r", ""); // унификация

        while (text.Length > TelegramTextLimit)
        {
            // ищем ближайший перенос по слову
            var splitIndex = text.LastIndexOf(' ', TelegramTextLimit);

            // если пробелов нет — режем жестко
            if (splitIndex <= 0)
                splitIndex = TelegramTextLimit;

            var part = text[..splitIndex].Trim();
            yield return part;

            text = text[splitIndex..].Trim();
        }

        if (text.Length > 0)
            yield return text;
    }

    #endregion
}
