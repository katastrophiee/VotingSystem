using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Text.Json;
using VotingSystem.API.DTO.ComponentTypes;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Enums;
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

    public List<ElectionOptionWithState> ElectionOptionsChecked { get; set; } = [];

    public List<ElectionOptionWithRank> ElectionOptionsRanked { get; set; } = [];

    public int? SelectedOptionId { get; set; }

    public AddVoterVoteRequest AddVoterVoteRequest { get; set; } = new();

    public bool ShowNoOptionsRankedError { get; set; } = false;

    public bool ShowRankingError { get; set; } = false;

    public class ElectionOptionWithRank : ElectionOption
    {
        public int? Rank { get; set; }

        public ElectionOptionWithRank()
        {
        }

        public ElectionOptionWithRank(ElectionOption option)
        {
            OptionId = option.OptionId;
            OptionName = option.OptionName;
            OptionDescription = option.OptionDescription;
            ElectionId = option.ElectionId;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        VoterId = await _localStorage.GetItemAsync<int>("currentVoterId");

        var getElectionResponse = await ApiRequestService.SendAsync<GetElectionResponse>($"Election/GetElection", HttpMethod.Get, queryString: $"electionId={ElectionId}&voterId={VoterId}");
        if (getElectionResponse.Error == null)
        {
            Election = getElectionResponse.Data;

            switch (getElectionResponse.Data.ElectionType)
            {
                case ElectionType.GeneralElection_FPTP:
                case ElectionType.ParliamentaryElection_FPTP:
                case ElectionType.LocalGovernmentElection_FPTP:
                    ElectionOptionsChecked = getElectionResponse.Data.ElectionOptions.Select(o => new ElectionOptionWithState(o)).ToList();
                    break;
                case ElectionType.Election_STV:
                case ElectionType.Election_Preferential:
                    ElectionOptionsRanked = getElectionResponse.Data.ElectionOptions.Select(o => new ElectionOptionWithRank(o)).ToList();
                    break;
            }
        }
        else
            Errors.Add(getElectionResponse.Error);
    }

    private async Task HandleValidSubmit()
    {
        Errors.Clear();
        ShowNoOptionsRankedError = false;
        ShowRankingError = false;

        AddVoterVoteRequest.ElectionId = ElectionId;
        AddVoterVoteRequest.VoterId = VoterId;
        AddVoterVoteRequest.Country = Election.Country;

        switch (Election.ElectionType)
        {
            case ElectionType.GeneralElection_FPTP:
            case ElectionType.ParliamentaryElection_FPTP:
            case ElectionType.LocalGovernmentElection_FPTP:
                AddVoterVoteRequest.Choices = ElectionOptionsChecked.Where(o => o.OptionId == SelectedOptionId).Select(o => new ElectionOption(o)).ToList();
                AddVoterVoteRequest.ElectionTypeAdditionalInfo = "";
                break;
            case ElectionType.Election_STV:
            case ElectionType.Election_Preferential:
                var orderedRankedOptions = ElectionOptionsRanked.Where(o => o.Rank != null).OrderBy(o => o.Rank).ToList();
                if (orderedRankedOptions.Count() == 0)
                {
                    ShowNoOptionsRankedError = true;
                    return;
                }
                if (!ValidateRanking(orderedRankedOptions))
                {
                    ShowRankingError = true;
                    return;
                }

                AddVoterVoteRequest.Choices = orderedRankedOptions.Select(o => new ElectionOption(o)).ToList();

                var voteDetails = new VoteDetails
                {
                    ElectionType = Election.ElectionType,
                    Choices = AddVoterVoteRequest.Choices,
                    ElectionTypeAdditionalInfo = JsonSerializer.Serialize(orderedRankedOptions)
                };

                AddVoterVoteRequest.ElectionTypeAdditionalInfo = JsonSerializer.Serialize(voteDetails);
                break;
        }

        var addVoterVoteResponse = await ApiRequestService.SendAsync<int>("Vote/AddVoterVote", HttpMethod.Post, AddVoterVoteRequest);
        if (addVoterVoteResponse.Error == null)
        {
            NavigationManager.NavigateTo("/view-elections");
        }
        else
            Errors.Add(addVoterVoteResponse.Error);
    }

    private static bool ValidateRanking(List<ElectionOptionWithRank> options)
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].Rank != i + 1)
            {
                return false;
            }
        }
        return true;
    }
}
