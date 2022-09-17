using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using SakuraIsayeki.Screener.Data;

namespace SakuraIsayeki.Screener.Services;

/// <summary>
/// Provides mechanisms to screen guild members (Accept/Reject users).
/// </summary>
public sealed class ScreeningService
{
	private readonly GuildConfigService _configService;
	private readonly GreetingService _greetingService;
	private readonly ILogger<ScreeningService> _logger;

	public ScreeningService(GuildConfigService configService, GreetingService greetingService, ILogger<ScreeningService> logger)
	{
		_configService = configService;
		_greetingService = greetingService;
		_logger = logger;
	}

	/// <summary>
	/// Accepts a new member into the guild, switching their guest roles with member-specific roles.
	/// </summary>
	/// <param name="member">Guild member being screened</param>
	/// <param name="acceptedBy">Guild operator who screened the user</param>
	public async Task AcceptMemberAsync(DiscordMember member, DiscordMember acceptedBy)
	{
		GuildScreeningConfig screeningConfig = await _configService.GetGuildScreeningConfigAsync(member.Guild.Id);

		// Add the member roles to the member
		// Parrallelize this to speed up the process
		await Task.WhenAll(screeningConfig.MemberRoleIds.Select(role => member.GrantRoleAsync(member.Guild.GetRole(role))));

		// Remove the guest roles from the member
		// Same spiel as above
		await Task.WhenAll(screeningConfig.GuestRoleIds.Select(role => member.RevokeRoleAsync(member.Guild.GetRole(role))));
		
		// Report to the Screening logs channel that the user has been accepted
		if (member.Guild.Channels.TryGetValue(screeningConfig.ScreeningLogsChannelId, out DiscordChannel? channel) && channel is { Type: ChannelType.Text })
		{
			await channel.SendMessageAsync($"User {member.Mention} has been accepted by {acceptedBy.Mention}");
		}
		// Anything else is strange. Log the missing channel.
		else
		{
			_logger.LogWarning("Screening logs channel not found for guild {GuildId}", member.Guild.Id);
		}

		_logger.LogInformation("User {UserId} has been accepted in guild {GuildId} by {AcceptedById}.", member.Id, member.Guild.Id, acceptedBy.Id);
		
		// Send greeting messages
		await Task.WhenAll(
			_greetingService.GreetUserDmAsync(member),
			_greetingService.GreetUserGuildAsync(member)
		);
	}

	/// <summary>
	/// Rejects a new member into the guild, removing their guest roles, and optionally removing them from the guild.
	/// </summary>
	/// <param name="member">Guild member being screened</param>
	/// <param name="rejectedBy">Guild operator who screened the user</param>
	/// <param name="actions">Actions to be taken on the rejected user</param>
	/// <param name="reason">Reason for the rejection</param>
	public async Task RejectMemberAsync(DiscordMember member, DiscordMember rejectedBy, ScreeningRejectActions actions, string reason)
	{
		GuildScreeningConfig screeningConfig = await _configService.GetGuildScreeningConfigAsync(member.Guild.Id);

		// Log the rejection to the screening logs channel
		if (member.Guild.Channels.TryGetValue(screeningConfig.ScreeningLogsChannelId, out DiscordChannel? channel) && channel is { Type: ChannelType.Text })
		{
			await channel.SendMessageAsync($"User {member.Mention} has been rejected by {rejectedBy.Mention} for {reason}");
		}
		// Or log a missing channel
		else
		{
			_logger.LogWarning("Screening logs channel not found for guild {GuildId}", member.Guild.Id);
		}

		// Act upon actions requested
		if ((actions & ScreeningRejectActions.InformUser) is not 0)
		{
			// Send a message to the user
			await _greetingService.RejectUserDmAsync(member, reason);
		}

		if ((actions & ScreeningRejectActions.Ban) is not 0)
		{
			// Ban the user
			await member.BanAsync(reason: reason);
		}
		else if ((actions & ScreeningRejectActions.Kick) is not 0)
		{
			// Kick the user
			await member.RemoveAsync(reason);
		}
		else if ((actions & ScreeningRejectActions.RemoveGuestRoles) is not 0)
		{
			// Remove guest roles from the user
			await Task.WhenAll(screeningConfig.GuestRoleIds.Select(member.Guild.GetRole).Select(role => member.RevokeRoleAsync(role, $"Rejected screening: {reason}")));
		}
		
		_logger.LogInformation("User {UserId} has been rejected in guild {GuildId} by {OperatorId}. (Actions: {Actions:F})", member.Id, member.Guild.Id, rejectedBy.Id, actions);
	}
	
	/// <summary>
	/// Checks a user's roles against the guild's screening config, making sure that they possess all member roles.
	/// </summary>
	/// <param name="member">Server member to check against screening</param>
	/// <returns><see langword="true"/> for a member, <see langword="false"/> for a guest.</returns>
	public async Task<bool> UserWasScreenedAsync(DiscordMember member)
	{
		// Get the screening config
		GuildScreeningConfig screeningConfig = await _configService.GetGuildScreeningConfigAsync(member.Guild.Id);

		// Check if the user has all the member roles
		return screeningConfig.MemberRoleIds.All(member.Roles.Select(static r => r.Id).Contains);
	}
}