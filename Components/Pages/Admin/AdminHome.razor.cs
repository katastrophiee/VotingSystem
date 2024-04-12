using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class AdminHome
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<AdminHome> Localizer { get; set; }

    protected override async Task OnInitializedAsync()
    {
        
    }
}
