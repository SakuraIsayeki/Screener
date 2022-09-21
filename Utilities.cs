using System.Diagnostics.Contracts;
using System.Security.Claims;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Components.Authorization;

namespace SakuraIsayeki.Screener;

public static class Utilities
{
	public const string ConfigurationUrl = "https://yumechan.nodsoft.net/p/SakuraIsayeki.Screener/";
	
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
	[Pure]
	public static IEnumerable<IGrouping<DiscordChannel?, DiscordChannel>> GetChannelsByCategory(this DiscordGuild guild) =>
		from channel in guild.Channels.Values
		group channel by channel.Parent into category
		select category;
	
	public static string? GetAvatarLink(this DiscordUser user, ushort size) => user.GetAvatarUrl(ImageFormat.Auto, size);
}