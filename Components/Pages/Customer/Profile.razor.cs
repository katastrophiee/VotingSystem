using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Enums;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Customer;

public partial class Profile
{
    // https://stackoverflow.com/questions/51226405/net-core-blazor-app-how-to-pass-data-between-pages 
    [Parameter]
    public int UserId { get; set; }

    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    public bool Editable { get; set; } = false;    
    public bool ShowUpdateButton { get; set; } = false;

    public GetCustomerAccountDetailsResponse? CustomerDetails { get; set; }
    public UpdateCustomerProfileRequest? UpdateCustomerProfileRequest { get; set; }
    public List<ErrorResponse> Errors { get; set; } = [];

    public InputFile UploadedIdDocumentFile;

    public Document? UploadedIdDocument { get; set; }

    public Document? CurrentIdDocument { get; set; } = null;

    public async Task Update()
    {
        if (UpdateCustomerProfileRequest.Email != CustomerDetails.Email ||
            UpdateCustomerProfileRequest.FirstName != CustomerDetails.FirstName ||
            UpdateCustomerProfileRequest.LastName != CustomerDetails.LastName ||
            UpdateCustomerProfileRequest.Country != CustomerDetails.Country)
        {
            var response = await ApiRequestService.PutUpdateCustomerProfile(UpdateCustomerProfileRequest);
            if (response.Error != null)
            {
                UpdateCustomerProfileRequest = new(CustomerDetails);
                Errors.Add(response.Error);
            }
            else
            {
                await FetchCustomerDetails();
            }
        }
        Editable = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await FetchCustomerDetails();
        await SetCurrentIdDocument();
    }

    private async Task FetchCustomerDetails()
    {
        var customerDetails = await ApiRequestService.GetCustomerInfo(UserId);
        if (customerDetails.Error == null)
        {
            //Trim as white space is being added
            customerDetails.Data.Email = customerDetails.Data.Email.Trim();
            customerDetails.Data.FirstName = customerDetails.Data.FirstName.Trim();
            customerDetails.Data.LastName = customerDetails.Data.LastName.Trim();
            CustomerDetails = customerDetails.Data;
            UpdateCustomerProfileRequest = new(CustomerDetails);
        }
        else
        {
            Errors.Add(customerDetails.Error);
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
                CustomerId = UserId,
                FileContent = fileBytes,
                FileName = file.Name,
                MimeType = file.ContentType,
                DocumentType = DocumentType.CustomerIdentification
            };

            UploadedIdDocument = document;

            ShowUpdateButton = true;
        }
    }

    private async Task UploadIdentificationDocument()
    {
        try
        {
            var successfulUpload = await ApiRequestService.PostUploadCustomerDocument(UploadedIdDocument);

            if (successfulUpload.Error != null)
                Errors.Add(successfulUpload.Error);

            ShowUpdateButton = false;

            await SetCurrentIdDocument();
        }
        catch (Exception ex)
        {
            Errors.Add(new()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to upload Id",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    private async Task SetCurrentIdDocument()
    {
        var documents = await ApiRequestService.GetCustomerDocuments(UserId);
        if (documents.Error == null)
        {
            var currentIdDocument = documents.Data?.Where(d =>
                d.MostRecentId == true &&
                d.DocumentType == DocumentType.CustomerIdentification)
                .FirstOrDefault();

            if (currentIdDocument is not null)
            {
                CurrentIdDocument = currentIdDocument;
            }

            // TO DO
            //get current id, check expirey date, option to upload new/different id if expires in 3 months
        }
        else
        {
            Errors.Add(documents.Error);
        }
        StateHasChanged();
    }
}
