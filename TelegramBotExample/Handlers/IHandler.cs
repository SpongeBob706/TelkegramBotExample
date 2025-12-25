using System.Threading;
using System.Threading.Tasks;
using TelegramBotExample.Models;

namespace TelegramBotExample.Handlers;

/// <summary>
///     Общий интерфейс обработчиков
/// </summary>
public interface IHandler
{
    /// <summary>
    ///     Обработать апдейт
    /// </summary>
    Task HandleUpdateAsync(UpdateModel update, CancellationToken cancellationToken);

    /// <summary>
    ///     Установить следующего обработчика
    /// </summary>
    IHandler SetNext(IHandler nextHandler);
}
