using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class ViewAdmin
{
    [Parameter]
    public int AdminId { get; set; }

    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<ViewAdmin> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    //TO DO
    //Move page properties to a seperate class

    public int CurrentAdminId { get; set; }

    public AdminGetAdminResponse? GetAdminResponse { get; set; }

    public AdminUpdateAdminRequest UpdateAdminRequest { get; set; } = new();

    public bool Editable { get; set; } = false;

    //TO DO
    //Check each page either does or doesnt show when errors occur, some should still show and others not

    protected override async Task OnInitializedAsync()
    {
        CurrentAdminId = await _localStorage.GetItemAsync<int>("adminUserId");

        await FetchAdminDetails();
    }

    private async Task FetchAdminDetails()
    {
        var admin = await ApiRequestService.SendAsync<AdminGetAdminResponse>($"Admin/GetAdmin", HttpMethod.Get, queryString: $"currentAdminId={CurrentAdminId}&requestedAdminId={AdminId}");
        if (admin.Error == null)
        {
            //Trim as white space is being added
            admin.Data.Email = admin.Data.Email.Trim();
            admin.Data.DisplayName = admin.Data.DisplayName.Trim();
            GetAdminResponse = admin.Data;
            UpdateAdminRequest = new(GetAdminResponse);
            StateHasChanged();
        }
        else
        {
            Errors.Add(admin.Error);
        }
    }

    public async Task Update()
    {
        if (UpdateAdminRequest is not null &&
            (UpdateAdminRequest.Email != GetAdminResponse?.Email ||
            UpdateAdminRequest.DisplayName != GetAdminResponse?.DisplayName ||
            UpdateAdminRequest.Country != GetAdminResponse?.Country ||
            UpdateAdminRequest.IsActive != GetAdminResponse?.IsActive))
        {
            var response = await ApiRequestService.SendAsync<bool>("Admin/PutUpdateAdmin", HttpMethod.Put, UpdateAdminRequest);
            if (response.Error != null)
            {
                if (GetAdminResponse is not null)
                    UpdateAdminRequest = new(GetAdminResponse);
                Errors.Add(response.Error);
            }
            else
            {
                await FetchAdminDetails();
            }
        }
        Editable = false;
    }
}
