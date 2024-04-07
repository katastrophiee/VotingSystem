using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.DbModels
{
    public class Vote
    {
        public int Id { get; set; }

        public int ElectionId { get; set; }

        public string ElectionName { get; set; }

        public string ElectionDescription { get; set; }

        public DateTime VoteDate { get; set; }

        public int CustomerId { get; set; }

        public CustomerCountry Country { get; set; }
    }
}
