using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using SakuraIsayeki.Screener.Data;

namespace SakuraIsayeki.Screener.Services;

/// <summary>
/// Provides mechanisms to screen guild members (Accept/Reject users).
/// </summary>
public class ScreeningService
{
	private readonly GuildConfigService _configService;
	private readonly ILogger<ScreeningService> _logger;

	public ScreeningService(GuildConfigService configService, ILogger<ScreeningService> logger)
	{
		_configService = configService;
		_logger = logger;
	}

	/// <summary>
	/// Accepts a new member into the guild, switching their guest roles with member-specific roles.
	/// </summary>
	/// <param name="guild">Guild on which the screening is taking place on</param>
	/// <param name="member">Guild member being screened</param>
	/// <param name="acceptedBy">Guild operator who screened the user</param>
	public async Task AcceptMemberAsync(DiscordGuild guild, DiscordMember member, DiscordMember acceptedBy)
	{
		GuildScreeningConfig screeningConfig = await _configService.GetGuildScreeningConfigAsync(guild.Id);
		
		// Add the member roles to the member
		// Parrallelize this to speed up the process
		await Task.WhenAll(screeningConfig.MemberRoleIds.Select(role => member.GrantRoleAsync(guild.GetRole(role))));
		
		// Remove the guest roles from the member
		// Same spiel as above
		await Task.WhenAll(screeningConfig.GuestRoleIds.Select(role => member.RevokeRoleAsync(guild.GetRole(role))));

		// Send a message to the user
		// TODO: Make this a configurable message
		
		// Report to the Screening logs channel that the user has been accepted
		if (guild.Channels.TryGetValue(screeningConfig.ScreeningLogsChannelId, out DiscordChannel? channel) && channel is { Type: ChannelType.Text })
		{
			await channel.SendMessageAsync($"{member.Mention} has been accepted by {acceptedBy.Mention}");
		}
		// Anything else is strange. Log the missing channel.
		else
		{
			_logger.LogWarning("Screening logs channel not found for guild {GuildId}", guild.Id.ToString());
		}
	}

	/// <summary>
	/// Rejects a new member into the guild, removing their guest roles, and optionally removing them from the guild.
	/// </summary>
	/// <param name="guild">Guild on which the screening is taking place on</param>
	/// <param name="member">Guild member being screened</param>
	/// <param name="rejectedBy">Guild operator who screened the user</param>
	/// <param name="actions">Actions to be taken on the rejected user</param>
	/// <param name="reason">Reason for the rejection</param>
	public async Task RejectMemberAsync(DiscordGuild guild, DiscordMember member, DiscordMember rejectedBy, ScreeningRejectActions actions, string reason)
	{
		GuildScreeningConfig screeningConfig = await _configService.GetGuildScreeningConfigAsync(guild.Id);

		// Log the rejection to the screening logs channel
		if (guild.Channels.TryGetValue(screeningConfig.ScreeningLogsChannelId, out DiscordChannel? channel) && channel is { Type: ChannelType.Text })
		{
			await channel.SendMessageAsync($"{member.Mention} has been rejected by {rejectedBy.Mention} for {reason}");
		}
		// Or log a missing channel
		else
		{
			_logger.LogWarning("Screening logs channel not found for guild {GuildId}", guild.Id.ToString());
		}

		// Act upon actions requested
		if ((actions & ScreeningRejectActions.InformUser) is not 0)
		{
			// Send a message to the user
			// TODO: message
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
			await Task.WhenAll(screeningConfig.GuestRoleIds.Select(role => member.RevokeRoleAsync(guild.GetRole(role))));
		}
	}
}