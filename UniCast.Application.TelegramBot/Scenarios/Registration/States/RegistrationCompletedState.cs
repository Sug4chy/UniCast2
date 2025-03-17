using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Repositories;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Students.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationCompletedState : IRegistrationState
{
    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly IAcademicGroupRepository _academicGroupRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ITelegramChatRepository _telegramChatRepository;
    private readonly ITelegramMessageSender _telegramMessageSender;

    public RegistrationCompletedState(RegistrationScenarioExecutor scenarioExecutor, IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _academicGroupRepository = serviceProvider.GetRequiredService<IAcademicGroupRepository>();
        _studentRepository = serviceProvider.GetRequiredService<IStudentRepository>();
        _telegramMessageSender = serviceProvider.GetRequiredService<ITelegramMessageSender>();
        _telegramChatRepository = serviceProvider.GetRequiredService<ITelegramChatRepository>();
    }

    public async Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        var fullName = StudentFullName.From(chat.CurrentScenarioArgs["FULL_NAME"]).Value;

        var groupName = AcademicGroupName.From(chat.CurrentScenarioArgs["GROUP_NAME"]).Value;
        var group = await _academicGroupRepository.GetByNameAsync(groupName, ct);

        var student = Student.Create(
            id: IdOf<Student>.New(),
            fullName: fullName,
            group: group!);
        await _studentRepository.AddStudentAsync(student, ct);

        chat.Student = student;
        await _telegramChatRepository.UpdateStudentForPrivateChatAsync(chat, ct);

        await _telegramMessageSender.SendMessageAsync(
            chatId: chat.ExtId,
            text: "Поздравляю вас с успешным завершением регистрации!",
            ct: ct);

        await _scenarioExecutor.ClearScenarioAsync(chat, ct);
    }

    public Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
        => Task.CompletedTask;
}