using System;
using System.Threading.Tasks;

namespace TelegramBotExample.Bots;

/// <summary>
///     Сервис для работы телеграм бота
/// </summary>
public interface IBotService : IDisposable
{
    /// <summary>
    ///     Запускает работу сервиса
    /// </summary>
    Task RunAsync();
}
