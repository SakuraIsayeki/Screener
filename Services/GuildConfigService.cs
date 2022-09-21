using MongoDB.Driver;
using SakuraIsayeki.Screener.Data;

namespace SakuraIsayeki.Screener.Services;

/// <summary>
/// Defines a provider for <see cref="GuildScreeningConfig"/> objects.
/// </summary>
public sealed class GuildConfigService
{
	private readonly IMongoCollection<GuildScreeningConfig> _configs;

	public GuildConfigService(IMongoCollection<GuildScreeningConfig> configs)
	{
		_configs = configs;
	}
	
	/// <summary>
	/// Gets the <see cref="GuildScreeningConfig"/> for the specified guild.
	/// </summary>
	/// <remarks>
	///	This method will create a new <see cref="GuildScreeningConfig"/> if one does not exist.
	/// </remarks>
	/// <param name="guildId">The ID of the guild.</param>
	/// <returns>The <see cref="GuildScreeningConfig"/> for the specified guild.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="guildId"/> is <c>null</c> or <c>0</c>.</exception>
	public async Task<GuildScreeningConfig> GetGuildScreeningConfigAsync(ulong guildId)
	{
		if (guildId is 0) throw new ArgumentNullException(nameof(guildId));

		GuildScreeningConfig? config = await _configs.Find(x => x.GuildId == guildId).FirstOrDefaultAsync();
		if (config is null)
		{
			config = new() { GuildId = guildId };
			await _configs.InsertOneAsync(config);
		}

		return config;
	}
	
	/// <summary>
	/// Updates/Saves the specified <see cref="GuildScreeningConfig"/>.
	/// </summary>
	/// <param name="screeningConfig">The <see cref="GuildScreeningConfig"/> to save.</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="screeningConfig"/> is <c>null</c>.</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="screeningConfig"/> does not have a valid <see cref="GuildScreeningConfig.GuildId"/>.</exception>
	/// <exception cref="InvalidOperationException">Thrown if the <see cref="GuildScreeningConfig"/> could not be saved.</exception>
	public async Task SaveGuildScreeningConfigAsync(GuildScreeningConfig screeningConfig)
	{
		if (screeningConfig is null) throw new ArgumentNullException(nameof(screeningConfig));
		if (screeningConfig.GuildId is 0) throw new ArgumentException("Guild ID must be set.", nameof(screeningConfig));

		try
		{
			await _configs.ReplaceOneAsync(x => x.GuildId == screeningConfig.GuildId, screeningConfig, new ReplaceOptions { IsUpsert = true });
		}
		catch (Exception e)
		{
			throw new InvalidOperationException("Failed to save guild screeningConfig.", e);
		}
	}
}



/*
 * Thank you Copilot for this quality code and docs <3
 *   - Sakura Akeno Isayeki
 */