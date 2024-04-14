using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface IVoteProvider
{
    Task<Response<IEnumerable<GetVotingHistoryResponse>>> GetVoterVotingHistory(int voterId);

    Task<Response<bool>> AddVoterVote(AddVoterVoteRequest request);
}
