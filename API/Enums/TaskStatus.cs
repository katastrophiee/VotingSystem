using System.ComponentModel.DataAnnotations;

namespace VotingSystem.API.Enums;

public enum TaskStatus
{
    [Display(Name = "New")]
    New = 0,

    [Display(Name = "In Progress")]
    InProgress = 1,

    [Display(Name = "On Hold")]
    OnHold = 2,

    [Display(Name = "Completed")]
    Completed = 3,

    [Display(Name = "Closed")]
    Closed = 4
}
