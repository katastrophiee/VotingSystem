using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class DocumentProvider(DBContext dbContext, IStringLocalizer<DocumentProvider> localizer) : IDocumentProvider
{
    private readonly DBContext _dbContext = dbContext;
    private readonly IStringLocalizer<DocumentProvider> _localizer = localizer;

    public async Task<Response<Document>> GetCurrentCustomerDocument(int customerId)
    {
        try
        {
            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == customerId);
            if (customer is null || customer.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoCustomerFound"],
                    Description = $"{_localizer["NoCustomerFoundWithId"]} {customerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var currentDocument = await _dbContext.Document
              .Where(v => v.CustomerId == customerId && v.MostRecentId == true)
              .FirstOrDefaultAsync();

            return new(currentDocument ?? new());
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetCurrentCustomerDocument"]} {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<Document>>> GetCustomerDocuments(int customerId)
    {
        try
        {
            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == customerId);
            if (customer is null || customer.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoCustomerFound"],
                    Description = $"{_localizer["NoCustomerFoundWithId"]} {customerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var documents = await _dbContext.Document
              .Where(v => v.CustomerId == customerId)
              .ToListAsync();

            return new(documents);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetCustomerDocuments"]} {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> PostUploadCustomerDocument(Document document)
    {
        try
        {
            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == document.CustomerId);

            if (customer is null || customer.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoCustomerFound"],
                    Description = $"{_localizer["NoCustomerFoundWithId"]} {document.CustomerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var currentIdDocument = await _dbContext.Document.FirstOrDefaultAsync(c => c.CustomerId == document.CustomerId && c.MostRecentId == true);
            if (currentIdDocument != null)
            {
                currentIdDocument.MostRecentId = false;
                _dbContext.Document.Update(currentIdDocument);
            }

            document.CustomerId = customer.Id;
            document.UploadedDate = DateTime.UtcNow;
            document.IsVerified = false;
            document.MostRecentId = true;

            customer.IsVerified = false;

            _dbContext.Document.Add(document);
            _dbContext.Customer.Update(customer);
            await _dbContext.SaveChangesAsync();

            // TO DO
            // add task for admins to verify the uploaded id, then sets the account to verified
            // ensure admins set the expirey date for the id by viewing it

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetCustomerDocuments"]} {document.FileName}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
