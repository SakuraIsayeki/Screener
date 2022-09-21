namespace SakuraIsayeki.Screener.Data;

/// <summary>
/// Defines a series of possible measures taken while rejecting a user on screening.
/// </summary>
[Flags]
public enum ScreeningRejectActions : byte
{
	/// <summary>
	/// No action is taken.
	/// </summary>
	None = 0,
	
	/// <summary>
	/// Inform the user that they have been rejected.
	/// </summary>
	InformUser = 1,
	
	/// <summary>
	/// Kick the user from the guild upon rejection.
	/// </summary>
	Kick = 2,
	
	/// <summary>
	/// Ban the user from the guild upon rejection.
	/// </summary>
	Ban = 4,
	
	/// <summary>
	/// Remove guest roles from the user upon rejection.
	/// </summary>
	RemoveGuestRoles = 8
	
	
}