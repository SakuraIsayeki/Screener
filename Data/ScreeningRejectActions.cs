namespace SakuraIsayeki.Screener.Data;

/// <summary>
/// Defines a series of possible measures taken while rejecting a user on screening.
/// </summary>
[Flags]
public enum ScreeningRejectActions
{
	/// <summary>
	/// Inform the user that they have been rejected.
	/// </summary>
	InformUser,
	
	/// <summary>
	/// Kick the user from the guild upon rejection.
	/// </summary>
	Kick,
	
	/// <summary>
	/// Ban the user from the guild upon rejection.
	/// </summary>
	Ban,
	
	/// <summary>
	/// Remove guest roles from the user upon rejection.
	/// </summary>
	RemoveGuestRoles
	
	
}