using System.ComponentModel.DataAnnotations;

namespace SakuraIsayeki.Screener.Data;

/// <summary>
/// Represents a greeting message template, used to generate a greeting/screening message.
/// </summary>
public record MessageTemplate
{
	/// <summary>
	/// The template for the content of the message.
	/// </summary>
	[MaxLength(2048)]
	public string? Message { get; set; }
	
	/// <summary>
	/// Whether the greeting message should be accompanied by an embed.
	/// </summary>
	public bool HasEmbed { get; set; }
	
	/// <summary>
	/// The template for the embed title.
	/// </summary>
	/// <remarks>
	/// If <see cref="HasEmbed"/> is <see langword="false"/>, this property is ignored.
	/// Otherwise this property is required.
	/// </remarks>
	[MaxLength(256)]
	public string? EmbedTitle { get; set; }
	
	/// <summary>
	/// The template for the embed body, if any.
	/// </summary>
	/// <remarks>
	/// If <see cref="HasEmbed"/> is <see langword="false"/>, this property is ignored.
	/// </remarks>
	[MaxLength(4096)]
	public string? EmbedBody { get; set; }
	
	/// <summary>
	/// Fixed image URI for the embed, if any.
	/// </summary>
	/// <remarks>
	/// - If <see cref="HasEmbed"/> is <see langword="false"/>, this property is ignored.
	/// - If this property is <see langword="null"/>, the embed's image will be set to the receiving user's avatar.
	/// </remarks>
	public string? EmbedImageUri { get; set; }
}

