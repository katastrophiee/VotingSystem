using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Shared;

public partial class Settings
{

    [Inject]
    NavigationManager NavigationManager { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IConfiguration Configuration { get; set; }

    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public IStringLocalizer<Settings> Localizer { get; set; }

    public string SelectedCulture = Thread.CurrentThread.CurrentCulture.Name;
    private Dictionary<string, string?> Cultures;


    protected override void OnInitialized()
    {
        Cultures = Configuration.GetSection("Cultures").GetChildren().ToDictionary(x => x.Key, x => x.Value);
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
