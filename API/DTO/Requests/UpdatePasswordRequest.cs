using System.ComponentModel.DataAnnotations;
using VotingSystem.API.DTO.ComponentTypes;

namespace VotingSystem.API.DTO.Requests;

public class UpdatePasswordRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    [PasswordRules]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}
