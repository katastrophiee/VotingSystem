using Microsoft.AspNetCore.Identity;

namespace VotingSystem.API.DTO.DbModels;

public class Customer
{
    public int Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string PasswordSalt { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool NewUser { get; set; }

    public bool IsCandidate { get; set; }

    public bool IsActive { get; set; }

    public bool IsVerified { get; set; }

    public DateTime LastLoggedIn { get; set; }
}
