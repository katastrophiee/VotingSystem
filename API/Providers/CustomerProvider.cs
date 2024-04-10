using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class CustomerProvider(DBContext dbContext) : ICustomerProvider
{
    private readonly DBContext _dbContext = dbContext;

    public async Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerAccountDetails(int customerId)
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

            var response = new GetCustomerAccountDetailsResponse(customer);

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to retrieve customer {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> PutUpdateCustomerProfile(UpdateCustomerProfileRequest request)
    {
        try
        {
            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == request.UserId);
            if (customer is null || customer.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Customer Found",
                    Description = $"No customer was found with the customer id {request.UserId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (!string.IsNullOrEmpty(request.Email))
                customer.Email = request.Email;

            if (!string.IsNullOrEmpty(request.FirstName))
                customer.FirstName = request.FirstName;

            if (!string.IsNullOrEmpty(request.LastName))
                customer.LastName = request.LastName;

            if (request.Country is not null)
                customer.Country = request.Country.Value;

            _dbContext.Customer.Update(customer);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to update profile for customer {request.UserId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
