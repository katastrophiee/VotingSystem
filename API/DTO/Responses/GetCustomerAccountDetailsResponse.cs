using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Responses;

public class GetCustomerAccountDetailsResponse
{
    public int UserId { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public CustomerCountry Country { get; set; }

    public bool NewUser { get; set; }

    public bool IsVerified { get; set; }

    public bool IsCandidate { get; set; }

    public GetCustomerAccountDetailsResponse()
    {
    }

    public GetCustomerAccountDetailsResponse(Customer customer)
    {
        UserId = customer.Id;
        Email = customer.Email;
        FirstName = customer.FirstName;
        LastName = customer.LastName;
        Country = customer.Country;
        NewUser = customer.NewUser;
        IsVerified = customer.IsVerified;
        IsCandidate = customer.IsCandidate;
    }
}
