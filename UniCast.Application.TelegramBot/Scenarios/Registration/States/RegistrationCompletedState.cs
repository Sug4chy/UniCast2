using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationCompletedState : IRegistrationState
{
    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly IDataContext _dataContext;
    private readonly ITelegramMessageManager _telegramMessageManager;

    public RegistrationCompletedState(RegistrationScenarioExecutor scenarioExecutor, IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _dataContext = serviceProvider.GetRequiredService<IDataContext>();
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
    }

    public async Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        var student = await GetStudentAsync(chat.CurrentScenarioArgs[RegistrationScenarioArgsKeys.StudentFullName], ct);
        chat.Student = student;

        await _scenarioExecutor.ClearScenarioAsync(chat, ct);

        await _telegramMessageManager.SendMessageAsync(
            chatId: chat.ExtId,
            text: string.Format(RegistrationScenarioMessages.RegistrationCompleted, student.FullName.ToString()),
            ct: ct);
    }

    public Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
        => Task.CompletedTask;

    private Task<Student> GetStudentAsync(string fullName, CancellationToken ct = default)
        => _dataContext.Students
            .SingleAsync(x => x.FullName == fullName, ct);
}