using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.Services;
using QRCoder;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.Components.Pages.Voter;

public partial class VoteInPerson
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<VoteInPerson> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    public int VoterId { get; set; }

    public List<int> VotedInElectionIds { get; set; } = [];

    public bool VotingEligibility { get; set; } = false;

    private string? qrImage;
    private int timeLeft = 10;
    private bool isTimerRunning = false;

    protected override async Task OnInitializedAsync()
    {
        VoterId = await _localStorage.GetItemAsync<int>("currentVoterId");

        var votingEligibility = await ApiRequestService.SendAsync<bool>($"Voter/GetInPersonVotingEligibility", HttpMethod.Get, queryString: $"voterId={VoterId}");
        if (votingEligibility.Error is null)
            VotingEligibility = votingEligibility.Data;
        else
            Errors.Add(votingEligibility.Error);

        var votedInElections = await ApiRequestService.SendAsync<List<GetElectionResponse>>($"Election/GetVoterVotedInElections", HttpMethod.Get, queryString: $"voterId={VoterId}");
        if (votedInElections.Error is null)
            VotedInElectionIds = votedInElections.Data.Select(v => v.ElectionId).ToList();
        else
            Errors.Add(votedInElections.Error);
    }

    private async Task GenerateQRCode()
    {
        //Used to make each QR code unique
        var randomNumber = new Random().Next(100000, 999999);

        //Used so that the QR code is only valid for 10 seconds
        var expireyDate = DateTime.UtcNow.AddSeconds(10);

        var qrCodeText = $"UserId: {VoterId}, VotedIn: {VotedInElectionIds}, RandomNumber: {randomNumber}, ExpiryDate: {expireyDate}";

        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);

        qrImage = "data:image/png;base64," + Convert.ToBase64String(qrCodeBytes);
        isTimerRunning = true;
        await StartTimer();
    }

    private async Task StartTimer()
    {
        while (timeLeft > 0 && isTimerRunning)
        {
            timeLeft--;
            await Task.Delay(1000);
            if (timeLeft == 0)
            {
                qrImage = null;
                isTimerRunning = false;
            }
            StateHasChanged();
        }
    }
}
