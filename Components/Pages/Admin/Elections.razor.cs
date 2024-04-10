using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class Elections
{
    private AddElectionRequest AddElectionRequest = new([])
    {
        StartDate = DateTime.Now,
        EndDate = DateTime.Now
    };

    private ElectionOption NewOption = new();

    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    public int AdminId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");
    }

    private async Task HandleValidSubmit()
    {
        AddElectionRequest.AdminId = AdminId;
        var response = await ApiRequestService.AdminPostAddElection(AddElectionRequest);

        if (response.Error == null)
        {
            //redirect to view elections page
        }
        else
            Errors.Add(response.Error);
    }


    private void AddOption()
    {
        AddElectionRequest.ElectionOptions.Add(NewOption);
        NewOption = new();
    }

    private void RemoveOption(ElectionOption option)
    {
        AddElectionRequest.ElectionOptions.Remove(option);
    }
}
