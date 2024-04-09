using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests;

public class AddCustomerVoteRequest
{
    public int ElectionId { get; set; }

    public int CustomerId { get; set; }

    public CustomerCountry Country { get; set; }

    public List<ElectionOption> Choices { get; set; }

    public string ElectionTypeAdditionalInfo { get; set; }
}
