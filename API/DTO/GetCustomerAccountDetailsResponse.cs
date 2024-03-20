using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO;

public class GetCustomerAccountDetailsResponse
{
    public int UserId { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public CustomerCountry Country { get; set; }

    public bool NewUser { get; set; }
}
