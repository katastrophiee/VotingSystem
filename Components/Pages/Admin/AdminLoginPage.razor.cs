﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
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

    [Inject]
    public IStringLocalizer<AdminLoginPage> Localizer { get; set; }

    [Inject]
    public IConfiguration Configuration { get; set; }

    public LoginRequest LoginRequest { get; set; }
    public LoginResponse LoginResult { get; set; }
    public List<ErrorResponse> Errors { get; set; } = [];

    // I used this when adding event callbacks 
    //https://stackoverflow.com/questions/76864163/subscribe-to-an-eventcallback-from-a-layout-when-using-router

    [Parameter]
    public EventCallback<int> OnLogin { get; set; }

    [Parameter]
    public EventCallback<bool> Return { get; set; }

    public bool CreateAccount { get; set; } = false;

    public bool ShowLoading { get; set; } = false;

    public bool ShowLoggingOutWarning { get; set; } = false;

    public string SelectedCulture = Thread.CurrentThread.CurrentCulture.Name;
    private Dictionary<string, string?> Cultures;

    protected override void OnInitialized()
    {
        LoginRequest = new();
        Cultures = Configuration.GetSection("Cultures").GetChildren().ToDictionary(x => x.Key, x => x.Value);
    }

    private async Task HandleLogin()
    {
        ShowLoading = true;
        Errors.Clear();
        var loginResponse = await ApiRequestService.SendAsync<LoginResponse>("Auth/PostAdminLogin", HttpMethod.Post, LoginRequest);
        if (loginResponse.Error == null)
        {
            LoginResult = loginResponse.Data;
            await _localStorage.RemoveItemsAsync(["currentVoterId", "authToken"]);

            await _localStorage.SetItemAsStringAsync("authToken", LoginResult.AccessToken);
            await _localStorage.SetItemAsync("adminUserId", LoginResult.UserId);
            await _localStorage.SetItemAsync("isAdmin", true);

            await OnLogin.InvokeAsync(LoginResult.UserId);
            StateHasChanged();
        }
        else
        {
            Errors.Add(loginResponse.Error);
            ShowLoading = false;
        }
    }

    private void ChangeCulture(string culture)
    {
        if (!string.IsNullOrEmpty(culture))
        {
            var uri = new Uri(NavigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);

            var query = $"?culture={Uri.EscapeDataString(SelectedCulture)}&redirectUri={Uri.EscapeDataString(uri)}";

            NavigationManager.NavigateTo($"api/Culture/SetCulture" + query, forceLoad: true);
        }
    }
}
