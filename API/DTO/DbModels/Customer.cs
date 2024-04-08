namespace VotingSystem.API.DTO.DbModels;

public class Customer : User
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool NewUser { get; set; }

    public bool IsCandidate { get; set; }

    public bool IsVerified { get; set; }
}
