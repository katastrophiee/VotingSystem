using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels.Admin;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class AddAdmin
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public IStringLocalizer<AddAdmin> Localizer { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public AdminCreateAdminAccountRequest AddAdminRequest { get; set; } = new();

    public List<ErrorResponse> Errors { get; set; } = [];

    public int AdminId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");
    }

    public async Task HandleCreateAdminAccount()
    {
        AddAdminRequest.RequestingAdminId = AdminId;

        Errors.Clear();

        var response = await ApiRequestService.SendAsync<LoginResponse>("Auth/PostCreateAdminAccount", HttpMethod.Post, AddAdminRequest);

        if (response.Error == null)
        {
            NavigationManager.NavigateTo($"/view-admin/adminId={response.Data.UserId}");
        }
        else
            Errors.Add(response.Error);
    }
}
