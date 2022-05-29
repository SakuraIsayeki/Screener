using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SakuraIsayeki.Screener.Data;
using SakuraIsayeki.Screener.Infrastructure.Preconditions;
using SakuraIsayeki.Screener.Services;

namespace SakuraIsayeki.Screener.Commands;


/// <summary>
/// Base group for all Screener slash commands.
/// </summary>
[Group("screener"), Description("Base prefix for all Screener commands."), RequireGuild]
public partial class BaseCommandGroup : ApplicationCommandModule
{
	private readonly ScreeningService _screeningService;

	public BaseCommandGroup(ScreeningService screeningService)
	{
		_screeningService = screeningService;
	}

	[Command("accept"), Description("Accepts a user through screening, granting them member roles."), RequireValidScreenerConfig, RequirePermissions(Permissions.KickMembers)]
	public async Task AcceptAsync(CommandContext ctx, [Description("User to accept screening for")] DiscordMember member)
	{
		// Check if the user is already a member.
		if (await _screeningService.UserWasScreenedAsync(member))
		{
			await ctx.RespondAsync($"{member.Mention} is already a member.");
			return;
		}
		
		// Otherwise, accept the user.
		await _screeningService.AcceptMemberAsync(member, ctx.Member!);
	}

	[Command("reject"), Description("Rejects a user from screening"), RequireValidScreenerConfig, RequirePermissions(Permissions.KickMembers)]
	public async Task RejectAsync(CommandContext ctx, [Description("User to reject from screening")] DiscordMember member, [RemainingText] string? reason = null)
	{
		// Check if the user is already a member.
		if (await _screeningService.UserWasScreenedAsync(member))
		{
			await ctx.RespondAsync($"{member.Mention} is already a member.");
			return;
		}
		
		// Otherwise, accept the user.
		await _screeningService.RejectMemberAsync(member, ctx.Member!, ScreeningRejectActions.InformUser, reason);
	}
}