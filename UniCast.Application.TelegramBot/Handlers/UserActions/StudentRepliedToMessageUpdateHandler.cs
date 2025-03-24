using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Messages.Entities;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Application.TelegramBot.Handlers.UserActions;

public sealed class StudentRepliedToMessageUpdateHandler : IUpdateHandler
{
    private readonly IDataContext _dataContext;
    private readonly ITelegramMessageSender _telegramMessageSender;
    private readonly ILogger<StudentRepliedToMessageUpdateHandler> _logger; 

    public StudentRepliedToMessageUpdateHandler(
        IDataContext dataContext, 
        ITelegramMessageSender telegramMessageSender,
        ILogger<StudentRepliedToMessageUpdateHandler> logger)
    {
        _dataContext = dataContext;
        _telegramMessageSender = telegramMessageSender;
        _logger = logger;
    }

    public async ValueTask<bool> CanHandleAsync(Update update, CancellationToken ct = default)
        => update.Type is UpdateType.Message &&
           update.Message!.ReplyToMessage is not null &&
           await MessageExistsByIdsPairAsync(update.Message.Chat.Id, update.Message.Id, ct);

    public async Task HandleAsync(Update update, CancellationToken ct = default)
    {
        var message = await GetMessageFromMethodistByTelegramMessageIdsAsync(
            update.Message!.Chat.Id,
            update.Message!.Id,
            ct
        );

        if (message is null)
        {
            _logger.LogError("Received reply for unknown message from {ChatID}", update.Message.Chat.Id);
            return;
        }

        var student = await GetStudentByChatExtIdAsync(update.Message.Chat.Id, ct);
        if (student is null)
        {
            _logger.LogError("Received reply from unknown chat with ID {ChatID}", update.Message.Chat.Id);
            return;
        }

        var reply = StudentsReply.Create(
            id: IdOf<StudentsReply>.New(),
            replyText: update.Message.Text ?? string.Empty,
            student: student,
            message: message
        );

        _dataContext.StudentsReplies.Add(reply);
        await _dataContext.SaveChangesAsync(ct);

        await _telegramMessageSender.SendMessageAsync(
            chatId: update.Message.Chat.Id,
            text: "Ваше сообщение было успешно отправлено методисту!",
            ct: ct);
    }

    private Task<bool> MessageExistsByIdsPairAsync(long chatExtId, int messageExtId, CancellationToken ct = default)
        => _dataContext.TelegramMessages
            .AnyAsync(x => x.ExtId == messageExtId &&
                           x.Chat!.Type == TelegramChatType.Private &&
                           x.Chat.ExtId == chatExtId, ct);

    private Task<MessageFromMethodist?> GetMessageFromMethodistByTelegramMessageIdsAsync(
        long chatExtId,
        int messageExtId,
        CancellationToken ct = default)
        => _dataContext.TelegramMessages
            .Where(x => x.ExtId == messageExtId &&
                        x.Chat!.ExtId == chatExtId)
            .Select(x => x.SrcMessage)
            .SingleOrDefaultAsync(ct);

    private Task<Student?> GetStudentByChatExtIdAsync(long chatExtId, CancellationToken ct = default)
        => _dataContext.Students
            .SingleOrDefaultAsync(x => x.TelegramChat!.ExtId == chatExtId, ct);
}