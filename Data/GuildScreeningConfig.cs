using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SakuraIsayeki.Screener.Data;

/// <summary>
/// Represents guild-specific configuration.
/// </summary>
public record GuildScreeningConfig
{
	/// <summary>
	/// ID of the guild to which this configuration belongs.
	/// </summary>
	[Key, BsonId, BsonRepresentation(BsonType.Int64)]
	public ulong GuildId { get; init; }
	
	/// <summary>
	/// Channel ID of the channel to which the plugin should log screening operations.
	/// </summary>
	public ulong ScreeningLogsChannelId { get; set; }

	/// <summary>
	/// IDs of roles belonging to guests.
	/// </summary>
	public ulong[] GuestRoleIds { get; set; } = Array.Empty<ulong>();
	
	/// <summary>
	/// IDs of roles belonging to members.
	/// </summary>
	public ulong[] MemberRoleIds { get; set; } = Array.Empty<ulong>();
}