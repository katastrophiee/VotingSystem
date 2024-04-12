using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.Requests;

namespace VotingSystem.Components.Pages.Customer;

public partial class CustomerRegisterPage
{
    private CreateCustomerAccountRequest CreateAccountRequest = new();

    [Parameter]
    public EventCallback<CreateCustomerAccountRequest> OnCreation { get; set; }

    [Parameter]
    public EventCallback<bool> Return { get; set; }

    [Inject]
    public IStringLocalizer<CustomerRegisterPage> Localizer { get; set; }

    public bool ShowLoading { get; set; } = false;

    private async Task HandleRegistration()
    {
        ShowLoading = true;
        await OnCreation.InvokeAsync(CreateAccountRequest);
        ShowLoading = false;
    }
}
