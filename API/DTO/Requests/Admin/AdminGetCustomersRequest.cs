using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests.Admin;

public class AdminGetCustomersRequest
{
    public int AdminId { get; set; }

    public int? UserId { get; set; }

    public CustomerCountry? Country { get; set; }

    public bool? IsCandidate { get; set; } = null;

    public bool? IsVerified { get; set; } = null;
}
