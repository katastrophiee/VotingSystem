using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests.Admin;

public class AdminGetElectionsRequest
{
    public int AdminId { get; set; }

    public int? ElectionId { get; set; }

    public string? ElectionName { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public UserCountry? Country { get; set; }

    public ElectionType? ElectionType { get; set; }
}
