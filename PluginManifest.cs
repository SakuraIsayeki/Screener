using DSharpPlus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SakuraIsayeki.Screener.Data;
using SakuraIsayeki.Screener.Infrastructure.Security.Authorization;
using SakuraIsayeki.Screener.Services;
using YumeChan.PluginBase;
using YumeChan.PluginBase.Tools.Data;

namespace SakuraIsayeki.Screener;

/// <summary>
/// Defines the Plugin Manifest for the Screener Plugin.
/// </summary>
public class PluginManifest : Plugin // This Class MUST be set as public to get picked up by the Plugin Loader.
{
	// This defines your Plugin's Display name.
	public override string DisplayName => "Screener";

	// This flag defines whether your Plugin should be shown to the general public or not.
	// Still shown to Server Operators, this is useful for security plugins, or plugins requiring covert operation.
	public override bool StealthMode => false;

	public override async Task LoadAsync()
	{
		//Here goes the Loading Logic, if some loading procedure is needed. Treat it as a Ctor.


		await base.LoadAsync(); // This method call sets Plugin.PluginLoaded to true.
	}
	public override async Task UnloadAsync()
	{
		//Here goes the Unloading Logic, if some unloading (e.g: Disposal) is needed. Treat it as a Dtor (~).


		await base.UnloadAsync(); // This method call resets Plugin.PluginLoaded to false.
	}
}

/// <summary>
/// Defines additions to the DI Container.
/// </summary>
public class DependencyInjectionAddons : DependencyInjectionHandler
{
	public override IServiceCollection ConfigureServices(IServiceCollection services)
	{
		services.AddSingleton(s => s.GetRequiredService<IDatabaseProvider<PluginManifest>>().GetMongoDatabase().GetCollection<GuildScreeningConfig>("screeningConfig"));
		
		services.AddSingleton<GuildConfigService>();
		services.AddSingleton<ScreeningService>();
		
		services.AddAuthorizationCore(options =>
		{
			options.AddPolicy(AuthorizationExtensions.RequireOperatorPermission, policy => policy
				.RequireGuildRole(Permissions.ManageGuild | Permissions.ManageRoles));
			
			options.AddPolicy(AuthorizationExtensions.RequireScreenerPermission, policy => policy
				.RequireGuildRole(Permissions.KickMembers));
			
			options.AddPolicy(AuthorizationExtensions.RequireAdminPermission, policy => policy
				.RequireGuildRole(Permissions.Administrator));
		});
		
		services.AddScoped<IAuthorizationHandler, GuildAccessAuthorizationHandler>();
		
		return services;
	}
}