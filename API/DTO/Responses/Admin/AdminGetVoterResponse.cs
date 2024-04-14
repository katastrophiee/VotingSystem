using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Responses.Admin;

public class AdminGetVoterResponse
{
    public int VoterId { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public VoterCountry Country { get; set; }

    public bool NewUser { get; set; }

    public bool IsVerified { get; set; }

    public Document CurrentIdDocument { get; set; }

    public AdminGetVoterResponse()
    {
    }

    public AdminGetVoterResponse(Voter voter)
    {
        VoterId = voter.Id;
        Email = voter.Email;
        FirstName = voter.FirstName;
        LastName = voter.LastName;
        Country = voter.Country;
        NewUser = voter.NewUser;
        IsVerified = voter.IsVerified;
    }
}
