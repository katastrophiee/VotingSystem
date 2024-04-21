using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Responses;

public class GetVoterAccountDetailsResponse
{
    public int VoterId { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public UserCountry Country { get; set; }

    public bool NewUser { get; set; }

    public bool IsVerified { get; set; }

    public bool IsCandidate { get; set; }

    public GetVoterAccountDetailsResponse()
    {
    }

    public GetVoterAccountDetailsResponse(Voter voter)
    {
        VoterId = voter.Id;
        Email = voter.Email;
        Address = voter.Address;
        FirstName = voter.FirstName;
        LastName = voter.LastName;
        Country = voter.Country;
        NewUser = voter.NewUser;
        IsVerified = voter.IsVerified;
        IsCandidate = voter.IsCandidate;
    }
}
