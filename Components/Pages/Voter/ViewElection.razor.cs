using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ComponentTypes;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Voter;

public partial class ViewElection
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IStringLocalizer<ViewElection> Localizer { get; set; }

    [Parameter]
    public int ElectionId { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public static int VoterId { get; set; }
    public GetElectionResponse Election { get; set; }

    public List<ElectionOptionWithState> ElectionOptions { get; set; } = [];
    public AddVoterVoteRequest AddVoterVoteRequest { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        VoterId = await _localStorage.GetItemAsync<int>("currentVoterId");

        var getElectionResponse = await ApiRequestService.SendAsync<GetElectionResponse>($"Election/GetElection", HttpMethod.Get, queryString: $"electionId={ElectionId}&voterId={VoterId}");
        if (getElectionResponse.Error == null)
        {
            Election = getElectionResponse.Data;
            ElectionOptions = getElectionResponse.Data.ElectionOptions.Select(o => new ElectionOptionWithState(o)).ToList();
        }
        else
            Errors.Add(getElectionResponse.Error);
    }

    private async Task HandleValidSubmit()
    {
        AddVoterVoteRequest.ElectionId = ElectionId;
        AddVoterVoteRequest.VoterId = VoterId;
        AddVoterVoteRequest.Country = Election.Country;
        AddVoterVoteRequest.Choices = ElectionOptions.Where(o => o.IsChecked == true).Select(o => new ElectionOption(o)).ToList();
        AddVoterVoteRequest.ElectionTypeAdditionalInfo = "";

        var addVoterVoteResponse = await ApiRequestService.SendAsync<int>("Vote/AddVoterVote", HttpMethod.Post, AddVoterVoteRequest);
        if (addVoterVoteResponse.Error == null)
        {
            NavigationManager.NavigateTo("/view-elections");
        }
        else
            Errors.Add(addVoterVoteResponse.Error);
    }
}
