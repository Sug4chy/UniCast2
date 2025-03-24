using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Domain.Students.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.Registration.States;

public sealed class RegistrationWaitingForGroupNameEnteredState : IRegistrationState
{
    private const string GroupNameCallbackDataStart = "GROUP_NAME_CALLBACK_";

    private readonly RegistrationScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageSender _telegramMessageSender;
    private readonly IDataContext _dataContext;
    private readonly ILogger<RegistrationWaitingForGroupNameEnteredState> _logger;

    public RegistrationWaitingForGroupNameEnteredState(
        RegistrationScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageSender = serviceProvider.GetRequiredService<ITelegramMessageSender>();
        _dataContext = serviceProvider.GetRequiredService<IDataContext>();
        _logger = serviceProvider.GetRequiredService<ILogger<RegistrationWaitingForGroupNameEnteredState>>();
    }

    public async Task OnStateChangedAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        await _telegramMessageSender.SendMessageAsync(
            chatId: chat.ExtId,
            text: "Принято! Теперь выбери, в какой группе ты состоишь.",
            inlineKeyboard: new InlineKeyboardMarkup(
                await GetInlineKeyboardWithGroupsNames(ct)
            ),
            ct: ct
        );
    }

    public async Task HandleUserInputAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
    {
        if (update.Type is not UpdateType.CallbackQuery ||
            !update.CallbackQuery!.Data!.StartsWith(GroupNameCallbackDataStart))
        {
            _logger.LogError("Update with ID {UpdateID} is invalid in {StateName}",
                update.Id, nameof(RegistrationWaitingForFullNameEnteredState));
            return;
        }

        var groupName = AcademicGroupName.From(update.CallbackQuery.Data[GroupNameCallbackDataStart.Length..]);
        if (!await GroupExistsByNameAsync(groupName, ct))
        {
            await _telegramMessageSender.SendMessageAsync(
                chatId: chat.ExtId,
                text: "Я не знаю такую группу. Пожалуйста, повторите ввод:",
                inlineKeyboard: new InlineKeyboardMarkup(
                    await GetInlineKeyboardWithGroupsNames(ct)
                ),
                ct: ct);
            return;
        }

        chat.CurrentScenarioArgs["GROUP_NAME"] = groupName;

        await _scenarioExecutor.ChangeStateAsync(chat,
            _scenarioExecutor.GetState((int)RegistrationScenarioState.Completed),
            update, ct);
    }

    private async Task<IEnumerable<InlineKeyboardButton>> GetInlineKeyboardWithGroupsNames(
        CancellationToken ct = default)
    {
        var groupNames = await _dataContext.AcademicGroups.Select(x => x.Name).ToListAsync(ct);

        return groupNames
            .Select(s => new InlineKeyboardButton(s, GroupNameCallbackDataStart + s));
    }

    private Task<bool> GroupExistsByNameAsync(AcademicGroupName groupName, CancellationToken ct = default)
        => _dataContext.AcademicGroups
            .AnyAsync(x => x.Name == groupName, ct);
}