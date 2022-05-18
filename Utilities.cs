using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace SakuraIsayeki.Screener;

public static class Utilities
{
	public static async Task<ulong?> GetUserSnowflakeAsync(this AuthenticationStateProvider authenticationStateProvider) 
		=> (await authenticationStateProvider.GetAuthenticationStateAsync()).User is { } user
			&& user.FindFirst(ClaimTypes.NameIdentifier) is { } claim
			&& ulong.TryParse(claim.Value, out ulong snowflake)
				? snowflake
				: null;
}