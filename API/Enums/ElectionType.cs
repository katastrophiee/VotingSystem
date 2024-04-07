using System.ComponentModel.DataAnnotations;

namespace VotingSystem.API.Enums;

public enum ElectionType
{
    [Display(Name = "General Election")]
    GeneralElection_FPTP = 1,

    [Display(Name = "Parliamentary Election")]
    ParliamentaryElection_FPTP = 2,

    [Display(Name = "Local Government Election")]
    LocalGovernmentElection_FPTP = 3,

    [Display(Name = "Election")]
    Election_STV = 4,

    [Display(Name = "Election")]
    Election_Preferential = 5,
}

