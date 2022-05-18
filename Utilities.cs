using System.Security.Claims;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Components.Authorization;

namespace SakuraIsayeki.Screener;

public static class Utilities
{
	/// <summary>
	/// Get the Discord Snowflake ID of the currently authenticated user.
	/// </summary>
	public static async Task<ulong?> GetUserSnowflakeAsync(this AuthenticationStateProvider authenticationStateProvider) 
		=> (await authenticationStateProvider.GetAuthenticationStateAsync()).User is { } user
			&& user.FindFirst(ClaimTypes.NameIdentifier) is { } claim
			&& ulong.TryParse(claim.Value, out ulong snowflake)
				? snowflake
				: null;
	
	/// <summary>
	/// Gets all channels in the specified guild, grouped by category.
	/// </summary>
	/// <remarks>
	///	This method is used to structure a channel list for channel selectors.
	/// </remarks>
	/// <returns></returns>
	public static IEnumerable<IGrouping<DiscordChannel?, DiscordChannel>> GetChannelsByCategory(this DiscordGuild guild) =>
		from channel in guild.Channels.Values
		group channel by channel.Parent into category
		select category;
}