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
            {
                var candidateDetails = await ApiRequestService.SendAsync<GetCandidateResponse>($"Voter/GetCandidate", HttpMethod.Get, queryString: $"voterId={VoterId}&candidateId={VoterId}");
                if (candidateDetails.Error == null)
                {
                    UpdateCandidateRequest.VoterId = VoterId;
                    UpdateCandidateRequest.CandidateName = candidateDetails.Data.Name;
                    UpdateCandidateRequest.CandidateDescription = candidateDetails.Data.Description;
                }
                else
                {
                    Errors.Add(candidateDetails.Error);
                }
            }
        }
        else
        {
            Errors.Add(voterDetails.Error);
        }
    }

    public async Task HandleValidSubmit()
    {
        var response = await ApiRequestService.SendAsync<bool>("Voter/PutMakeVoterACandidate", HttpMethod.Put, BecomeCandidateRequest);

        if (response.Error == null)
            NavigationManager.NavigateTo("/view-candidate");
        else
            Errors.Add(response.Error);
    }

    public async Task HandleUpdateCandidate()
    {
        var response = await ApiRequestService.SendAsync<bool>("Voter/PutUpateCandidate", HttpMethod.Put, UpdateCandidateRequest);

        if (response.Error == null)
            NavigationManager.NavigateTo($"/view-candidate/userId={VoterId}");
        else
            Errors.Add(response.Error);
    }
}
