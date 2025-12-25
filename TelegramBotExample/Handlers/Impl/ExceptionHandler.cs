using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using TelegramBotExample.Models;
using TelegramBotExample.Telegram.Builder;
using TelegramBotExample.Telegram.Client;
using ILogger=NLog.ILogger;

namespace TelegramBotExample.Handlers.Impl;

/// <summary>
///     Обработчик ошибок
/// </summary>
public class ExceptionHandler : HandlerBase
{
    #region Fields

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly ITelegramClient _telegramClient;

    #endregion

    #region .ctor

    /// <inheritdoc cref="ExceptionHandler"/>
    public ExceptionHandler(ITelegramClient telegramClient)
    {
        _telegramClient = telegramClient;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override HandlerType Type => HandlerType.Exception;

    #endregion

    /// <inheritdoc />
    public override async Task HandleUpdateAsync(UpdateModel updateModel, CancellationToken cancellationToken)
    {
        try
        {
            await NextHandlerAsync(updateModel, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.Message);

            var messageBuilder = new TelegramMessageBuilder()
                .SetChatId(updateModel.ChatId)
                .SetMessageText("Что-то пошло не так. Обратитесь за помощью к @****");

            await _telegramClient.SendMessageAsync(messageBuilder.GetResult(), cancellationToken);
        }
    }
}
