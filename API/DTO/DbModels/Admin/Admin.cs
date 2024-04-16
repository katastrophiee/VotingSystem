namespace VotingSystem.API.DTO.DbModels.Admin;

public class Admin : User
{
    public string DisplayName { get; set; }

    public DateTime? LastLoggedIn { get; set; }
}
