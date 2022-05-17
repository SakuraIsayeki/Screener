using MongoDB.Driver;
using SakuraIsayeki.Screener.Data;

namespace SakuraIsayeki.Screener.Services;

/// <summary>
/// Defines a provider for <see cref="GuildConfig"/> objects.
/// </summary>
public class GuildConfigService
{
	private readonly IMongoCollection<GuildConfig> _configs;

	public GuildConfigService(IMongoCollection<GuildConfig> configs)
	{
		_configs = configs;
	}
	
	/// <summary>
	/// Gets the <see cref="GuildConfig"/> for the specified guild.
	/// </summary>
	/// <remarks>
	///	This method will create a new <see cref="GuildConfig"/> if one does not exist.
	/// </remarks>
	/// <param name="guildId">The ID of the guild.</param>
	/// <returns>The <see cref="GuildConfig"/> for the specified guild.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="guildId"/> is <c>null</c> or <c>0</c>.</exception>
	public async Task<GuildConfig> GetGuildConfigAsync(ulong guildId)
	{
		if (guildId is 0) throw new ArgumentNullException(nameof(guildId));

		GuildConfig? config = await _configs.Find(x => x.GuildId == guildId).FirstOrDefaultAsync();
		if (config is null)
		{
			config = new() { GuildId = guildId };
			await _configs.InsertOneAsync(config);
		}

		return config;
	}
	
	/// <summary>
	/// Updates/Saves the specified <see cref="GuildConfig"/>.
	/// </summary>
	/// <param name="config">The <see cref="GuildConfig"/> to save.</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="config"/> is <c>null</c>.</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="config"/> does not have a valid <see cref="GuildConfig.GuildId"/>.</exception>
	/// <exception cref="InvalidOperationException">Thrown if the <see cref="GuildConfig"/> could not be saved.</exception>
	public async Task SaveGuildConfigAsync(GuildConfig config)
	{
		if (config is null) throw new ArgumentNullException(nameof(config));
		if (config.GuildId is 0) throw new ArgumentException("Guild ID must be set.", nameof(config));

		try
		{
			await _configs.ReplaceOneAsync(x => x.GuildId == config.GuildId, config, new ReplaceOptions { IsUpsert = true });
		}
		catch (Exception e)
		{
			throw new InvalidOperationException("Failed to save guild config.", e);
		}
	}
}



/*
 * Thank you Copilot for this quality code and docs <3
 *   - Sakura Akeno Isayeki
 */