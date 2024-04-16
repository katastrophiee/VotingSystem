using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests;

public class AddVoterVoteRequest
{
    public int ElectionId { get; set; }

    public int VoterId { get; set; }

    public UserCountry Country { get; set; }

    public List<ElectionOption> Choices { get; set; }

    public string ElectionTypeAdditionalInfo { get; set; }
}
