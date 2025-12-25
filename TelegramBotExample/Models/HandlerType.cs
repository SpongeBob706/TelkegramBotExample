using System.ComponentModel.DataAnnotations;

namespace TelegramBotExample.Models;

/// <summary>
///     Тип запроса - определяет к какому обработчику относится данный запрос
/// </summary>
public enum HandlerType
{
    /// <summary>
    ///     Тип запроса – Добавление пользователя в белый список.
    /// </summary>
    [Display(Name = "Добавить пользователя в белый список")]
    AddUserToWhiteList,

    /// <summary>
    ///     Тип запроса – Задать вопрос.
    /// </summary>
    [Display(Name = "Задать вопрос")]
    AskQuestion,

    /// <summary>
    ///     Тип запроса – BOB (бот-помощник).
    /// </summary>
    [Display(Name = "Вызвать BOB")]
    Bob,

    /// <summary>
    ///     Тип запроса – Каталог проектов.
    /// </summary>
    [Display(Name = "Каталог проектов")]
    Catalog,

    /// <summary>
    ///     Тип запроса – Equity / доходность.
    /// </summary>
    [Display(Name = "Доходность / Equity")]
    Equity,

    /// <summary>
    ///     Тип запроса – Обработка исключений.
    /// </summary>
    [Display(Name = "Обработка исключений")]
    Exception,

    /// <summary>
    ///     Тип запроса – Генерация сторис.
    /// </summary>
    [Display(Name = "Генерация сторис")]
    GenerateStories,

    /// <summary>
    ///     Тип запроса – Помощь / справка.
    /// </summary>
    [Display(Name = "Помощь / справка")]
    Help,

    /// <summary>
    ///     Тип запроса – Последние премиум-листинги.
    /// </summary>
    [Display(Name = "Последние премиум-листинги")]
    LastPremiumListings,

    /// <summary>
    ///     Тип запроса – Просмотр белого списка.
    /// </summary>
    [Display(Name = "Просмотр белого списка")]
    ListWhiteList,

    /// <summary>
    ///     Тип запроса – Главное меню.
    /// </summary>
    [Display(Name = "Главное меню")]
    Menu,

    /// <summary>
    ///     Тип запроса – Мониторинг
    /// </summary>
    [Display(Name = "Мониторинг")]
    Monitoring,

    /// <summary>
    ///     Тип запроса – Детали проекта.
    /// </summary>
    [Display(Name = "Детали проекта")]
    ProjectDetail,

    /// <summary>
    ///     Тип запроса – Возврашает ссылку на презентацию проекта
    /// </summary>
    [Display(Name = "Ссылка на презентацию проекта")]
    ProjectPresentation,

    /// <summary>
    ///     Тип запроса – Возвращает описание проекта
    /// </summary>
    [Display(Name = "Описание проекта")]
    ProjectDescription,

    /// <summary>
    ///     Тип запроса – Возвращает фото проекта
    /// </summary>
    [Display(Name = "Фото проекта")]
    ProjectPhotos,

    /// <summary>
    ///     Тип запроса – Возвращает КВ и срок фиксации проекта
    /// </summary>
    [Display(Name = "КВ и срок фиксации проекта")]
    ProjectFixationTerms,

    /// <summary>
    ///     Тип запроса – Возвращает шахматку проекта
    /// </summary>
    [Display(Name = "Шахматка проекта")]
    ProjectPlan,

    /// <summary>
    ///     Тип запроса – Возвращает контакты отдела продаж проекта
    /// </summary>
    [Display(Name = "Контакты отдела продаж проекта")]
    ProjectContacts,

    /// <summary>
    ///     Тип запроса – Возвращает регламент проекта
    /// </summary>
    [Display(Name = "Регламент проекта")]
    ProjectRegulations,

    /// <summary>
    ///     Тип запроса – Возвращает документы проекта (счет, акт, отчет)
    /// </summary>
    [Display(Name = "Документы проекта")]
    ProjectDocuments,

    /// <summary>
    ///     Тип запроса – Возвращает текущие акции проекта
    /// </summary>
    [Display(Name = "Текущие акции проекта")]
    ProjectPromotion,

    /// <summary>
    ///     Тип запроса – Удаление пользователя из белого списка.
    /// </summary>
    [Display(Name = "Удалить пользователя из белого списка")]
    RemoveUserToWhiteList,

    /// <summary>
    ///     Тип запроса – Старт команд.
    /// </summary>
    [Display(Name = "Старт команд")]
    Start,

    /// <summary>
    ///     Тип запроса – Обновление файлов в store (не используется, но оставлен для структуры).
    /// </summary>
    [Display(Name = "Обновление файлов в хранилище")]
    UpdateFilesInStore,

    /// <summary>
    ///     Тип запроса – Проверка наличия пользователя в white list.
    /// </summary>
    [Display(Name = "Проверка пользователя в белом списке")]
    WhiteList,

    /// <summary>
    ///     Генерация сторис с рамкой
    /// </summary>
    [Display(Name = "Генерация сторис")]
    FrameStories
}
