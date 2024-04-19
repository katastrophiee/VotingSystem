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
    public IEnumerable<GetElectionResponse> VotedInElections { get; set; } = [];
    public IEnumerable<GetElectionResponse> OngoingElections { get; set; } = [];
    public IEnumerable<GetElectionResponse> UpcomingElections { get; set; } = [];
    public IEnumerable<GetElectionResponse> PreviousElections { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        VoterId = await _localStorage.GetItemAsync<int>("currentVoterId");

        var voterDetails = await ApiRequestService.SendAsync<GetVoterAccountDetailsResponse>($"Voter/GetVoterDetails", HttpMethod.Get, queryString: $"voterId={VoterId}");

        if (voterDetails.Error == null)
            VoterDetails = voterDetails.Data;
        else
            Errors.Add(voterDetails.Error);

        var votedInElections = await ApiRequestService.SendAsync<IEnumerable<GetElectionResponse>>($"Election/GetVoterVotedInElections", HttpMethod.Get, queryString: $"voterId={VoterId}");
        if (votedInElections.Error == null)
            VotedInElections = votedInElections.Data;
        else
            Errors.Add(votedInElections.Error);

        var ongoingElections = await ApiRequestService.SendAsync<IEnumerable<GetElectionResponse>>($"Election/GetVoterOngoingElections", HttpMethod.Get, queryString: $"voterId={VoterId}");
        if (ongoingElections.Error == null)
            OngoingElections = ongoingElections.Data;
        else
            Errors.Add(ongoingElections.Error);

        var upcomingElections = await ApiRequestService.SendAsync<IEnumerable<GetElectionResponse>>($"Election/GetVoterUpcomingElections", HttpMethod.Get, queryString: $"voterId={VoterId}");
        if (upcomingElections.Error == null)
            UpcomingElections = upcomingElections.Data;
        else
            Errors.Add(upcomingElections.Error);

        var previousElections = await ApiRequestService.SendAsync<IEnumerable<GetElectionResponse>>($"Election/GetRecentlyEndedElections", HttpMethod.Get, queryString: $"voterId={VoterId}");
        if (previousElections.Error == null)
            PreviousElections = previousElections.Data;
        else
            Errors.Add(previousElections.Error);
    }
}
