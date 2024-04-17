using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Voter;

public partial class ViewCandidate
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<ViewCandidate> Localizer { get; set; }

    [Parameter]
    public int CandidateId { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public static int VoterId { get; set; }

    public GetCandidateResponse? Candidate { get; set; }

    public List<GetElectionResponse> Elections { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        VoterId = await _localStorage.GetItemAsync<int>("currentVoterId");

        var candidate = await ApiRequestService.SendAsync<GetCandidateResponse>($"Voter/GetCandidate", HttpMethod.Get, queryString: $"voterId={VoterId}&candidateId={CandidateId}");
        if (candidate.Error == null)
            Candidate = candidate.Data;
        else
            Errors.Add(candidate.Error);

        var getElectionsRequest = new GetElectionsRequest(VoterId, Candidate is null ? [] : Candidate.OngoingEnteredElectionsIds);

        var elections = await ApiRequestService.SendAsync<List<GetElectionResponse>>("Election/PostGetElections", HttpMethod.Post, getElectionsRequest);
        if (elections.Error == null)
            Elections = elections.Data;
        else
            Errors.Add(elections.Error);
    }
}
