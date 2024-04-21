using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Responses.Admin;

public class AdminGetVoterResponse
{
    public int VoterId { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Address { get; set; }

    public DateTime DateOfBirth { get; set; }

    public UserCountry Country { get; set; }

    public bool NewUser { get; set; }

    public bool IsVerified { get; set; }

    public bool IsActive { get; set; }

    public Document CurrentIdDocument { get; set; }

    public bool IsCandidate { get; set; }

    public string? CandidateName { get; set; }

    public string? CandidateDescription { get; set; }

    public AdminGetVoterResponse()
    {
    }

    public AdminGetVoterResponse(Voter voter)
    {
        VoterId = voter.Id;
        Email = voter.Email;
        FirstName = voter.FirstName;
        LastName = voter.LastName;
        Address = voter.Address;
        DateOfBirth = voter.DateOfBirth;
        Country = voter.Country;
        NewUser = voter.NewUser;
        IsVerified = voter.IsVerified;
        IsActive = voter.IsActive;
        IsCandidate = voter.IsCandidate;
        CandidateName = voter.CandidateName;
        CandidateDescription = voter.CandidateDescription;
    }
}
