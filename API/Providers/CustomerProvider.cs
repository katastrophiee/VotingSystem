using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO;
using VotingSystem.API.Enums;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class CustomerProvider(IDbContextFactory<DBContext> dbContext) : ICustomerProvider
{
    private readonly IDbContextFactory<DBContext> _dbContext = dbContext;

    public async Task<GetCustomerAccountDetailsResponse> GetCustomerAccountDetails(int customerId)
    {
        try
        {
            //db call
            using (var context = _dbContext.CreateDbContext())
            {
                var customer = context.Customer.Find(customerId);
            }

            var result = new GetCustomerAccountDetailsResponse()
            {
                UserId = 1,
                FirstName = "Kat",
                Country = CustomerCountry.UnitedKingdom,
                NewUser = true,
            };

            return result;
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Error getting Price List");
            //return new PriceListResponse(ExceptionHelper.CreateMovexErrorResponse(MovexErrorCode.InternalServerError, "Error while getting price.", ex));
            return null;
        }
    }
}
