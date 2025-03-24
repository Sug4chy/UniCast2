using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Students.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Handlers.UserActions;

public sealed partial class BotAddedToChannelUpdateHandler : IUpdateHandler
{
    private readonly string _botUsername;
    private readonly IDataContext _dataContext;
    private readonly ILogger<BotAddedToChannelUpdateHandler> _logger;

    public BotAddedToChannelUpdateHandler(
        string botUsername,
        IDataContext dataContext,
        ILogger<BotAddedToChannelUpdateHandler> logger)
    {
        _botUsername = botUsername;
        _dataContext = dataContext;
        _logger = logger;
    }

    public ValueTask<bool> CanHandleAsync(Update update, CancellationToken ct = default)
        => ValueTask.FromResult(
            update.Type is UpdateType.MyChatMember &&
            update.MyChatMember!.Chat.Type is ChatType.Channel &&
            update.MyChatMember.NewChatMember.Status is ChatMemberStatus.Administrator &&
            update.MyChatMember.NewChatMember.User.Username == _botUsername[1..]
        );

    public async Task HandleAsync(Update update, CancellationToken ct = default)
    {
        string chatTitle = update.MyChatMember?.Chat.Title ?? string.Empty;
        if (GroupNameRegex().Match(chatTitle).Value.Length is 0)
        {
            _logger.LogWarning("Bot has been added to chat with title {Title}, which isn't academic group's chat",
                chatTitle);
            return;
        }

        var groupName = AcademicGroupName.From(GroupNameRegex().Match(chatTitle).Value);

        var group = await GetGroupByNameAsync(groupName, ct);
        if (group is null)
        {
            _logger.LogError("AcademicGroup with name {Name} wasn't found", groupName);
            return;
        }

        var channel = TelegramChannel.Create(IdOf<TelegramChat>.New(), chatTitle, update.MyChatMember!.Chat.Id, group);

        _dataContext.TelegramChats.Add(channel);
        await _dataContext.SaveChangesAsync(ct);
    }

    private Task<AcademicGroup?> GetGroupByNameAsync(AcademicGroupName groupName, CancellationToken ct = default)
        => _dataContext.AcademicGroups
            .SingleOrDefaultAsync(x => x.Name == groupName, ct);

    [GeneratedRegex("(ПрИ|БИ|ПИ)-\\d{3}")]
    private static partial Regex GroupNameRegex();
}