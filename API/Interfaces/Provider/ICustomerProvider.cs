using VotingSystem.API.DTO;

namespace VotingSystem.API.Interfaces.Provider;

public interface ICustomerProvider
{
    Task<GetCustomerAccountDetailsResponse> GetCustomerAccountDetails(int customerId);
}
