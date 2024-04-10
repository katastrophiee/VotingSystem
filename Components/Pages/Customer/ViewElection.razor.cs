using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Customer;

public partial class ViewElection
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Parameter]
    public int ElectionId { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public static int CustomerId { get; set; }
    public GetElectionResponse Election { get; set; }

    public List<ElectionOptionWithState> ElectionOptions { get; set; }
    public AddCustomerVoteRequest AddCustomerVote { get; set; } = new();

    public class ElectionOptionWithState : ElectionOption
    {
        public bool IsChecked { get; set; }

        public ElectionOptionWithState()
        {
        }

        public ElectionOptionWithState(ElectionOption option)
        {
            OptionId = option.OptionId;
            OptionName = option.OptionName;
            OptionDescription = option.OptionDescription;
            ElectionId = option.ElectionId;
            IsChecked = false;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        CustomerId = await _localStorage.GetItemAsync<int>("currentUserId");

        var election = await ApiRequestService.GetElection(ElectionId, CustomerId);
        if (election.Error == null)
        {
            Election = election.Data;
            ElectionOptions = election.Data.ElectionOptions.Select(o => new ElectionOptionWithState(o)).ToList();
        }
        else
            Errors.Add(election.Error);
    }

    private async Task HandleValidSubmit()
    {
        AddCustomerVote.ElectionId = ElectionId;
        AddCustomerVote.CustomerId = CustomerId;
        AddCustomerVote.Country = Election.Country;
        AddCustomerVote.Choices = ElectionOptions.Where(o => o.IsChecked == true).Select(o => new ElectionOption(o)).ToList();
        AddCustomerVote.ElectionTypeAdditionalInfo = "";

        var response = await ApiRequestService.AddCustomerVote(AddCustomerVote);
        if (response.Error == null)
        {
            //Page needs adding
            NavigationManager.NavigateTo("/admin-view-elections");
        }
        else
            Errors.Add(response.Error);
    }
}
