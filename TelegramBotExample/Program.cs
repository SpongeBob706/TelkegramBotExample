using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using TelegramBotExample;
using TelegramBotExample.Bots.Impl;
using TelegramBotExample.Handlers;
using TelegramBotExample.Handlers.Impl;
using TelegramBotExample.Telegram.Client;
using TelegramBotExample.Telegram.Client.Impl;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
Console.OutputEncoding = Encoding.UTF8;

logger.Info("Инициализация");

try
{
    var builder = WebApplication.CreateBuilder(args);

    var folder = builder.Configuration.GetValue<string>(Keys.DATA_STORAGE_FOLDER);
    logger.Info($"Папка для хранения файлов {folder}");

    builder.Services.AddSingleton<ITelegramClient>(_ =>
    {
        var token = builder.Configuration.GetValue<string>(Keys.TOKEN);

        return new TelegramClient(token!);
    });

    builder.Services.AddSingleton<IHandler>(serviceProvider =>
    {
        var telegramClient = serviceProvider.GetRequiredService<ITelegramClient>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        // конфигурирую обработчики
        var handlers = ConfigureHandlers(telegramClient, configuration);

        return handlers;
    });

    // Регистрируем бота как HostedService
    builder.Services.AddHostedService<BotHostedService>();

    var app = builder.Build();
    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Fatal(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}


static IHandler ConfigureHandlers(ITelegramClient telegramClient, IConfiguration configuration)
{
    var exceptionHandler = new ExceptionHandler(telegramClient);
    var helpHandler = new HelpHandler(telegramClient, configuration);
    var startHandler = new StartHandler(telegramClient);
    var menuHandler = new MenuHandler(telegramClient);

    // всегда первым, для отлова ошибок и корректной отправки их пользователю
    // нужно учитывать порядок добавления обработчиков
    exceptionHandler
        .SetNext(startHandler)
        .SetNext(menuHandler)
        .SetNext(helpHandler);

    return exceptionHandler;
}
