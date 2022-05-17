using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace YumeChan.PluginSampleCS.Commands;

// Commands defined here can be stateless.
// This also means the IDE will easily be triggered to suggest turning your commands into static methods, which would break 'em. 
#pragma warning disable CA1822


/*
 * Defining PingCommandGroup within BaseCommandGroup
 * will result in all commands from the group to be prefixed by "==sample ping <command>".
 */
public partial class BaseCommandGroup
{
	/*
	 * This is a sample command group, with some commands.
	 *
	 * When defining a command group, your class must follow the following constraints:
	 *   - Must be public, non-static, and well defined
	 *   - Must inherit from BaseCommandGroup
	 *   - Must have a [Group("<group-name>")] attribute
	 *
	 * You can use any constructor parameters you want, following Dependency Injection (DI) conventions.
	 */
	[Group("ping"), Description("A series of Ping commands.")]
	public class PingCommandGroup : BaseCommandModule
	{
		/*
		 * When defining a command, your method must follow the following constraints:
		 *   - Must be public, non-static, and well defined
		 *   - Must have a [Command] attribute
		 *   - Must contain a first parameter of type CommandContext
		 */
		
		/// <summary>
		/// A simple ping command which will respond with "Pong!".
		/// </summary>
		/// <remarks>
		///	Invoked using "==sample ping"
		/// </remarks>
		[Command(""), Description("Pong!")]
		public async Task PingAsync(CommandContext ctx)
		{
			await ctx.RespondAsync("Pong!");
		}
		
		/// <summary>
		/// A simple echo command which will respond with the message sent.
		/// </summary>
		/// <remarks>
		/// Invoked using "==sample ping echo &lt;message&gt;"
		/// </remarks>
		[Command("echo"), Description("Echoes the message sent.")]
		public async Task EchoAsync(CommandContext ctx, [RemainingText] string message)
		{
			await ctx.RespondAsync(message);
		}
	}
}