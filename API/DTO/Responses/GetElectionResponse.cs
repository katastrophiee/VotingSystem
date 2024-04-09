using VotingSystem.API.DTO.DbModels;

namespace VotingSystem.API.DTO.Responses;

public class GetElectionResponse
{
    public int ElectionId { get; set; }

    public string ElectionName { get; set; }

    public string ElectionDescription { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool HasVoted { get; set; }

    public GetElectionResponse()
    {
    }

    public GetElectionResponse(Election election)
    {
        ElectionId = election.Id;
        ElectionName = election.ElectionName;
        ElectionDescription = election.ElectionDescription;
        StartDate = election.StartDate;
        EndDate = election.EndDate;
    }
}
