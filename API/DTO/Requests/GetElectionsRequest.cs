namespace VotingSystem.API.DTO.Requests;

public class GetElectionsRequest
{
    public int VoterId { get; set; }

    public List<int> ElectionIds { get; set; }

    public GetElectionsRequest()
    {
    }

    public GetElectionsRequest(int voterId, List<int> electionIds)
    {
        VoterId = voterId;
        ElectionIds = electionIds;
    }
}
