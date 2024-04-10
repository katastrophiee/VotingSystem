using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class DocumentProvider(DBContext dbContext) : IDocumentProvider
{
    private readonly DBContext _dbContext = dbContext;
    public async Task<Response<Document?>> GetCurrentCustomerDocument(int customerId)
    {
        try
        {
            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == customerId);
            if (customer is null || customer.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Customer Found",
                    Description = $"No customer was found with the customer id {customerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var currentDocument = await _dbContext.Document
              .Where(v => v.CustomerId == customerId && v.MostRecentId == true)
              .FirstOrDefaultAsync();

            return new(currentDocument);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to retrieve documents for customer {customerId}",
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
                    Title = "No Customer Found",
                    Description = $"No customer was found with the customer id {customerId}",
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
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to retrieve documents for customer {customerId}",
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
                    Title = "No Customer Found",
                    Description = $"No customer was found with the customer id {document.CustomerId}",
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

            _dbContext.Document.Add(document);
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
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to upload document {document.FileName}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
