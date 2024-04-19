using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels.Admin;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class AdminViewElections
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<AdminViewElections> Localizer { get; set; }

    [Inject]
    public NavigationManager NavigationException { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    public List<AdminGetElectionResponse>? Elections { get; set; }

    public AdminGetElectionsRequest GetElectionsRequest { get; set; } = new();

    public int AdminId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");
    }

    private async Task FetchElections()
    {
        GetElectionsRequest.AdminId = AdminId;
        var response = await ApiRequestService.SendAsync<IEnumerable<AdminGetElectionResponse>>("Admin/GetElections", HttpMethod.Get, GetElectionsRequest);
        if (response.Error == null)
        {
            Elections = response.Data.ToList();
        }
        else
        {
            Errors.Add(response.Error);
        }
    }

    private void ClearSearch()
    {
        GetElectionsRequest = new();
        Elections = null;
    }

    private async Task DeleteTask(int electionId)
    {
        Errors.Clear();

        var response = await ApiRequestService.SendAsync<bool>("Admin/DeleteElection", HttpMethod.Delete, queryString: $"electionId={electionId}&adminId={AdminId}");
        if (response.Error == null)
        {
            NavigationException.NavigateTo("/admin-view-elections");
        }
        else
        {
            Errors.Add(response.Error);
        }

        await FetchElections();
    }
}
