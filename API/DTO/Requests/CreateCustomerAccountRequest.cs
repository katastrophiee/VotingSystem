using System.ComponentModel.DataAnnotations;

namespace VotingSystem.API.DTO.Requests;

public class CreateCustomerAccountRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
