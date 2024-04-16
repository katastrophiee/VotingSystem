using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests.Admin;

public class AdminGetVotersRequest
{
    public int AdminId { get; set; }

    public int? VoterId { get; set; }

    public UserCountry? Country { get; set; }

    public bool? IsCandidate { get; set; } = null;

    public bool? IsVerified { get; set; } = null;
}
