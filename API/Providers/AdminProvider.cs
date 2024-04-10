using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class AdminProvider(DBContext dbContext) : IAdminProvider
{
    private readonly DBContext _dbContext = dbContext;

    public async Task<Response<List<AdminGetVotersResponse>>> GetCustomers(AdminGetCustomersRequest request)
    {
        try
        {
            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == request.AdminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Admin Found",
                    Description = $"No admin was found with the admin id {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var customers = await _dbContext.Customer.Where(c => 
                (request.UserId == null || c.Id == request.UserId) &&
                (request.Country == null || c.Country == request.Country) &&
                (request.IsCandidate == null || c.IsCandidate == request.IsCandidate) &&
                (request.IsVerified == null || c.IsVerified == request.IsVerified))
               .ToListAsync();

            var response = new List<AdminGetVotersResponse>();
            customers?.ForEach(customer => response.Add(new(customer)));

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to retrieve customers for admin {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<AdminGetCustomerResponse>> GetCustomerDetails(int customerId, int adminId)
    {
        try
        {
            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == adminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Admin Found",
                    Description = $"No admin was found with the admin id {adminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == customerId);

            var response = new AdminGetCustomerResponse(customer);

            var currentIdDocument = await _dbContext.Document.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.MostRecentId == true);
            if (currentIdDocument != null)
                response.CurrentIdDocument = currentIdDocument;

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to retrieve customer details for admin {adminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> AdminVerifyId(AdminVerifyIdRequest request)
    {
        try
        {
            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == request.AdminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Admin Found",
                    Description = $"No admin was found with the admin id {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var document = await _dbContext.Document.FirstOrDefaultAsync(d => d.Id == request.DocumentId);

            if (document is null)
                return new(new ErrorResponse()
                {
                    Title = "No Document Found",
                    Description = $"No document was found with the id {request.DocumentId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            document.IdRejected = request.IsRejected;

            if (!request.IsRejected)
            {
                document.ExpiryDate = request.DocumentExpiryDate;

                var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == document.CustomerId);
                customer.IsVerified = true;
                _dbContext.Customer.Update(customer);
            }
            else
            {
                document.RejectionReason = request.RejectionReason;
                document.RejectedByAdminId = request.AdminId;
            }

            _dbContext.Document.Update(document);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to verify ID {request.DocumentId} for admin {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> AddElection(AddElectionRequest request)
    {
        try
        {
            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == request.AdminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Admin Found",
                    Description = $"No admin was found with the admin id {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            // check startdate is not more than today and check it is not more than the end date and end date is not today

            var election = new Election()
            {
                ElectionName = request.ElectionName,
                ElectionDescription = request.ElectionDescription,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Country = request.Country,
                ElectionType = request.ElectionType,
                ElectionOptions = request.ElectionOptions,
            };

            _dbContext.Election.Add(election);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to add a new election for admin {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
