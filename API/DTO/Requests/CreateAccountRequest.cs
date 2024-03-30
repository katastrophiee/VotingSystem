using System.ComponentModel.DataAnnotations;

namespace VotingSystem.API.DTO.Requests;

public class CreateAccountRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
