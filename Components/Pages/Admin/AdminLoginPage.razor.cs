﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class AdminLoginPage
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

    [Parameter]
    public EventCallback<bool> Return { get; set; }

    public bool CreateAccount { get; set; } = false;

    protected override void OnInitialized()
    {
        LoginRequest = new();
    }

    private async Task HandleLogin()
    {
        Errors.Clear();
        var loginResponse = await ApiRequestService.PostAdminLogin(LoginRequest);
        if (loginResponse.Error == null)
        {
            LoginResult = loginResponse.Data;
            await _localStorage.RemoveItemsAsync(["currentUserId", "authToken"]);

            await _localStorage.SetItemAsStringAsync("authToken", LoginResult.AccessToken);
            await _localStorage.SetItemAsync("adminUserId", LoginResult.UserId);
            await _localStorage.SetItemAsync("isAdmin", true);

            await OnLogin.InvokeAsync(LoginResult.UserId);

            StateHasChanged();
        }
        else
        {
            Errors.Add(loginResponse.Error);
        }
    }
}