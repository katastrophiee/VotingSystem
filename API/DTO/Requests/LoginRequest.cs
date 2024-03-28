using System.ComponentModel.DataAnnotations;

namespace VotingSystem.API.DTO.Requests;

public class LoginRequest 
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}

public class InputModel()
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
