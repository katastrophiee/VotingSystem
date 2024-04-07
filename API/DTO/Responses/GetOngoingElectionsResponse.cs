using VotingSystem.API.DTO.DbModels;

namespace VotingSystem.API.DTO.Responses;

public class GetOngoingElectionsResponse
{
    public int ElectionId { get; set; }

    public string ElectionName { get; set; }

    public string ElectionDescription { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool HasVoted { get; set; }

    public GetOngoingElectionsResponse()
    {
    }

    public GetOngoingElectionsResponse(Election election)
    {
        ElectionId = election.Id;
        ElectionName = election.ElectionName;
        ElectionDescription = election.ElectionDescription;
        StartDate = election.StartDate;
        EndDate = election.EndDate;
    }
}
