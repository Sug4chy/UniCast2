using UniCast.Application.InternalApi.Models;

namespace UniCast.Application.InternalApi.Command.Messages.SendIndividualMessage;

public record struct SendIndividualMessageCommand(
    List<StudentModel> Students,
    string Message,
    string From,
    int SenderId
) : ICommand;