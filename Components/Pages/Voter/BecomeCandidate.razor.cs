using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Voter;

public partial class BecomeCandidate
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<BecomeCandidate> Localizer { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public static int VoterId { get; set; }

    public GetVoterAccountDetailsResponse? VoterDetails { get; set; }

    public BecomeCandidateRequest BecomeCandidateRequest { get; set; } = new();
    public UpdateCandidateRequest UpdateCandidateRequest { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        VoterId = await _localStorage.GetItemAsync<int>("currentVoterId");

        await GetVoterDetails();
    }

    private async Task GetCandidateDetails()
    {
        var candidateDetails = await ApiRequestService.SendAsync<GetCandidateResponse>($"Voter/GetCandidate", HttpMethod.Get, queryString: $"voterId={VoterId}&candidateId={VoterId}");
        if (candidateDetails.Error == null)
        {
            UpdateCandidateRequest.VoterId = VoterId;
            UpdateCandidateRequest.CandidateName = candidateDetails.Data.Name;
            UpdateCandidateRequest.CandidateDescription = candidateDetails.Data.Description;
            StateHasChanged();
        }
        else
        {
            Errors.Add(candidateDetails.Error);
        }
    }

    private async Task GetVoterDetails()
    {
        var voterDetails = await ApiRequestService.SendAsync<GetVoterAccountDetailsResponse>($"Voter/GetVoterDetails", HttpMethod.Get, queryString: $"voterId={VoterId}");
        if (voterDetails.Error == null)
        {
            //Trim as white space is being added to existing strings
            voterDetails.Data.FirstName = voterDetails.Data.FirstName.Trim();
            voterDetails.Data.LastName = voterDetails.Data.LastName.Trim();

            VoterDetails = voterDetails.Data;

            BecomeCandidateRequest.CandidateName = $"{VoterDetails.FirstName} {VoterDetails.LastName}";
            BecomeCandidateRequest.VoterId = VoterId;

            if (voterDetails.Data.IsCandidate)
                await GetCandidateDetails();

            StateHasChanged();
        }
        else
        {
            Errors.Add(voterDetails.Error);
        }
    }

    private async Task HandleValidSubmit()
    {
        var response = await ApiRequestService.SendAsync<bool>("Voter/PutMakeVoterACandidate", HttpMethod.Put, BecomeCandidateRequest);
        if (response.Error == null)
            await GetVoterDetails();
        else
            Errors.Add(response.Error);
    }

    private async Task HandleUpdateCandidate()
    {
        var response = await ApiRequestService.SendAsync<bool>("Voter/PutUpateCandidate", HttpMethod.Put, UpdateCandidateRequest);
        if (response.Error == null)
            await GetVoterDetails();
        else
            Errors.Add(response.Error);
    }

    private async Task WithdrawCandidacy()
    {
        var response = await ApiRequestService.SendAsync<bool>("Voter/PutRevokeCandidacy", HttpMethod.Put, queryString: $"candidateId={VoterId}");
        if (response.Error == null)
            await GetVoterDetails();
        else
            Errors.Add(response.Error);
    }
}
