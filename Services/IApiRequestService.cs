using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.Services;

public interface IApiRequestService
{
    Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerInfo(int customerId);

    Task<Response<List<GetVotingHistoryResponse>>> GetCustomerVotingHistory(int customerId);

    Task<Response<LoginResponse>> PostCustomerLogin(LoginRequest loginRequest);

    Task<Response<int>> PostCustomerCreateAccount(CreateAccountRequest loginRequest);
}
