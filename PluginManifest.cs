using Microsoft.Extensions.DependencyInjection;
using YumeChan.PluginBase;

namespace YumeChan.PluginSampleCS;

// This Class defines the Plugin, and makes it visible to the Loader.
public class PluginManifest : Plugin // This Class MUST be set as public to get picked up by the Plugin Loader.
{
	// This defines your Plugin's Display name.
	public override string DisplayName => "YumeChan : Sample Plugin";

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

/*
 * This class allows you to register furter services to YumeChan's DI Container.
 * This is optional, and you can remove it if you don't need it.
 */
public class DependencyInjectionAddons : DependencyInjectionHandler
{
	public override IServiceCollection ConfigureServices(IServiceCollection services)
	{
		/*
		 * Here you can register your services to YumeChan's DI Container.
		 * For dependencies that may be shared, and are not defined in your namespaces,
		 * consider using the TryAdd methods, instead of Add.
		 */
		
		// Add your services here.
		
		return services;
	}
}