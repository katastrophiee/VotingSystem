using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
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

    public GetCustomerAccountDetailsResponse? CustomerDetails { get; set; }
    public List<ErrorResponse> Errors { get; set; } = [];
    
    public InputFile uploadedIdDocument;

    public Document? IdDocument { get; set; } = null;

    public async Task Update()
    {

        Editable = false;
    }

    protected override async Task OnInitializedAsync()
    {
        var customerDetails = await ApiRequestService.GetCustomerInfo(UserId);
        if (customerDetails.Error == null)
            CustomerDetails = customerDetails.Data;
        else
            Errors.Add(customerDetails.Error);

       await SetCurrentIdDocument();
    }

    private async Task UploadIdentification(InputFileChangeEventArgs uploaded)
    {
        try
        {
            // TO DO
            //change to show a button to upload that then runs this code
            var file = uploaded.File;
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

                var successfulUpload = await ApiRequestService.PutUploadCustomerDocument(document);

                if (successfulUpload.Error != null)
                    Errors.Add(successfulUpload.Error);

                await SetCurrentIdDocument();
            }
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
                IdDocument = currentIdDocument;
            }

            // TO DO
            //get current id, check expirey date, option to upload new/different id if expires in 3 months
        }
        else
        {
            Errors.Add(documents.Error);
        }
    }
}
