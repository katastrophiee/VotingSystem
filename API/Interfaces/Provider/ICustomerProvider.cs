using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface ICustomerProvider
{
    Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerAccountDetails(int customerId);

    Task<Response<bool>> PutUpdateCustomerProfile(UpdateCustomerProfileRequest request);

    Task<Response<List<GetCandidateResponse>>> GetActiveCandidates(int customerId);
}
