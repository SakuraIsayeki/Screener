using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JetBrains.Annotations;
using SakuraIsayeki.Screener.Data;
using SakuraIsayeki.Screener.Infrastructure.Preconditions;
using SakuraIsayeki.Screener.Services;

namespace SakuraIsayeki.Screener.Commands;


/// <summary>
/// Base group for all Screener commands.
/// </summary>
[Group("screener"), Description("Base prefix for all Screener commands."), RequireGuild]
public sealed partial class BaseCommandGroup : BaseCommandModule
{
	private readonly ScreeningService _screeningService;

	public BaseCommandGroup(ScreeningService screeningService)
	{
		_screeningService = screeningService;
	}

	/// <summary>
	/// Accepts a user through the screening process.
	/// </summary>
	/// <param name="ctx">The context of the slash command.</param>
	/// <param name="member">The member to accept.</param>
	[Command("accept"), Description("Accepts a user through screening, granting them member roles.")]
	[RequireValidScreenerConfig, RequirePermissions(Permissions.KickMembers), RequireBotPermissions(Permissions.ManageRoles), UsedImplicitly]
	public async Task AcceptAsync(CommandContext ctx, 
		[Description("User to accept screening for")] DiscordMember member)
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

	/// <summary>
	/// Rejects a user from the screening process.
	/// </summary>
	/// <param name="ctx">The context of the slash command.</param>
	/// <param name="member">The member to reject.</param>
	/// <param name="reason">The reason for rejecting the user.</param>
	[Command("reject"), Description("Rejects a user from screening"), RequireValidScreenerConfig, RequirePermissions(Permissions.KickMembers), UsedImplicitly]
	public async Task RejectAsync(CommandContext ctx, 
		[Description("User to reject from screening")] DiscordMember member, 
		[RemainingText, Description("Reason for rejection")] string? reason = null)
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