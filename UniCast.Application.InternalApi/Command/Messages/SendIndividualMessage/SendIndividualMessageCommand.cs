using UniCast.Application.InternalApi.Models;

namespace UniCast.Application.InternalApi.Command.Messages.SendIndividualMessage;

public readonly record struct SendIndividualMessageCommand(
    List<StudentModel> Students,
    string Message,
    string From,
    int SenderId,
    long MessageId
) : ICommand;