using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface ICustomerProvider
{
    Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerAccountDetails(int customerId);
}
