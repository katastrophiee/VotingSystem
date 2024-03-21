using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.DbResults;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers
{
    public class CustomerProvider(IDbContextFactory<DBContext> dbContext) : ICustomerProvider
    {
        private readonly IDbContextFactory<DBContext> _dbContext = dbContext;

        public async Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerAccountDetails(int customerId)
        {
            try
            {
                await using var context = _dbContext.CreateDbContext();

                var customer = await context.Customer.FindAsync(customerId);

                if (customer is null)
                    return new(new ErrorResponse(ErrorCode.CustomerNotFound));

                var response = new GetCustomerAccountDetailsResponse(customer);

                return new(response);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error getting Price List");
                return new(new ErrorResponse(ErrorCode.InternalServerError, "Unknown error occurred when retrieveing the customer account details", ex));
            }
        }

        public async Task<Response<IEnumerable<Vote>>> GetCustomerVotingHistory(int customerId)
        {
            try
            {
                await using var context = _dbContext.CreateDbContext();

                var customer = await context.Customer.FindAsync(customerId);
                if (customer is null)
                    return new(new ErrorResponse(ErrorCode.CustomerNotFound));

                var votes = await context.Votes.FindAsync(customerId);

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
