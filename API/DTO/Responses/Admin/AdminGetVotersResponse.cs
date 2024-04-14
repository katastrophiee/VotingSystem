using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Responses.Admin
{
    public class AdminGetVotersResponse
    {
        public int VoterId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public VoterCountry Country { get; set; }

        public DateTime LastLoggedIn { get; set; }

        public bool NewUser { get; set; }

        public bool IsCandidate { get; set; }

        public bool IsVerified { get; set; }

        public bool IsActive { get; set; }

        public AdminGetVotersResponse()
        {
        }

        public AdminGetVotersResponse(Voter voter)
        {
            VoterId = voter.Id;
            FirstName = voter.FirstName;
            LastName = voter.LastName;
            Country = voter.Country;
            LastLoggedIn = voter.LastLoggedIn;
            NewUser = voter.NewUser;
            IsCandidate = voter.IsCandidate;
            IsVerified = voter.IsVerified;
            IsActive = voter.IsActive;
        }
    }
}
