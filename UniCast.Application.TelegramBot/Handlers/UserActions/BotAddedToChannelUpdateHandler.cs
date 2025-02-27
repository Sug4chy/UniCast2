using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Repositories;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Handlers.UserActions;

public sealed partial class BotAddedToChannelUpdateHandler : IUpdateHandler
{
    private readonly string _botUsername;
    private readonly ITelegramChatRepository _telegramChatRepository;
    private readonly IAcademicGroupRepository _academicGroupRepository;
    private readonly ILogger<BotAddedToChannelUpdateHandler> _logger;

    public BotAddedToChannelUpdateHandler(
        string botUsername,
        ITelegramChatRepository telegramChatRepository,
        IAcademicGroupRepository academicGroupRepository,
        ILogger<BotAddedToChannelUpdateHandler> logger)
    {
        _botUsername = botUsername;
        _telegramChatRepository = telegramChatRepository;
        _academicGroupRepository = academicGroupRepository;
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

        string groupName = GroupNameRegex().Match(chatTitle).Value;
        var group = await _academicGroupRepository.GetByNameAsync(groupName, ct);
        if (group is null)
        {
            _logger.LogError("AcademicGroup with name {Name} wasn't found", groupName);
            return;
        }

        var channelResult = TelegramChannel.Create(chatTitle, update.MyChatMember!.Chat.Id, group);
        if (channelResult.IsFailure)
        {
            _logger.LogError("{Error} occured while creating TelegramChannel", channelResult.Error);
            return;
        }

        await _telegramChatRepository.AddChannelAsync(channelResult.Value, ct);
    }

    [GeneratedRegex("(ПрИ|БИ|ПИ)-\\d{3}")]
    private static partial Regex GroupNameRegex();
}