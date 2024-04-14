using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Voter;

public partial class Home
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage {  get; set; }

    [Inject]
    public IStringLocalizer<Home> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public static int VoterId { get; set; }
    public GetVoterAccountDetailsResponse? VoterDetails { get; set; }
    public List<GetVotingHistoryResponse> VotingHistory { get; set; } = [];
    public List<GetElectionResponse> OngoingElections { get; set; } = [];

    public bool IsAdmin { get; set; } = false;

    public DateTime? IdExpireyDate { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            IsAdmin = await _localStorage.GetItemAsync<bool>("isAdmin");
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (!IsAdmin)
        {
            VoterId = await _localStorage.GetItemAsync<int>("currentVoterId");

            var voterDetails = await ApiRequestService.SendAsync<GetVoterAccountDetailsResponse>($"Voter/GetVoterDetails?voterId={VoterId}", HttpMethod.Get);
            if (voterDetails.Error == null)
                VoterDetails = voterDetails.Data;
            else
                Errors.Add(voterDetails.Error);

            var votingHistory = await ApiRequestService.SendAsync<List<GetVotingHistoryResponse>>($"Vote/GetVoterVotingHistory?voterId={VoterId}", HttpMethod.Get);
            if (votingHistory.Error == null)
                VotingHistory = votingHistory.Data;
            else
                Errors.Add(votingHistory.Error);

            var ongoingElections = await ApiRequestService.SendAsync<List<GetElectionResponse>>($"Election/GetVoterOngoingElections?voterId={VoterId}", HttpMethod.Get);
            if (ongoingElections.Error == null)
                OngoingElections = ongoingElections.Data;
            else
                Errors.Add(ongoingElections.Error);

            var currentIdDocument = await ApiRequestService.SendAsync<Document>($"Document/GetCurrentVoterDocument?voterId={VoterId}", HttpMethod.Get);
            if (currentIdDocument.Error == null)
            {
                if (currentIdDocument.Data.Id != 0)
                    IdExpireyDate = currentIdDocument.Data.ExpiryDate;
            }
            else
            {
                Errors.Add(currentIdDocument.Error);
            }
        }
    }
}
