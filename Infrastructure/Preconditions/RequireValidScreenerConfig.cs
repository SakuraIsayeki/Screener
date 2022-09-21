using DSharpPlus.CommandsNext;
using Microsoft.Extensions.DependencyInjection;
using SakuraIsayeki.Screener.Data;
using SakuraIsayeki.Screener.Services;
using YumeChan.PluginBase.Infrastructure;

namespace SakuraIsayeki.Screener.Infrastructure.Preconditions;

/// <summary>
/// Provides a precondition check to validate guild configuration before executing a command.
/// </summary>
public class RequireValidScreenerConfig : PluginCheckBaseAttribute
{
	public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
	{
		GuildScreeningConfig config = await ctx.Services.GetRequiredService<GuildConfigService>().GetGuildScreeningConfigAsync(ctx.Guild.Id);
		
		// Check for valid Channels & Roles
		return help || config is not { GuestRoleIds.Length: 0, MemberRoleIds.Length: 0, ScreeningLogsChannelId: 0 };
	}

	public override string ErrorMessage { get; protected set; } = $"Please configure screening on this server first. Use {Utilities.ConfigurationUrl} to edit configuration.";
}