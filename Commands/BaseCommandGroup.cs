using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace YumeChan.PluginSampleCS.Commands;


/// <summary>
/// Base command class for all commands in the "sample" group.
/// </summary>
/// <remarks>
///	Commands defined within this class will can be invoked using prefix "==sample &lt;command&gt;".
/// </remarks>
[Group("sample"), Description("Base prefix for all SamplePluginCS commands.")]
public partial class BaseCommandGroup : BaseCommandModule
{
 /*
  * This partial class is the baseline for all command groups.
  * You define a base class to nest commands inside the "sample" group,
  * but also allowing you to have further groups nested inside the "sample" group.
  *
  * Examples :
  *   ==sample ping <arguments>		[Command] (this command is contained in "==sample" prefix)
  *   ==sample config <command>		[Group] (any command in the group will be prefixed with "==sample config")
  *   ==sample config api <command>	[Group] (any command in the group will be prefixed with "==sample config api")
  *   ==sample debug <command>		[Group] (any command in the group will be prefixed with "==sample debug")
  *
  *
  * Any further nested groups (like "debug", "config" (and "api")) 
  * must also define descending partial classes, if they want to use separate files (for clarity).
  *
  * This would give you the following class structure :
  *   BaseCommandGroup.cs
  *   ├─ ConfigCommandGroup.cs
  *	 │  └─ ApiCommandGroup.cs
  *   └─ DebugCommandGroup.cs
  */	
}