namespace VotingSystem.API.DTO.Requests;

public class GetElectionsRequest
{
    public int CustomerId { get; set; }

    public List<int> ElectionIds { get; set; }

    public GetElectionsRequest()
    {
    }

    public GetElectionsRequest(int customerId, List<int> electionIds)
    {
        CustomerId = customerId;
        ElectionIds = electionIds;
    }
}
