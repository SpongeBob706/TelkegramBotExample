using System;
using System.IO;
using Telegram.Bot.Types;
using TelegramBotExample.Telegram.Models;
using MessageType=TelegramBotExample.Telegram.Models.MessageType;

namespace TelegramBotExample.Telegram.Builder;

/// <summary>
///     Строитель сообщений для отправки в телеграм
/// </summary>
public class TelegramMessageBuilder
{
    #region Fields

    public static readonly string[] AllReservedSymbols =
        { "*", "`", "(", ")", "[", "]", ".", "-", "!", "#", "_", "|", "{", "}", "~", "=", "+", ">" };

    // оставлены для Markdown разметки * ` ( ) [ ]
    private static readonly string[] SymbolsToReplace =
        { ".", "-", "!", "#", "_", "|", "{", "}", "~", "=", "+", ">" };

    private TelegramMessageModel _messageModel = new();

    #endregion

    #region Public methods

    /// <summary>
    ///     Сбрасывает настройки сообщения
    /// </summary>
    public TelegramMessageBuilder Reset()
    {
        _messageModel = new();

        return this;
    }

    /// <summary>
    ///     Установить id чата
    /// </summary>
    public TelegramMessageBuilder SetChatId(long chatId)
    {
        _messageModel.ChatId = chatId;

        return this;
    }

    /// <summary>
    ///     Установить id чата
    /// </summary>
    public TelegramMessageBuilder SetChatId(string chatId)
    {
        _messageModel.ChatId = long.Parse(chatId);

        return this;
    }

    /// <summary>
    ///     Установить документ
    /// </summary>
    public TelegramMessageBuilder SetDocument(string path, string name, string caption)
    {
        _messageModel.FilePath = path;
        _messageModel.FileName = name;

        _messageModel.FileCaption = EscapeReservedSymbols(caption);
        _messageModel.Type = MessageType.WithDocument;

        return this;
    }

    /// <summary>
    ///     Установить текст сообщения
    /// </summary>
    public TelegramMessageBuilder SetMessageText(string message)
    {
        _messageModel.Text = message;

        return this;
    }

    /// <summary>
    ///     Установить inline кнопку с ссылкой
    /// </summary>
    public TelegramMessageBuilder AddInlineButtonWithUrl(string buttonText, string url, bool onNewLine = false)
    {
        var inlineButton = InlineKeyboardButtonBase.WithUrl(buttonText, url);
        _messageModel.AddInlineKeyboardButtons(inlineButton, onNewLine);
        _messageModel.Type = MessageType.WithInlineButton;

        return this;
    }

    /// <summary>
    ///     Установить inline кнопку с обратным ответом
    /// </summary>
    public TelegramMessageBuilder AddInlineButtonWithCallbackData(string buttonText, string callbackData,
        bool onNewLine = false)
    {
        var inlineButton = InlineKeyboardButtonBase.WithCallbackData(buttonText, callbackData);
        _messageModel.AddInlineKeyboardButtons(inlineButton, onNewLine);
        _messageModel.Type = MessageType.WithInlineButton;

        return this;
    }

    /// <summary>
    ///     Установить кнопку ответа вместо клавиатуры
    /// </summary>
    /// <remarks>
    ///     Текст сообщения должен быть уже установлен перед использованием этого метода
    /// </remarks>
    public TelegramMessageBuilder AddKeyboardButton(string text, bool requestContact = false)
    {
        _messageModel.Type = MessageType.WithReplyKeyboardMarkup;

        var replyKeyboardModel = new ReplyKeyboardMarkupModel(text, requestContact);
        _messageModel.ReplyKeyboardMarkups.Add(replyKeyboardModel);

        return this;
    }

    /// <summary>
    ///     Установить кнопку ответа вместо клавиатуры с ссылкой на сайт
    /// </summary>
    /// <remarks>
    ///     Текст сообщения должен быть уже установлен перед использованием этого метода
    /// </remarks>
    public TelegramMessageBuilder AddKeyboardButton(string text, string url)
    {
        _messageModel.Type = MessageType.WithReplyKeyboardMarkup;

        var replyKeyboardModel = new ReplyKeyboardMarkupModel(text, url);
        _messageModel.ReplyKeyboardMarkups.Add(replyKeyboardModel);

        return this;
    }

    /// <summary>
    ///     Удалить разметку для клавиатуры
    /// </summary>
    public TelegramMessageBuilder SetReplyKeyboardRemove()
    {
        _messageModel.Type = MessageType.ReplyKeyboardRemove;

        return this;
    }

    /// <summary>
    ///     Добавить изображение
    /// </summary>
    /// <param name="pathToImage"> Путь к нужному изображению </param>
    public TelegramMessageBuilder AddImage(string pathToImage)
    {
        _messageModel.Type = MessageType.WithImage;

        var imageStream = File.OpenRead(pathToImage);

        var mediaPhoto =
            new InputMediaPhoto(InputFile.FromStream(imageStream, Path.GetFileNameWithoutExtension(pathToImage)));

        if (_messageModel.Images.Length == 0)
        {
            _messageModel.Images = new[] { mediaPhoto };
        }
        else
        {
            _messageModel.Images = [.._messageModel.Images, mediaPhoto];
        }

        return this;
    }

    /// <summary>
    ///     Возвращает софрмированную модель сообщения
    /// </summary>
    public TelegramMessageModel GetResult() => _messageModel;

    #endregion

    #region Private methods

    /// <summary>
    ///     Экранировать зарезервированные символы
    /// </summary>
    public static string EscapeReservedSymbols(string text)
    {
        foreach (var symbol in SymbolsToReplace)
        {
            text = text.Replace(symbol, $"\\{symbol}");
        }

        return text;
    }

    /// <summary>
    ///     Экранировать все символы
    /// </summary>
    public static string EscapeAllReservedSymbols(string text)
    {
        foreach (var symbol in AllReservedSymbols)
        {
            text = text.Replace(symbol, $"\\{symbol}");
        }

        return text;
    }

    internal TelegramMessageBuilder SetMessageText(object requestsText)
    {
        throw new NotImplementedException();
    }

    #endregion
}
