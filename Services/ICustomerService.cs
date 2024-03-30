using VotingSystem.API.DTO.Responses;

namespace VotingSystem.Services;

public interface ICustomerService
{
    Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerInfo(int customerId);

    Task<Response<List<GetVotingHistoryResponse>>> GetCustomerVotingHistory(int customerId);
}
