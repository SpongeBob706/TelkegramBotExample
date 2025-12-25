using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TelegramBotExample.Models;

namespace TelegramBotExample.Handlers.Impl;

/// <summary>
///     Проверяет доступ к боту по id пользователя на основе файла whitelist.txt
///     Реализовано кэширование с автообновлением, чтобы не перегружать диск
/// </summary>
public class WhiteListHandler : HandlerBase
{
    private readonly string _whiteListFilePath;
    private readonly TimeSpan _cacheTtl = TimeSpan.FromSeconds(10); // обновляем кэш раз в 10 секунд
    private DateTime _lastReadTime = DateTime.MinValue;

    private readonly object _lock = new();
    private HashSet<string> _allowedUserIds = new();

    public WhiteListHandler(IConfiguration configuration)
    {
        var dataFolder = configuration.GetValue(Keys.DATA_STORAGE_FOLDER, AppContext.BaseDirectory);

        _whiteListFilePath = Path.Combine(dataFolder,  Consts.WHITE_LIST_FILE_NAME);
    }

    #region Properties

    /// <inheritdoc />
    protected override HandlerType Type => HandlerType.WhiteList;

    #endregion

    public override async Task HandleUpdateAsync(UpdateModel updateModel, CancellationToken cancellationToken)
    {
        var userName = updateModel.From.Username;

        if (!string.IsNullOrEmpty(userName) && IsUserAllowed(userName))
        {
            await NextHandlerAsync(updateModel, cancellationToken);
        }

        // Если не в белом списке — можно ничего не отвечать, просто не пропускать дальше
    }

    private bool IsUserAllowed(string userName)
    {
        EnsureWhitelistLoaded();

        return _allowedUserIds.Contains(userName);
    }

    private void EnsureWhitelistLoaded()
    {
        var now = DateTime.UtcNow;

        // Быстрая проверка без блокировок
        if (now - _lastReadTime < _cacheTtl)
        {
            return;
        }

        // Если TTL истёк - обновляем кэш
        lock (_lock)
        {
            if (now - _lastReadTime < _cacheTtl)
            {
                return;
            }

            try
            {
                if (!File.Exists(_whiteListFilePath))
                {
                    _allowedUserIds = new HashSet<string>();
                    _lastReadTime = now;

                    return;
                }

                var lines = File.ReadAllLines(_whiteListFilePath)
                    .Select(l => l.Trim())
                    .ToHashSet();

                _allowedUserIds = lines;
                _lastReadTime = now;
            }
            catch (IOException)
            {
                // если файл занят — просто пропускаем обновление, оставляем старый кэш
            }
        }
    }
}
