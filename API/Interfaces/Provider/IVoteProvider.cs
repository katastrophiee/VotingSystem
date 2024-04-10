using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface IVoteProvider
{
    Task<Response<IEnumerable<GetVotingHistoryResponse>>> GetCustomerVotingHistory(int customerId);

    Task<Response<bool>> AddCustomerVote(AddCustomerVoteRequest request);
}
