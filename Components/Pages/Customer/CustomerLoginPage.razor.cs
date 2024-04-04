using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Customer;

public partial class CustomerLoginPage
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public LoginRequest LoginRequest { get; set; }
    public LoginResponse LoginResult { get; set; }
    public List<ErrorResponse> Errors { get; set; } = [];

    [Parameter]
    public EventCallback<int> OnLogin { get; set; }

    public bool CreateAccount { get; set; } = false;

    protected override void OnInitialized()
    {
        LoginRequest = new();
    }

    private async Task HandleLogin()
    {
        Errors.Clear();
        var loginResponse = await ApiRequestService.PostCustomerLogin(LoginRequest);
        if (loginResponse.Error == null)
        {
            await HandleLoginResponse(loginResponse.Data);
        }
        else
        {
            Errors.Add(loginResponse.Error);
        }
    }

    private async Task HandleRegister(CreateAccountRequest request)
    {
        Errors.Clear();
        var createAccountResponse = await ApiRequestService.PostCreateCustomerAccount(request);
        if (createAccountResponse.Data is not null)
        {
            await HandleLoginResponse(createAccountResponse.Data);
        }
        else
        {
            Errors.Add(createAccountResponse.Error);
        }
    }

    private async Task HandleLoginResponse(LoginResponse response)
    {
        LoginResult = response;
        await _localStorage.SetItemAsStringAsync("authToken", response.AccessToken);
        await _localStorage.SetItemAsync("currentUserId", response.UserId);
        await OnLogin.InvokeAsync(response.UserId);
    }
}
