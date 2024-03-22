using System.ComponentModel.DataAnnotations;

namespace VotingSystem.API.Enums;

public enum CustomerCountry
{
    [Display(Name = "United Kingdom")]
    UnitedKingdom = 1,

    Unknown = 255
}
