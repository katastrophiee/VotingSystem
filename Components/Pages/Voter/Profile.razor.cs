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
    public UpdateVoterProfileRequest? UpdateVoterProfileRequest { get; set; }
    public List<ErrorResponse> Errors { get; set; } = [];

    public InputFile UploadedIdDocumentFile;

    public Document? UploadedIdDocument { get; set; }

    public Document? CurrentIdDocument { get; set; } = null;

    public async Task Update()
    {
        if (UpdateVoterProfileRequest is not null &&
            (UpdateVoterProfileRequest.Email != VoterDetails.Email ||
            UpdateVoterProfileRequest.FirstName != VoterDetails.FirstName ||
            UpdateVoterProfileRequest.LastName != VoterDetails.LastName ||
            UpdateVoterProfileRequest.Country != VoterDetails.Country))
        {
            var response = await ApiRequestService.SendAsync<bool>("Voter/PutUpdateVoterProfile", HttpMethod.Put, UpdateVoterProfileRequest);
            if (response.Error != null)
            {
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

    protected override async Task OnInitializedAsync()
    {
        await FetchVoterDetails();
        await SetCurrentIdDocument();
    }

    private async Task FetchVoterDetails()
    {
        var voterDetails = await ApiRequestService.SendAsync<GetVoterAccountDetailsResponse>($"Voter/GetVoterDetails?voterId={UserId}", HttpMethod.Get);
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

        var successfulUpload = await ApiRequestService.SendAsync<bool>("Document/PostUploadVoterDocument", HttpMethod.Post, UploadedIdDocument);

        if (successfulUpload.Error != null)
            Errors.Add(successfulUpload.Error);
        else
            await SetCurrentIdDocument();
        

        ShowUpdateButton = false;
    }

    private async Task SetCurrentIdDocument()
    {
        var document = await ApiRequestService.SendAsync<Document>($"Document/GetCurrentVoterDocument?voterId={UserId}", HttpMethod.Get);
        if (document.Error == null)
        {
            CurrentIdDocument = document.Data;

            // TO DO
            //get current id, check expirey date, option to upload new/different id if expires in 3 months
        }
        else
        {
            Errors.Add(document.Error);
        }
        StateHasChanged();
    }
}
