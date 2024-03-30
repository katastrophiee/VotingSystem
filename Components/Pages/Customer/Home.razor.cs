using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Customer;

public partial class Home : ComponentBase
{
    [Inject]
    public ICustomerService CustomerService { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public static LoginResponse LoginResult { get; } = new LoginResponse { UserId = 1 };
    public GetCustomerAccountDetailsResponse? CustomerDetails { get; set; }
    public List<GetVotingHistoryResponse> VotingHistory { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var customerDetails = await CustomerService.GetCustomerInfo(LoginResult.UserId);

        if (customerDetails.Error == null)
            CustomerDetails = customerDetails.Data;
        else
            Errors.Add(customerDetails.Error);

        var votingHistory = await CustomerService.GetCustomerVotingHistory(LoginResult.UserId);
        if (votingHistory.Error == null)
            VotingHistory = votingHistory.Data;
        else
            Errors.Add(votingHistory.Error);
    }
}
