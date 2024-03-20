using VotingSystem.API.DTO;
using VotingSystem.API.Enums;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Providers;

public class CustomerProvider : ICustomerProvider
{
    //private readonly ICustomerRepository _customerRepository;

    //public CustomerProvider(ICustomerRepository customerRepository)
    //{
    //    _customerRepository = customerRepository;
    //}

    public async Task<GetCustomerAccountDetailsResponse> GetCustomerAccountDetails(int customerId)
    {
        try
        {
            //db call

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
