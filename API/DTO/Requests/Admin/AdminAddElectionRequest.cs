using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests.Admin;

public class AdminAddElectionRequest
{
    public int AdminId { get; set; }

    public string ElectionName { get; set; }

    public string ElectionDescription { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public UserCountry Country { get; set; }

    public ElectionType ElectionType { get; set; }

    public List<ElectionOption> ElectionOptions { get; set; }

    public AdminAddElectionRequest()
    {
    }

    public AdminAddElectionRequest(List<ElectionOption> electionOptions)
    {
        ElectionOptions = electionOptions;
    }
}
