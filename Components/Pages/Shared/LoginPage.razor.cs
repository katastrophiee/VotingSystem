using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Shared;

public partial class LoginPage : ComponentBase
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    public LoginRequest LoginRequest { get; set; }
    public LoginResponse LoginResult { get; set; }
    public List<ErrorResponse> Errors { get; set; } = [];

    [Parameter]
    public EventCallback<int> OnLogin { get; set; }

    protected override void OnInitialized()
    {
        LoginRequest = new();
    }

    private async Task HandleLogin()
    {
        var loginResponse = await ApiRequestService.PostCustomerLogin(LoginRequest);
        if (loginResponse.Error == null)
        {
            LoginResult = loginResponse.Data;
            await _localStorage.SetItemAsStringAsync("authToken", loginResponse.Data.AccessToken);
            await _localStorage.SetItemAsync("currentUserId", loginResponse.Data.UserId);
            await OnLogin.InvokeAsync(loginResponse.Data.UserId);
        }
        else
        {
            Errors.Add(loginResponse.Error);
        }
    }
}
