using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using SakuraIsayeki.Screener.Data;
using TextPress;

namespace SakuraIsayeki.Screener.Services;

/// <summary>
/// Provides greeting functionality for screening procedures.
/// </summary>
public class GreetingService
{
	private readonly GuildConfigService _configService;
	private readonly ILogger<GreetingService> _logger;
	private readonly StringTemplate _stringTemplate;

	public GreetingService(GuildConfigService configService, ILogger<GreetingService> logger, StringTemplateFactory stringTemplateFactory)
	{
		_configService = configService;
		_logger = logger;
		
		_stringTemplate = stringTemplateFactory.GetTemplate(typeof(GreetingService).FullName, new StringTemplateOptions
		{
			EscapingStyle = VariableEscapingStyle.DoubleDelimiters,
			RegexOptions = RegexOptions.Compiled // Compile the regex for better performance
		});
	}
	
	/// <summary>
	/// Sends a greeting message to the user, in the user's Direct Messages.
	/// </summary>
	/// <param name="member">The user to greet.</param>
	public async Task GreetUserDmAsync(DiscordMember member)
	{
		// Get the guild configuration
		GuildScreeningConfig config = await _configService.GetGuildScreeningConfigAsync(member.Guild.Id);
		
		// If the greeting message template is not null, build a greeting message and send it to the user
		if (config.GreetingDmMessage is not { } template)
		{
			_logger.LogDebug("Greeting message template is null or empty, not sending greeting message.");
			return;
		}

		// Build and send the greeting message/embed
		(string? message, DiscordEmbed? embed) = BuildMessageComponents(template, member);
		_logger.LogDebug("Sending greeting message to user {User} in guild {Guild}.", member.Id, member.Guild.Id);
		_logger.LogTrace("Message: {Message}", message);
		
		await member.SendMessageAsync(message, embed: embed);
	}
	
	/// <summary>
	/// Sends a greeting message to the user, in the channel specified in the guild configuration.
	/// </summary>
	/// <param name="member">The user to greet.</param>
	public async Task GreetUserGuildAsync(DiscordMember member)
	{
		// Get the guild configuration
		GuildScreeningConfig config = await _configService.GetGuildScreeningConfigAsync(member.Guild.Id);
		
		// If the greeting message template is not null, build a greeting message and send it to the user
		if (config.GreetingChannelMessage is not { } template)
		{
			_logger.LogDebug("Greeting message template is null or empty, not sending greeting message.");
			return;
		}

		// Build and send the greeting message/embed
		(string? message, DiscordEmbed? embed) = BuildMessageComponents(template, member);
		_logger.LogDebug("Sending greeting message to guild {Guild}.", member.Guild.Id);
		_logger.LogTrace("Message: {Message}", message);
		
		await member.Guild.GetDefaultChannel().SendMessageAsync(message, embed: embed);
	}
	
	/// <summary>
	/// Sends a screening rejection message to the user, in the user's Direct Messages.
	/// </summary>
	/// <param name="member">The user to reject.</param>
	/// <param name="reason">The reason for the rejection, if any.</param>
	public async Task RejectUserDmAsync(DiscordMember member, string? reason = null)
	{
		// Get the guild configuration
		GuildScreeningConfig config = await _configService.GetGuildScreeningConfigAsync(member.Guild.Id);
		
		// If the rejection message template is not null, build a rejection message and send it to the user
		if (config.ScreeningRejectedMessage is not { } template)
		{
			_logger.LogDebug("Rejection message template is null or empty, not sending rejection message.");
			return;
		}

		// Build and send the rejection message/embed
		(string? message, DiscordEmbed? embed) = BuildMessageComponents(template, member, new Dictionary<string, string> { { "reason", reason } });
		_logger.LogDebug("Sending rejection message to user {User} in guild {Guild}.", member.Id, member.Guild.Id);
		_logger.LogTrace("Message: {Message}", message);
		
		await member.SendMessageAsync(message, embed: embed);
	}

	/// <summary>
	/// Builds a greeting message for the user from the specified template and context items.
	/// </summary>
	/// <param name="template">The template to use.</param>
	/// <param name="member">The guild member to greet.</param>
	/// <param name="additionalContext">Additional context items to use in the template, if any.</param>
	/// <returns>The greeting message.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="template"/> or <paramref name="member"/> is null.</exception>
	private (string? message, DiscordEmbed? embed) BuildMessageComponents(
		MessageTemplate template, 
		DiscordMember member, 
		IReadOnlyDictionary<string, string>? additionalContext = null)
	{
		Dictionary<string, string> contextItems = new(additionalContext ?? new Dictionary<string, string>())
		{
			{ "userId", member.Id.ToString() },
			{ "userMention", member.Mention },
			{ "userName", member.Nickname }
		};

		string? message = null;
		DiscordEmbed? embed = null;

		if (template.Message is { Length: not 0 })
		{
			message = _stringTemplate.Fill(template.Message, contextItems);
		}

		if (template is { HasEmbed: true, EmbedTitle.Length: not 0 })
		{
			embed = new DiscordEmbedBuilder
			{
				Title = _stringTemplate.Fill(template.EmbedTitle, contextItems),
				Description = template.EmbedBody is { Length: not 0 } ? _stringTemplate.Fill(template.EmbedBody, contextItems) : "",

				Thumbnail = new()
				{
					Url = template.EmbedImageUri ?? member.AvatarUrl
				},

				Author = new()
				{
					Name = member.Guild.Name,
					IconUrl = member.Guild.IconUrl
				}
			};
		}

		return (message, embed);
	}
}