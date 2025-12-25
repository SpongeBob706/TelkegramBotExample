using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NLog;
using Telegram.Bot.Types.Enums;
using TelegramBotExample.Handlers;
using TelegramBotExample.Models;
using TelegramBotExample.Telegram.Client;
using ILogger = NLog.ILogger;

namespace TelegramBotExample.Bots.Impl;

internal class BotHostedService : BackgroundService
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly ITelegramClient _telegramClient;
    private readonly IHandler _handler;

    public BotHostedService(ITelegramClient telegramClient, IHandler handler)
    {
        _telegramClient = telegramClient;
        _handler = handler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var updateReceiver = _telegramClient.GetUpdateReceiver(
                    new[]
                    {
                        UpdateType.Message,
                        UpdateType.CallbackQuery,
                        UpdateType.ShippingQuery,
                        UpdateType.PreCheckoutQuery
                    });

                Logger.Info("Бот запущен!");

                await foreach (var update in updateReceiver.WithCancellation(stoppingToken))
                {
                    try
                    {
                        var updateModel = new UpdateModel(update);
                        await _handler.HandleUpdateAsync(updateModel, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, ex.Message);
                    }
                }

                Logger.Warn("Получение обновлений остановилось! Перезапускаю цикл...");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка в основном цикле получения обновлений");
            }

            // задержка перед повторной попыткой, чтобы не крутить цикл бешено
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.Info("Остановка бота...");
        await base.StopAsync(cancellationToken);
    }
}
