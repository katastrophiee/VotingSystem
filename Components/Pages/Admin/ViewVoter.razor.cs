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

    public AdminUpdateVoterDetailsRequest UpdateVoterDetailsRequest { get; set; } = new();

    public bool IsEditable { get; set; } = false;

    public string NewUserString { get; set; } = "";

    public string IsVerifiedString { get; set; } = "";

    public string IsCandidateString { get; set; } = "";

    public string IsActiveString { get; set; } = "";


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
        Errors.Clear();

        var voterDetails = await ApiRequestService.SendAsync<AdminGetVoterResponse>($"Admin/GetVoterDetails", HttpMethod.Get, queryString: $"voterId={UserId}&adminId={AdminId}");
        if (voterDetails.Error == null)
        {
            voterDetails.Data.Email = voterDetails.Data.Email.Trim();
            voterDetails.Data.FirstName = voterDetails.Data.FirstName.Trim();
            voterDetails.Data.LastName = voterDetails.Data.LastName.Trim();

            VoterDetails = voterDetails.Data;
            UpdateVoterDetailsRequest = new(voterDetails.Data);

            //Convert to lower case to match the checkbox values
            NewUserString = voterDetails.Data.NewUser.ToString().ToLower();
            IsVerifiedString = voterDetails.Data.IsVerified.ToString().ToLower();
            IsCandidateString = voterDetails.Data.IsCandidate.ToString().ToLower();
            IsActiveString = voterDetails.Data.IsActive.ToString().ToLower();
        }
        else
            Errors.Add(voterDetails.Error);
    }

    private async Task UpdateVoterDetails()
    {
        Errors.Clear();

        UpdateVoterDetailsRequest.VoterId = UserId;
        UpdateVoterDetailsRequest.AdminId = AdminId;
        UpdateVoterDetailsRequest.NewUser = NewUserString.StringToNullableBool();
        UpdateVoterDetailsRequest.IsVerified = IsVerifiedString.StringToNullableBool();
        UpdateVoterDetailsRequest.IsCandidate = IsCandidateString.StringToNullableBool();
        UpdateVoterDetailsRequest.IsActive = IsActiveString.StringToNullableBool();

        var response = await ApiRequestService.SendAsync<bool>($"Admin/PutUpdateVoterDetails", HttpMethod.Put, UpdateVoterDetailsRequest);
        if (response.Error == null)
        {
            IsEditable = false;
            await FetchVoterDetails();
        }
        else
            Errors.Add(response.Error);
    }
}

