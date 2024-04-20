using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class ViewAdmins
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<ViewAdmins> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    public int AdminId { get; set; }

    public List<AdminGetAdminResponse>? Admins { get; set; }
    public AdminGetAdminRequest GetAdminRequest { get; set; } = new();

    public string IsActiveString { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");
    }

    private async Task SearchAdmins()
    {
        GetAdminRequest.RequestingAdminId = AdminId;
        GetAdminRequest.IsActive = IsActiveString.StringToNullableBool();
        var admins = await ApiRequestService.SendAsync<IEnumerable<AdminGetAdminResponse>>("Admin/PostGetAdmins", HttpMethod.Post, GetAdminRequest);
        if (admins.Error == null)
            Admins = admins.Data.ToList();
        else
            Errors.Add(admins.Error);
    }

    private void ClearSearch()
    {
        GetAdminRequest = new();
        IsActiveString = "";
        Admins.Clear();
    }
}
