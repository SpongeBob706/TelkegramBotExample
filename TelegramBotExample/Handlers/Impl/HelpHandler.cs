using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TelegramBotExample.Models;
using TelegramBotExample.Telegram.Builder;
using TelegramBotExample.Telegram.Client;

namespace TelegramBotExample.Handlers.Impl;

/// <summary>
///     Обработчик команды /help
/// </summary>
public class HelpHandler : HandlerBase
{
    private readonly long _adminId;
    private readonly ITelegramClient _telegramClient;

    /// <inheritdoc cref="HelpHandler"/>
    public HelpHandler(ITelegramClient telegramClient, IConfiguration configuration)
    {
        _telegramClient = telegramClient;
        _adminId = configuration.GetValue<long>(Keys.ADMIN, Consts.ADMIN_ID);
    }

    #region Properties

    /// <inheritdoc />
    protected override HandlerType Type => HandlerType.Help;

    #endregion

    public override async Task HandleUpdateAsync(UpdateModel updateModel, CancellationToken cancellationToken)
    {
        if (updateModel.Text != "/help")
        {
            await NextHandlerAsync(updateModel, cancellationToken);
            return;
        }

        var messageBuilder = new TelegramMessageBuilder()
            .SetChatId(updateModel.ChatId);

        var helpText =
            "ℹ️ Доступные команды:\n\n" +
            "/start – начать работу с ботом\n" +
            "/menu – список команд бота\n" +
            "/catalog – каталог новостроек\n" +
            "/equity – акции застройщиков\n" +
            "/bob – спросить Боба\n" +
            "премиум – квартиры и дома в продаже в статусе премиум\n" +
            "сториз – создать историю\n" +
            "/help - помощь";

        if (updateModel.ChatId == _adminId)
        {
            helpText += $"\nadd <ник пользователя> - добавить пользователя в белый список бота\n"
                + $"remove <ник пользователя> - удалить пользователя из бота\n"
                + $"list - получить текущий список разрешенных пользователей\n"
                + $"мониторинг - получить информацию о мониторинге запросов пользователей\n"
                + $"update-docs - запустить команду на полное обновление файлов в хранилище";
        }

        var message = messageBuilder
            .SetMessageText(helpText)
            .GetResult();

        await _telegramClient.SendMessageAsync(message, cancellationToken);
    }
}
