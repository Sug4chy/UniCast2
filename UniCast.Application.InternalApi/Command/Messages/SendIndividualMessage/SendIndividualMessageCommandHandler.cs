using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.Result;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Messages.Entities;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Students.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.InternalApi.Command.Messages.SendIndividualMessage;

public sealed class SendIndividualMessageCommandHandler : ICommandHandler<SendIndividualMessageCommand>
{
    private readonly IDataContext _dataContext;
    private readonly ITelegramMessageSender _telegramMessageSender;
    private readonly ILogger<SendIndividualMessageCommandHandler> _logger;

    public SendIndividualMessageCommandHandler(
        IDataContext dataContext,
        ITelegramMessageSender telegramMessageSender,
        ILogger<SendIndividualMessageCommandHandler> logger)
    {
        _dataContext = dataContext;
        _telegramMessageSender = telegramMessageSender;
        _logger = logger;
    }

    public async Task<UnitResult<Error>> HandleAsync(
        SendIndividualMessageCommand command,
        CancellationToken ct = default)
    {
        List<Student> students = [];
        foreach (var studentModel in command.Students)
        {
            var student = await GetStudentByFullNameAsync(studentModel.FullName, ct);
            if (student is null)
            {
                _logger.LogWarning("Student with name {StudentName} wasn't found", studentModel.FullName);
                continue;
            }

            students.Add(student);
        }

        var messageFromMethodistResult = CSharpFunctionalExtensions.Result.Try(
            () => MessageFromMethodist.Create(
                id: IdOf<MessageFromMethodist>.New(),
                body: command.Message,
                senderUsername: command.From),
            e => e.Message
        );
        if (messageFromMethodistResult.IsFailure)
        {
            return UnitResult.Failure(Error.Of(messageFromMethodistResult.Error, ErrorGroup.DomainError));
        }

        messageFromMethodistResult.Value.Students = students;
        _dataContext.MessageFromMethodists.Add(messageFromMethodistResult.Value);
        foreach (var student in students)
        {
            var chat = await GetPrivateChatForStudentAsync(student, ct);
            if (chat is null)
            {
                _logger.LogWarning("Chat for student with name {StudentName} wasn't found", student.FullName);
                continue;
            }

            var message = await _telegramMessageSender.SendMessageAsync(chat, command.Message, ct: ct);
            message.SrcMessageId = messageFromMethodistResult.Value.Id;
            _dataContext.TelegramMessages.Add(message);
        }

        await _dataContext.SaveChangesAsync(ct);

        return UnitResult.Success<Error>();
    }

    private Task<Student?> GetStudentByFullNameAsync(StudentFullName fullName, CancellationToken ct = default)
        => _dataContext.Students
            .Include(x => x.Group)
            .SingleOrDefaultAsync(x => x.FullName == fullName, ct);

    private Task<PrivateTelegramChat?> GetPrivateChatForStudentAsync(Student student, CancellationToken ct = default)
        => _dataContext.TelegramChats
            .Cast<PrivateTelegramChat>()
            .SingleOrDefaultAsync(x => x.Student == student, ct);
}