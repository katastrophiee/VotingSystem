using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.DbModels;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string PasswordSalt { get; set; }

    public VoterCountry Country { get; set; }

    public DateTime LastLoggedIn { get; set; }

    public bool IsActive { get; set; }
}
