using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Customer;

public partial class Home
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage {  get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public static int CustomerId { get; set; }
    public GetCustomerAccountDetailsResponse? CustomerDetails { get; set; }
    public List<GetVotingHistoryResponse> VotingHistory { get; set; } = [];
    public List<GetOngoingElectionsResponse> OngoingElections { get; set; } = [];

    public bool IsAdmin { get; set; } = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            IsAdmin = await _localStorage.GetItemAsync<bool>("isAdmin");
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (!IsAdmin)
        {
            CustomerId = await _localStorage.GetItemAsync<int>("currentUserId");

            var customerDetails = await ApiRequestService.GetCustomerInfo(CustomerId);

            if (customerDetails.Error == null)
                CustomerDetails = customerDetails.Data;
            else
                Errors.Add(customerDetails.Error);

            var votingHistory = await ApiRequestService.GetCustomerVotingHistory(CustomerId);
            if (votingHistory.Error == null)
                VotingHistory = votingHistory.Data;
            else
                Errors.Add(votingHistory.Error);

            var ongoingElections = await ApiRequestService.GetCustomerOngoingElections(CustomerId);
            if (ongoingElections.Error == null)
                OngoingElections = ongoingElections.Data;
            else
                Errors.Add(ongoingElections.Error);
        }
    }
}
