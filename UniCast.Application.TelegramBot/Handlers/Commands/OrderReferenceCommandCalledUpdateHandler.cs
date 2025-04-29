using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Moodle;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.Result;
using UniCast.Application.TelegramBot.Scenarios.RefreshToken;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Handlers.Commands;

public sealed class OrderReferenceCommandCalledUpdateHandler : IUpdateHandler
{
    private readonly IMoodleClient _moodleClient;
    private readonly IDataContext _dataContext;
    private readonly RefreshTokenScenarioExecutor _refreshTokenScenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly ILogger<OrderReferenceCommandCalledUpdateHandler> _logger;

    public OrderReferenceCommandCalledUpdateHandler(
        IMoodleClient moodleClient,
        IDataContext dataContext,
        RefreshTokenScenarioExecutor refreshTokenScenarioExecutor,
        ITelegramMessageManager telegramMessageManager,
        ILogger<OrderReferenceCommandCalledUpdateHandler> logger)
    {
        _moodleClient = moodleClient;
        _dataContext = dataContext;
        _refreshTokenScenarioExecutor = refreshTokenScenarioExecutor;
        _telegramMessageManager = telegramMessageManager;
        _logger = logger;
    }

    public ValueTask<bool> CanHandleAsync(Update update, CancellationToken ct = default)
        => ValueTask.FromResult(
            update.Type is UpdateType.Message &&
            update.Message!.Text is not null &&
            update.Message.Text.StartsWith("/order_reference"));

    public async Task HandleAsync(Update update, CancellationToken ct = default)
    {
        var student = await GetStudentByChatExtIdAsync(update.Message!.Chat.Id, ct);
        if (student is null)
        {
            _logger.LogError("Order reference for unexisting student with chat ID {ChatID}",
                update.Message!.Chat.Id);
            return;
        }

        var orderResult = await _moodleClient.OrderReferenceForStudentAsync(student, ct);
        if (orderResult.IsFailure)
        {
            if (orderResult.Error.Group is ErrorGroup.AccessError)
            {
                await _refreshTokenScenarioExecutor.StartScenarioAsync(
                    chat: await GetTelegramChatByExtIdAsync(update.Message!.Chat.Id, ct),
                    update: update,
                    ct: ct);
                return;
            }

            _logger.LogError("'{Error}' occured while ordering the reference", orderResult.Error);
            await _telegramMessageManager.SendMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "Кажется, что-то пошло не так. Пожалуйста, повторите попытку позже",
                ct: ct);
            return;
        }

        await _telegramMessageManager.SendMessageAsync(
            chatId: update.Message.Chat.Id,
            text: "Справка была успешно заказана, ваше сообщение отправлено методисту",
            ct: ct);
    }

    private Task<Student?> GetStudentByChatExtIdAsync(long chatExtId, CancellationToken ct = default)
        => _dataContext.Students
            .Include(x => x.MoodleAccount)
            .FirstOrDefaultAsync(x => x.TelegramChat!.ExtId == chatExtId, ct);

    private Task<PrivateTelegramChat> GetTelegramChatByExtIdAsync(long chatExtId, CancellationToken ct = default)
        => _dataContext.TelegramChats
            .Cast<PrivateTelegramChat>()
            .FirstAsync(x => x.ExtId == chatExtId, ct);
}