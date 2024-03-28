using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.ErrorHandling;
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

    public async Task<Response<IEnumerable<GetVotingHistoryResponse>>> GetCustomerVotingHistory(int customerId)
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

            var votes = await _dbContext.Vote
               .Where(v => v.CustomerId == customerId)
               .ToListAsync();

            votes.Add(new DTO.DbResults.Vote());


            var response = new List<GetVotingHistoryResponse>();
            votes?.ForEach(vote => response.Add(new(vote)));

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to retrieve voting history for customer {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
