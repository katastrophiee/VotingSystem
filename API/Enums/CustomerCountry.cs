using System.ComponentModel.DataAnnotations;

namespace VotingSystem.API.Enums;

public enum CustomerCountry
{
    //First past the post
    [Display(Name = "England")]
    England = 1,

    [Display(Name = "Wales")]
    Wales = 4,

    // Single transferable vote
    [Display(Name = "Northern Island")]
    NorthernIreland = 2,

    [Display(Name = "Scotland")]
    Scotland = 3,

    //Preferential voting
    [Display(Name = "Australia")]
    Australia = 5,    
    
    [Display(Name = "Ireland")]
    Ireland = 6,    
    
    [Display(Name = "United States")]
    UnitedStates = 7,

    // Misc
    Unknown = 255
}
