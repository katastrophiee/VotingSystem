using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Enums;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Voter;

public partial class Profile
{
    // https://stackoverflow.com/questions/51226405/net-core-blazor-app-how-to-pass-data-between-pages 
    [Parameter]
    public int UserId { get; set; }

    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public IStringLocalizer<Profile> Localizer { get; set; }

    public bool Editable { get; set; } = false;    
    public bool ShowUpdateButton { get; set; } = false;

    public GetVoterAccountDetailsResponse? VoterDetails { get; set; }
    public UpdateVoterProfileRequest UpdateVoterProfileRequest { get; set; } = new();
    public List<ErrorResponse> Errors { get; set; } = [];

    public InputFile UploadedIdDocumentFile;

    public Document? UploadedIdDocument { get; set; }

    public Document? CurrentIdDocument { get; set; } = null;

    protected override async Task OnInitializedAsync()
    {
        await FetchVoterDetails();
        await SetCurrentIdDocument();
    }

    private async Task HandleUpdateVoterProfile()
    {
        if (UpdateVoterProfileRequest is not null &&
            (UpdateVoterProfileRequest.Email != VoterDetails?.Email ||
            UpdateVoterProfileRequest.FirstName != VoterDetails?.FirstName ||
            UpdateVoterProfileRequest.LastName != VoterDetails?.LastName ||
            UpdateVoterProfileRequest.Address != VoterDetails?.Address ||
            UpdateVoterProfileRequest.DateOfBirth != VoterDetails?.DateOfBirth ||
            UpdateVoterProfileRequest.Country != VoterDetails?.Country ||
            UpdateVoterProfileRequest.Password is not null))
        {
            var response = await ApiRequestService.SendAsync<bool>("Voter/PutUpdateVoterProfile", HttpMethod.Put, UpdateVoterProfileRequest);
            if (response.Error != null)
            {
                if (VoterDetails is not null)
                    UpdateVoterProfileRequest = new(VoterDetails);

                Errors.Add(response.Error);
            }
            else
            {
                await FetchVoterDetails();
            }
        }
        Editable = false;
    }

    private async Task FetchVoterDetails()
    {
        var voterDetails = await ApiRequestService.SendAsync<GetVoterAccountDetailsResponse>($"Voter/GetVoterDetails", HttpMethod.Get, queryString: $"voterId={UserId}");
        if (voterDetails.Error == null)
        {
            //Trim as white space is being added
            voterDetails.Data.Email = voterDetails.Data.Email.Trim();
            voterDetails.Data.FirstName = voterDetails.Data.FirstName.Trim();
            voterDetails.Data.LastName = voterDetails.Data.LastName.Trim();
            VoterDetails = voterDetails.Data;
            UpdateVoterProfileRequest = new(VoterDetails);
        }
        else
        {
            Errors.Add(voterDetails.Error);
        }
    }

    private async Task ShowUploadButton(InputFileChangeEventArgs uploadedFile)
    {
        var file = uploadedFile.File;
        if (file != null)
        {
            using var memoryStream = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(memoryStream);

            var fileBytes = memoryStream.ToArray();

            var document = new Document()
            {
                VoterId = UserId,
                FileContent = fileBytes,
                FileName = file.Name,
                MimeType = file.ContentType,
                DocumentType = DocumentType.VoterIdentification
            };

            UploadedIdDocument = document;

            ShowUpdateButton = true;
        }
    }

    private async Task UploadIdentificationDocument()
    {

        var response = await ApiRequestService.SendAsync<int>("Document/PostUploadVoterDocument", HttpMethod.Post, UploadedIdDocument);

        if (response.Error != null)
            Errors.Add(response.Error);
        else
            await SetCurrentIdDocument();

        ShowUpdateButton = false;
    }

    private async Task SetCurrentIdDocument()
    {
        var document = await ApiRequestService.SendAsync<Document?>($"Document/GetCurrentVoterDocument", HttpMethod.Get, queryString: $"voterId={UserId}", isNullable: true);
        if (document.Error == null)
        {
            if (document.Data is not null)
                CurrentIdDocument = document.Data;
        }
        else
        {
            Errors.Add(document.Error);
        }
        StateHasChanged();
    }
}
