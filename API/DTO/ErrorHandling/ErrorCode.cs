using System.ComponentModel.DataAnnotations;

namespace VotingSystem.API.DTO.ErrorHandling;

public enum ErrorCode
{
    [Display(Name = "Internal server error", Description = "An internal server error occurred")]
    InternalServerError = 1,

    [Display(Name = "Customer not found", Description = "No customer was found with this ID")]
    CustomerNotFound = 2,

    [Display(Name = "No model passed in", Description = "No model was passed in")]
    NoModelPassedIn = 3
}
