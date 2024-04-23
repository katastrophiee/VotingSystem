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

            // I used this when trying to force a re-render of the component
            //https://stackoverflow.com/questions/56839527/how-to-force-blazor-to-re-render-a-component

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
        if (userId != 0)
        {
            LoggedIn = true;
            UserId = userId;
            NavigationManager.NavigateTo("");
        }
    }
}
