using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Customer;

public partial class ViewCandidates
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<ViewCandidates> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public static int CustomerId { get; set; }

    public GetCustomerAccountDetailsResponse? CustomerDetails { get; set; }

    public List<GetCandidateResponse> Candidates { get; set; }

    protected override async Task OnInitializedAsync()
    {
        CustomerId = await _localStorage.GetItemAsync<int>("currentUserId");

        var customerDetails = await ApiRequestService.GetCustomerInfo(CustomerId);
        if (customerDetails.Error == null)
            CustomerDetails = customerDetails.Data;
        else
            Errors.Add(customerDetails.Error);

        var candidates = await ApiRequestService.GetActiveCandidates(CustomerId);
        if (candidates.Error == null)
            Candidates = candidates.Data;
        else
            Errors.Add(candidates.Error);
    }
}
