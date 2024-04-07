using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests;

public class UpdateCustomerProfileRequest
{
    public int UserId { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public CustomerCountry? Country { get; set; }

    public UpdateCustomerProfileRequest()
    {
    }

    public UpdateCustomerProfileRequest(GetCustomerAccountDetailsResponse customerDetails)
    {
        UserId = customerDetails.UserId;
        Email = customerDetails.Email;
        FirstName = customerDetails.FirstName;
        LastName = customerDetails.LastName;
        Country = customerDetails.Country;
    }
}
