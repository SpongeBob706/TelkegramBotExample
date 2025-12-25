using System.Threading;
using System.Threading.Tasks;
using TelegramBotExample.Models;

namespace TelegramBotExample.Handlers;

/// <summary>
///     Базовый класс обработчиков апдейта
/// </summary>
public abstract class HandlerBase : IHandler
{
    private IHandler? _nextHandler;

    /// <summary>
    ///     Тип обработчика для мониторинга
    /// </summary>
    protected abstract HandlerType Type { get; }

    #region Implementation of IHandler

    /// <summary>
    ///     Обработать апдейт
    /// </summary>
    public abstract Task HandleUpdateAsync(UpdateModel _, CancellationToken __);

    /// <summary>
    ///     Установить следующего обработчика
    /// </summary>
    public virtual IHandler SetNext(IHandler nextHandler)
        => _nextHandler = nextHandler;

    #endregion

    #region Protected methods

    /// <summary>
    ///     Передать обработку обновления следующему обработчику (если он есть)
    /// </summary>
    protected virtual async Task NextHandlerAsync(UpdateModel update, CancellationToken cancellationToken)
    {
        if (_nextHandler is not null)
        {
            await _nextHandler.HandleUpdateAsync(update, cancellationToken);
        }
    }

    #endregion
}
