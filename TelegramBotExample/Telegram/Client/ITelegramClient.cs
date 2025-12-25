using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotExample.Telegram.Models;

namespace TelegramBotExample.Telegram.Client;

/// <summary>
///     Клиент для работы с Telegram
/// </summary>
public interface ITelegramClient
{
    /// <summary>
    ///     Получить получатель обновлений в очереди
    /// </summary>
    /// <remarks>
    ///     Если получатель обновлений был уже создан, возвращает существующий экземпляр
    /// </remarks>
    QueuedUpdateReceiver GetUpdateReceiver(UpdateType[] allowedUpdates);

    /// <summary>
    ///     Удаляет сообщение из чата
    /// </summary>
    /// <param name="channelId"> Id канала </param>
    /// <param name="messageId"> Id сообщения, которое хотим удалить </param>
    /// <param name="cancellationToken"> Токен отмены </param>
    Task DeleteMessageAsync(long channelId, int messageId, CancellationToken cancellationToken);

    /// <summary>
    ///     Проверяет является ли пользователь администратором
    /// </summary>
    /// <param name="channelId"> Id канала </param>
    /// <param name="userId"> Id пользователя </param>
    /// <param name="cancellationToken"> Токен отмены </param>
    Task<bool> IsAdminAsync(long channelId, long userId, CancellationToken cancellationToken);

    /// <summary>
    ///     Отправляет блок сообщений за раз
    /// </summary>
    /// <param name="models"> Модели сообщений для отправки </param>
    Task SendManyMessagesAsync(IEnumerable<TelegramMessageModel> models, CancellationToken cancellationToken);

    /// <summary>
    ///     Отправляет сообщение в указанный чат
    /// </summary>
    Task<int> SendMessageAsync(TelegramMessageModel messageModel, CancellationToken cancellationToken);

    /// <summary>
    ///     Обновляет сообщение
    /// </summary>
    Task UpdateMessageWithInlineButtonAsync(
        int messageId,
        TelegramMessageModel messageModel,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Отправляет пользователю Telegram инвойс на оплату VIP-доступа или другого сервиса
    ///     с использованием внутренней валюты Telegram (звёзды).
    /// </summary>
    /// <param name="chatId">
    ///     Идентификатор чата Telegram, куда будет отправлен инвойс.
    /// </param>
    /// <param name="title">
    ///     Заголовок инвойса, который увидит пользователь (например, "VIP Доступ").
    /// </param>
    /// <param name="description">
    ///     Описание инвойса, поясняющее, за что производится оплата.
    /// </param>
    /// <param name="payload">
    ///     Уникальный идентификатор платежа. Используется для отслеживания оплаты и
    ///     связывания успешного платежа с конкретным пользователем или заказом.
    /// </param>
    /// <param name="amountStars">
    ///     Сумма к оплате в Telegram stars (звёздах). Указывается в целых единицах,
    ///     внутри метода можно преобразовать в минимальные единицы для API Telegram.
    /// </param>
    /// <param name="cancellationToken">
    ///     Токен отмены, позволяющий прервать асинхронную операцию при необходимости.
    /// </param>
    Task SendInvoiceAsync(
        long chatId,
        string title,
        string description,
        string payload,
        string provider,
        int amountStars,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Разрешить или отклонить оплату
    /// </summary>
    /// <remarks>
    ///     Телеграм отправляет апдейт типа PreCheckoutQuery, на который нам необходимо ответить в течение десяти секунд
    /// </remarks>
    Task AnswerPreCheckoutQueryAsync(string preCheckoutQueryId, string errorMessage = "", CancellationToken cancellationToken = default);

    /// <summary>
    ///     Выгрузить файл из телеграм
    /// </summary>
    Task<TGFile> GetFileAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Загрузить файл
    /// </summary>
    /// <param name="filePath">Путь к файлу на сервер ТГ</param>
    /// <param name="destination"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DownloadFile(string filePath, Stream destination, CancellationToken cancellationToken = default);
}
