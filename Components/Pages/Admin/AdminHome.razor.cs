using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class AdminHome
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        
    }
}
