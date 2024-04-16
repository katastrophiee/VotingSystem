using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Responses.Admin;

public class AdminGetAdminResponse
{
    public int AdminId { get; set; }

    public string DisplayName { get; set; }

    public string Email { get; set; }

    public UserCountry Country { get; set; }

    public DateTime? LastLoggedIn { get; set; }

    public bool IsActive { get; set; }

    public AdminGetAdminResponse()
    {
    }

    public AdminGetAdminResponse(DbModels.Admin.Admin admin)
    {
        AdminId = admin.Id;
        DisplayName = admin.DisplayName;
        Email = admin.Email;
        Country = admin.Country;
        LastLoggedIn = admin.LastLoggedIn;
        IsActive = admin.IsActive;
    }
}
