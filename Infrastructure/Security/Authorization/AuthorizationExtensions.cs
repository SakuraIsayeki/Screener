using DSharpPlus;
using Microsoft.AspNetCore.Authorization;

namespace SakuraIsayeki.Screener.Infrastructure.Security.Authorization;

public static class AuthorizationExtensions
{
	public const string RequireOperatorPermission = "SakuraIsayeki.Screener-RequireOperatorPermission";
	public const string RequireScreenerPermission = "SakuraIsayeki.Screener-RequireScreenerPermission";
	public const string RequireAdminPermission = "SakuraIsayeki.Screener-RequireAdminPermission";

	public static AuthorizationPolicyBuilder RequireGuildRole(this AuthorizationPolicyBuilder builder, Permissions permissions)
	{
		builder.AddRequirements(new GuildPermissionsRequirement(permissions));
		return builder;
	}
}