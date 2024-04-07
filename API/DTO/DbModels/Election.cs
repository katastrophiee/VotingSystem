using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.DbModels;

public class Election
{
    public int Id { get; set; }

    public string ElectionName { get; set; }

    public string ElectionDescription { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public CustomerCountry Country { get; set; }

    public List<ElectionOption> ElectionOptions { get; set; }

    public string ElectionResult { get; set; }

    public ElectionType ElectionType { get; set; }
}
