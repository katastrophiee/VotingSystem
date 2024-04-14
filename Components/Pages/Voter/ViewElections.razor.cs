using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Voter;

public partial class ViewElections
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<ViewElections> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public static int VoterId { get; set; }

    public GetVoterAccountDetailsResponse? VoterDetails { get; set; } 
    public List<GetElectionResponse> VotedInElections { get; set; } = [];
    public List<GetElectionResponse> OngoingElections { get; set; } = [];
    public List<GetElectionResponse> UpcomingElections { get; set; } = [];
    public List<GetElectionResponse> PreviousElections { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        VoterId = await _localStorage.GetItemAsync<int>("currentVoterId");

        var voterDetails = await ApiRequestService.SendAsync<GetVoterAccountDetailsResponse>($"Voter/GetVoterDetails?voterId={VoterId}", HttpMethod.Get);

        if (voterDetails.Error == null)
            VoterDetails = voterDetails.Data;
        else
            Errors.Add(voterDetails.Error);

        var votedInElections = await ApiRequestService.SendAsync<List<GetElectionResponse>>($"Election/GetVoterVotedInElections?voterId={VoterId}", HttpMethod.Get);
        if (votedInElections.Error == null)
            VotedInElections = votedInElections.Data;
        else
            Errors.Add(votedInElections.Error);

        var ongoingElections = await ApiRequestService.SendAsync<List<GetElectionResponse>>($"Election/GetVoterOngoingElections?voterId={VoterId}", HttpMethod.Get);
        if (ongoingElections.Error == null)
            OngoingElections = ongoingElections.Data;
        else
            Errors.Add(ongoingElections.Error);

        var upcomingElections = await ApiRequestService.SendAsync<List<GetElectionResponse>>($"Election/GetVoterUpcomingElections?voterId={VoterId}", HttpMethod.Get);

        if (upcomingElections.Error == null)
            UpcomingElections = upcomingElections.Data;
        else
            Errors.Add(upcomingElections.Error);

        var previousElections = await ApiRequestService.SendAsync<List<GetElectionResponse>>($"Election/GetRecentlyEndedElections?voterId={VoterId}", HttpMethod.Get);
        if (previousElections.Error == null)
            PreviousElections = previousElections.Data;
        else
            Errors.Add(previousElections.Error);
    }
}
