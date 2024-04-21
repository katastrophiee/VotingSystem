using VotingSystem.API.DTO.ComponentTypes;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests;

public class UpdateVoterProfileRequest
{
    public int VoterId { get; set; }

    public string Email { get; set; }

    public string Address { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public UserCountry? Country { get; set; }

    [PasswordRules]
    public string? Password { get; set; }

    public UpdateVoterProfileRequest()
    {
    }

    public UpdateVoterProfileRequest(GetVoterAccountDetailsResponse voterDetails)
    {
        VoterId = voterDetails.VoterId;
        Email = voterDetails.Email;
        Address = voterDetails.Address;
        FirstName = voterDetails.FirstName;
        LastName = voterDetails.LastName;
        Country = voterDetails.Country;
    }
}
