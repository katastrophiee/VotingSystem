using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Voter;

public partial class VoterLoginPage
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<VoterLoginPage> Localizer { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IConfiguration Configuration { get; set; }

    public LoginRequest LoginRequest { get; set; }
    public LoginResponse LoginResult { get; set; }
    public List<ErrorResponse> Errors { get; set; } = [];

    [Parameter]
    public EventCallback<int> OnLogin { get; set; }

    public string SelectedCulture = Thread.CurrentThread.CurrentCulture.Name;
    private Dictionary<string, string?> Cultures;

    public bool CreateAccount { get; set; } = false;

    public bool ShowLoading { get; set; } = false;


    protected override void OnInitialized()
    {
        LoginRequest = new();
        Cultures = Configuration.GetSection("Cultures").GetChildren().ToDictionary(x => x.Key, x => x.Value);
    }

    private async Task HandleLogin()
    {
        ShowLoading = true;
        Errors.Clear();
        var loginResponse = await ApiRequestService.SendAsync<LoginResponse>("Auth/PostVoterLogin", HttpMethod.Post, LoginRequest);
        if (loginResponse.Error == null)
        {
            await HandleLoginResponse(loginResponse.Data);
        }
        else
        {
            Errors.Add(loginResponse.Error);
            ShowLoading = false;
        }
    }

    private async Task HandleRegister(CreateVoterAccountRequest request)
    {
        Errors.Clear();
        var createAccountResponse = await ApiRequestService.SendAsync<LoginResponse>("Auth/PostCreateVoterAccount", HttpMethod.Post, request);
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
        await _localStorage.RemoveItemsAsync(["adminUserId", "authToken"]);

        await _localStorage.SetItemAsStringAsync("authToken", response.AccessToken);
        await _localStorage.SetItemAsync("currentVoterId", response.UserId);
        await _localStorage.SetItemAsync("isAdmin", false);

        await OnLogin.InvokeAsync(response.UserId);
    }

    private void ChangeCulture(string culture)
    {
        if (!string.IsNullOrEmpty(culture))
        {
            var uri = new Uri(NavigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);

            var query = $"?culture={Uri.EscapeDataString(SelectedCulture)}&redirectUri={Uri.EscapeDataString(uri)}";

            //var authToken = await _localStorage.GetItemAsync<string>("authToken");
            //var isAdmin = await _localStorage.GetItemAsync<bool>("isAdmin");

            //var voterId = isAdmin 
            //    ? await _localStorage.GetItemAsync<int>("adminUserId") 
            //    : await _localStorage.GetItemAsync<int>("currentVoterId");

            NavigationManager.NavigateTo($"api/Culture/SetCulture" + query, forceLoad: true);

            //logs out the user for some reason
            //NavigationManager.NavigateTo("settings");
        }
    }
}
