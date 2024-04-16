using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests.Admin;

public class AdminGetAdminRequest
{
    public int RequestingAdminId { get; set; }

    public int? AdminId { get; set; }

    public UserCountry? Country { get; set; }

    public bool? IsActive { get; set; }
}
