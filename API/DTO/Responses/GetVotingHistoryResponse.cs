using VotingSystem.API.DTO.DbResults;

namespace VotingSystem.API.DTO.Responses;

public class GetVotingHistoryResponse
{
    public int Id { get; set; }

    public int ElectionId { get; set; }

    public string ElectionName { get; set; }

    public DateTime VoteCast { get; set; }

    public GetVotingHistoryResponse(Vote vote)
    {
        Id = vote.Id;
        ElectionId = vote.ElectionId;
        ElectionName = vote.ElectionName;
    }

    public GetVotingHistoryResponse()
    {
    }
}
