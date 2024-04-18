using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class ViewVoters
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<ViewVoters> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    public int AdminId { get; set; }

    public List<AdminGetVotersResponse> VotersResult { get; set; } = [];

    public AdminGetVotersRequest GetVotersRequest { get; set; } = new();

    public string IsCandidateString { get; set; }
    public string IsVerifiedString { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");
    }

    private async Task SearchVoters()
    {
        GetVotersRequest.AdminId = AdminId;
        GetVotersRequest.IsCandidate = IsCandidateString.StringToNullableBool();
        GetVotersRequest.IsVerified = IsVerifiedString.StringToNullableBool();

        var votersResult = await ApiRequestService.SendAsync<List<AdminGetVotersResponse>>("Admin/PostGetVoters", HttpMethod.Post, GetVotersRequest);
        if (votersResult.Error == null)
        {
            VotersResult = votersResult.Data;
        }
        else
            Errors.Add(votersResult.Error);
    }

    private void ClearSearch()
    {
        GetVotersRequest = new();
        IsCandidateString = "";
        IsVerifiedString = "";
        VotersResult.Clear();
    }
}
