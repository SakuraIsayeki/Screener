using DSharpPlus;
using Microsoft.AspNetCore.Authorization;

namespace SakuraIsayeki.Screener.Infrastructure.Security.Authorization;

public class GuildPermissionsRequirement : IAuthorizationRequirement
{
	public Permissions RequiredPermissions { get; init; }

	public GuildPermissionsRequirement(Permissions requirePermissions)
	{
		RequiredPermissions = requirePermissions;
	}
}