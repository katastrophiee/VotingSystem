using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests.Admin;

public class AdminUpdateAdminRequest
{
    public int AdminId { get; set; }

    public int UpdatingAdminId { get; set; }

    public string? DisplayName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public UserCountry? Country { get; set; }

    public bool IsActive { get; set; }

    public AdminUpdateAdminRequest()
    {
    }

    public AdminUpdateAdminRequest(AdminGetAdminResponse adminResponse)
    {
        AdminId = adminResponse.AdminId;
        UpdatingAdminId = adminResponse.AdminId;
        DisplayName = adminResponse.DisplayName;
        Email = adminResponse.Email;
        Country = adminResponse.Country;
        IsActive = adminResponse.IsActive;
    }
}
