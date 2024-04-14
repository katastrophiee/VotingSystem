using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests;

public class UpdateVoterProfileRequest
{
    public int VoterId { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public VoterCountry? Country { get; set; }

    public UpdateVoterProfileRequest()
    {
    }

    public UpdateVoterProfileRequest(GetVoterAccountDetailsResponse voterDetails)
    {
        VoterId = voterDetails.VoterId;
        Email = voterDetails.Email;
        FirstName = voterDetails.FirstName;
        LastName = voterDetails.LastName;
        Country = voterDetails.Country;
    }
}
