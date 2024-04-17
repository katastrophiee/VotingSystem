using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.Services;
using Blazored.LocalStorage;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using Microsoft.Extensions.Localization;

namespace VotingSystem.Components.Pages.Admin;

public partial class ViewVoter
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IStringLocalizer<ViewVoter> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    [Parameter]
    public int UserId { get; set; }

    public int AdminId { get; set; }

    public AdminGetVoterResponse VoterDetails { get; set; }

    public AdminVerifyIdRequest VerifyIdRequest { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");

        await FetchVoterDetails();
    }

    private async Task VerifyId()
    {
        VerifyIdRequest.AdminId = AdminId;
        VerifyIdRequest.DocumentId = VoterDetails.CurrentIdDocument.Id;

        var verifyIdResult = await ApiRequestService.SendAsync<bool>("Admin/PostAdminVerifyId", HttpMethod.Post, VerifyIdRequest);

        if (verifyIdResult.Error != null)
            Errors.Add(verifyIdResult.Error);
        else
            await FetchVoterDetails();

        StateHasChanged();
    }

    private async Task FetchVoterDetails()
    {
        var voterDetails = await ApiRequestService.SendAsync<AdminGetVoterResponse>($"Admin/GetVoterDetails", HttpMethod.Get, queryString: $"voterId={UserId}&adminId={AdminId}");

        if (voterDetails.Error == null)
            VoterDetails = voterDetails.Data;
        else
            Errors.Add(voterDetails.Error);
    }
}

