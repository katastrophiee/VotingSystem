using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class AdminProfile
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<AdminProfile> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    [Parameter]
    public int UserId { get; set; }

    public AdminGetAdminResponse? GetAdminResponse { get; set; }

    public AdminUpdateAdminRequest UpdateAdminRequest { get; set; } = new();

    public bool Editable { get; set; } = false;

    //TO DO
    //Check each page either does or doesnt show when errors occur, some should still show and others not

    protected override async Task OnInitializedAsync()
    {
        await FetchAdminDetails();
    }

    private async Task FetchAdminDetails()
    {
        var admin = await ApiRequestService.SendAsync<AdminGetAdminResponse>($"Admin/GetAdmin?currentAdminId={UserId}&requestedAdminId={UserId}", HttpMethod.Get);
        if (admin.Error == null)
        {
            //Trim as white space is being added
            admin.Data.Email = admin.Data.Email.Trim();
            admin.Data.DisplayName = admin.Data.DisplayName.Trim();
            GetAdminResponse = admin.Data;
            UpdateAdminRequest = new(GetAdminResponse);
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
            UpdateAdminRequest.Country != GetAdminResponse?.Country))
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
