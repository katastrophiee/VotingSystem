using VotingSystem.API.DTO.DbModels;

namespace VotingSystem.API.DTO.Responses;

public class GetVotingHistoryResponse
{
    public int Id { get; set; }

    public int ElectionId { get; set; }

    public string ElectionName { get; set; }

    public string ElectionDescription { get; set; }

    public DateTime VoteDate { get; set; }

    public GetVotingHistoryResponse(Vote vote)
    {
        Id = vote.Id;
        ElectionId = vote.ElectionId;
        ElectionName = vote.ElectionName;
        ElectionDescription = vote.ElectionDescription;
        VoteDate = vote.VoteDate;
    }

    public GetVotingHistoryResponse()
    {
    }
}
