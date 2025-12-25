using System.Threading;
using System.Threading.Tasks;
using TelegramBotExample.Models;
using TelegramBotExample.Telegram.Builder;
using TelegramBotExample.Telegram.Client;

namespace TelegramBotExample.Handlers.Impl;

/// <summary>
///     Обработчик старта
/// </summary>
public class StartHandler : HandlerBase
{
    #region Fields

    private readonly ITelegramClient _telegramClient;

    #endregion

    #region .ctor

    /// <inheritdoc cref="StartHandler"/>
    public StartHandler(ITelegramClient telegramClient)
    {
        _telegramClient = telegramClient;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override HandlerType Type => HandlerType.Start;

    #endregion

    /// <inheritdoc />
    public override async Task HandleUpdateAsync(UpdateModel updateModel, CancellationToken cancellationToken)
    {
        if (updateModel.Text == "/start")
        {
            var messageBuilder = new TelegramMessageBuilder();

            var director = new Director(messageBuilder);
            director.MakeMenuMessage(updateModel.ChatId);
            director.AddBackButton();

            await _telegramClient.SendMessageAsync(messageBuilder.GetResult(), cancellationToken);

            return;
        }

        await NextHandlerAsync(updateModel, cancellationToken);
    }
}
