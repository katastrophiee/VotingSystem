using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.DbResults;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers
{
    public class CustomerProvider : ICustomerProvider
    {
        private readonly DBContext _dbContext;

        public CustomerProvider(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerAccountDetails(int customerId)
        {
            try
            {
                var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == customerId);

                if (customer is null || customer.Id == 0)
                    return new(new ErrorResponse(ErrorCode.CustomerNotFound));

                var response = new GetCustomerAccountDetailsResponse(customer);

                return new(response);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error getting Price List");
                return new(new ErrorResponse(ErrorCode.InternalServerError, "Unknown error occurred when retrieving the customer account details", ex));
            }
        }

        public async Task<Response<IEnumerable<Vote>>> GetCustomerVotingHistory(int customerId)
        {
            try
            {
                var customer = await _dbContext.Customer.FindAsync(customerId);
                if (customer is null || customer.Id == 0)
                    return new(new ErrorResponse(ErrorCode.CustomerNotFound));

                var votes = await _dbContext.Vote
                    .Where(v => v.CustomerId == customerId)
                    .ToListAsync();

                if (votes is null)
                    return new([]);

                return new(votes);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error getting Price List");
                return new(new ErrorResponse(ErrorCode.InternalServerError, "Unknown error occurred when retrieveing the customer account details", ex));
            }
        }
    }
}
