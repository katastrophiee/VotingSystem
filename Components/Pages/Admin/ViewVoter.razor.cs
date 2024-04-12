using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.Services;
using Blazored.LocalStorage;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using Microsoft.Extensions.Localization;

namespace VotingSystem.Components.Pages.Admin;

public partial class ViewVoter
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IStringLocalizer<ViewVoter> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    [Parameter]
    public int UserId { get; set; }

    public int AdminId { get; set; }

    public AdminGetCustomerResponse CustomerDetails { get; set; }

    public AdminVerifyIdRequest VerifyIdRequest { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");

        await FetchCustomerDetails();
    }

    private async Task VerifyId()
    {
        VerifyIdRequest.AdminId = AdminId;
        VerifyIdRequest.DocumentId = CustomerDetails.CurrentIdDocument.Id;

        var verifyIdResult = await ApiRequestService.AdminVerifyCustomerIdDocument(VerifyIdRequest);

        if (verifyIdResult.Error != null)
            Errors.Add(verifyIdResult.Error);
        else
            await FetchCustomerDetails();

        StateHasChanged();
    }

    private async Task FetchCustomerDetails()
    {
        var customerDetails = await ApiRequestService.AdminGetCustomerDetails(UserId, AdminId);

        if (customerDetails.Error == null)
            CustomerDetails = customerDetails.Data;
        else
            Errors.Add(customerDetails.Error);
    }
}

