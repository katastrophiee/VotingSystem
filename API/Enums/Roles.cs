using System.ComponentModel.DataAnnotations;

namespace VotingSystem.API.Enums;

/// <summary>
/// Roles enum used for storing in the database and ease with conversion to string, not used for UI so no localisation needed
/// </summary>
public enum Roles
{
    [Display(Name = "Voter")]
    Voter = 1,

    [Display(Name = "Candidate")]
    Candidate = 2,

    [Display(Name = "Admin")]
    Admin = 3,

    [Display(Name = "Observer")]
    Observer = 4
}
