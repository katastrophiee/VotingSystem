namespace VotingSystem.API.DTO.DbModels;

public class Voter : User
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Address { get; set; }

    public DateTime DateOfBirth { get; set; }

    public bool NewUser { get; set; }

    public bool IsCandidate { get; set; }

    public string? CandidateName { get; set; }

    public string? CandidateDescription { get; set; }

    public DateTime? DateOfCandidacy { get; set; }

    public bool IsVerified { get; set; }

    public DateTime LastLoggedIn { get; set; }
}
