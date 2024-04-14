using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace VotingSystem.Components.Layout;

public partial class NavMenu
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<NavMenu> Localizer { get; set; }

    public bool ShowAdminNavigation { get; set; } = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ShowAdminNavigation = await _localStorage.GetItemAsync<bool>("isAdmin");
            StateHasChanged();
        }
    }

    private async Task Logout()
    {
        await _localStorage.RemoveItemsAsync(["currentVoterId", "adminUserId", "authToken", "isAdmin"]);
        NavigationManager.NavigateTo("/");
    }
}
