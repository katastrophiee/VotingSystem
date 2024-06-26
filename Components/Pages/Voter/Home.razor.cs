﻿using Blazored.LocalStorage;
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
    public IEnumerable<GetVotingHistoryResponse> VotingHistory { get; set; } = [];
    public IEnumerable<GetElectionResponse> OngoingElections { get; set; } = [];

    public bool IsAdmin { get; set; } = false;

    public DateTime? IdExpireyDate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // I used this video to help with the addition of local storage
        // https://www.youtube.com/watch?v=jSs5DUHAvBc&t=390s

        VoterId = await _localStorage.GetItemAsync<int>("currentVoterId");
        IsAdmin = await _localStorage.GetItemAsync<bool>("isAdmin");

        if (!IsAdmin)
        {
            var voterDetails = await ApiRequestService.SendAsync<GetVoterAccountDetailsResponse>($"Voter/GetVoterDetails", HttpMethod.Get, queryString: $"voterId={VoterId}");
            if (voterDetails.Error == null)
                VoterDetails = voterDetails.Data;
            else
                Errors.Add(voterDetails.Error);

            var votingHistory = await ApiRequestService.SendAsync<IEnumerable<GetVotingHistoryResponse>>($"Vote/GetVoterVotingHistory", HttpMethod.Get, queryString: $"voterId={VoterId}");
            if (votingHistory.Error == null)
                VotingHistory = votingHistory.Data;
            else
                Errors.Add(votingHistory.Error);

            var ongoingElections = await ApiRequestService.SendAsync<IEnumerable<GetElectionResponse>>($"Election/GetVoterOngoingElections", HttpMethod.Get, queryString: $"voterId={VoterId}");
            if (ongoingElections.Error == null)
                OngoingElections = ongoingElections.Data;
            else
                Errors.Add(ongoingElections.Error);

            var currentIdDocument = await ApiRequestService.SendAsync<Document?>($"Document/GetCurrentVoterDocument", HttpMethod.Get, queryString: $"voterId={VoterId}", isNullable: true);
            if (currentIdDocument.Error == null)
            {
                if (currentIdDocument.Data is not null)
                    IdExpireyDate = currentIdDocument.Data.ExpiryDate;
            }
            else
            {
                Errors.Add(currentIdDocument.Error);
            }
        }
    }
}
