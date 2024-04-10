using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Customer;

public partial class ViewElections
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public static int CustomerId { get; set; }

    public GetCustomerAccountDetailsResponse? CustomerDetails { get; set; } 
    public List<GetElectionResponse> VotedInElections { get; set; } = [];
    public List<GetElectionResponse> OngoingElections { get; set; } = [];
    public List<GetElectionResponse> UpcomingElections { get; set; } = [];
    public List<GetElectionResponse> PreviousElections { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        CustomerId = await _localStorage.GetItemAsync<int>("currentUserId");

        var customerDetails = await ApiRequestService.GetCustomerInfo(CustomerId);

        if (customerDetails.Error == null)
            CustomerDetails = customerDetails.Data;
        else
            Errors.Add(customerDetails.Error);

        var votedInElections = await ApiRequestService.GetCustomerVotedInElections(CustomerId);
        if (votedInElections.Error == null)
            VotedInElections = votedInElections.Data;
        else
            Errors.Add(votedInElections.Error);

        var ongoingElections = await ApiRequestService.GetCustomerOngoingElections(CustomerId);
        if (ongoingElections.Error == null)
            OngoingElections = ongoingElections.Data;
        else
            Errors.Add(ongoingElections.Error);

        var upcomingElections = await ApiRequestService.GetCustomerUpcomingElections(CustomerId);
        if (upcomingElections.Error == null)
            UpcomingElections = upcomingElections.Data;
        else
            Errors.Add(ongoingElections.Error);

        var previousElections = await ApiRequestService.GetRecentlyEndedElections(CustomerId);
        if (previousElections.Error == null)
            PreviousElections = previousElections.Data;
        else
            Errors.Add(previousElections.Error);
    }
}
