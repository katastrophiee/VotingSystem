using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Responses.Admin
{
    public class AdminGetVotersResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public CustomerCountry Country { get; set; }

        public DateTime LastLoggedIn { get; set; }

        public bool NewUser { get; set; }

        public bool IsCandidate { get; set; }

        public bool IsVerified { get; set; }

        public bool IsActive { get; set; }

        public AdminGetVotersResponse()
        {
        }

        public AdminGetVotersResponse(Customer customer)
        {
            UserId = customer.Id;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Country = customer.Country;
            LastLoggedIn = customer.LastLoggedIn;
            NewUser = customer.NewUser;
            IsCandidate = customer.IsCandidate;
            IsVerified = customer.IsVerified;
            IsActive = customer.IsActive;
        }
    }
}
