using System.ComponentModel.DataAnnotations;

namespace VotingSystem.API.Enums;

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
