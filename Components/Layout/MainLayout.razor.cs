using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace VotingSystem.Components.Layout;

public partial class MainLayout
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<MainLayout> Localizer { get; set; }

    private bool LoggedIn { get; set; } = false;
    private bool IsAdmin { get; set; } = false;
    private int UserId { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            var id = IsAdmin
                ? await _localStorage.GetItemAsync<int>("adminUserId")
                : await _localStorage.GetItemAsync<int>("currentVoterId");

            if (id != 0)
            {
                UserId = id;
                LoggedIn = true;
            }            
            else
            {
                LoggedIn = false;
            }

            StateHasChanged();
        }
        else
        {
            await _localStorage.ClearAsync();
            IsAdmin = false;
        }
    }

    private async Task HandleOnLogin(int userId)
    {
        //need to add enforcement for password structure
        if (userId != 0)
        {
            LoggedIn = true;
            UserId = userId;
            NavigationManager.NavigateTo("");
        }
    }
}
