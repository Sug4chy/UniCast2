using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Students.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationCompletedState : IRegistrationState
{
    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly IDataContext _dataContext;
    private readonly ITelegramMessageSender _telegramMessageSender;

    public RegistrationCompletedState(RegistrationScenarioExecutor scenarioExecutor, IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _dataContext = serviceProvider.GetRequiredService<IDataContext>();
        _telegramMessageSender = serviceProvider.GetRequiredService<ITelegramMessageSender>();
    }

    public async Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        var fullName = StudentFullName.From(chat.CurrentScenarioArgs["FULL_NAME"]);

        var groupName = AcademicGroupName.From(chat.CurrentScenarioArgs["GROUP_NAME"]);
        var group = await GetGroupByNameAsync(groupName, ct);

        var student = Student.Create(
            id: IdOf<Student>.New(),
            fullName: fullName,
            group: group!);
        _dataContext.Students.Add(student);

        chat.Student = student;
        await _dataContext.SaveChangesAsync(ct);

        await _telegramMessageSender.SendMessageAsync(
            chatId: chat.ExtId,
            text: "Поздравляю вас с успешным завершением регистрации!",
            ct: ct);

        await _scenarioExecutor.ClearScenarioAsync(chat, ct);
    }

    public Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
        => Task.CompletedTask;

    private Task<AcademicGroup?> GetGroupByNameAsync(AcademicGroupName groupName, CancellationToken ct = default)
        => _dataContext.AcademicGroups
            .SingleOrDefaultAsync(x => x.Name == groupName, ct);
}