using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Voter;

public partial class ViewCandidates
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<ViewCandidates> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public static int VoterId { get; set; }

    public GetVoterAccountDetailsResponse? VoterDetails { get; set; }

    public List<GetCandidateResponse> Candidates { get; set; }

    protected override async Task OnInitializedAsync()
    {
        VoterId = await _localStorage.GetItemAsync<int>("currentVoterId");

        var voterDetails = await ApiRequestService.SendAsync<GetVoterAccountDetailsResponse>($"Voter/GetVoterDetails?voterId={VoterId}", HttpMethod.Get);
        if (voterDetails.Error == null)
            VoterDetails = voterDetails.Data;
        else
            Errors.Add(voterDetails.Error);

        var candidates = await ApiRequestService.SendAsync<List<GetCandidateResponse>>($"Voter/GetActiveCandidates?voterId={VoterId}", HttpMethod.Get);
        if (candidates.Error == null)
            Candidates = candidates.Data;
        else
            Errors.Add(candidates.Error);
    }
}
