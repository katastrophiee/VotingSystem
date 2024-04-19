using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Responses.Admin;

public class AdminGetElectionResponse
{
    public int ElectionId { get; set; }

    public string ElectionName { get; set; }

    public string ElectionDescription { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public UserCountry Country { get; set; }

    public ElectionType ElectionType { get; set; }

    public List<ElectionOption> ElectionOptions { get; set; }

    public AdminGetElectionResponse()
    {
    }

    public AdminGetElectionResponse(Election election)
    {
        ElectionId = election.Id;
        ElectionName = election.ElectionName;
        ElectionDescription = election.ElectionDescription;
        StartDate = election.StartDate;
        EndDate = election.EndDate;
        Country = election.Country;
        ElectionOptions = election.ElectionOptions;
        ElectionType = election.ElectionType;
    }
}
