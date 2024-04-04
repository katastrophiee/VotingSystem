using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Customer;

public partial class CustomerRegisterPage
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private CreateAccountRequest CreateAccountRequest = new();

    [Parameter]
    public EventCallback<CreateAccountRequest> OnCreation { get; set; }

    private async Task HandleRegistration()
    {
        await OnCreation.InvokeAsync(CreateAccountRequest);
    }
}
