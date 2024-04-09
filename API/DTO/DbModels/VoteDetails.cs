using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.DbModels;

public class VoteDetails
{
    public int Id { get; set; }

    public int VoteId { get; set; }

    public ElectionType ElectionType { get; set; }

    public List<ElectionOption> Choices { get; set; }

    public string ElectionTypeAdditionalInfo { get; set; }
}
