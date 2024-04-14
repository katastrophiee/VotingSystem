using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.DTO.Responses.Admin;

namespace VotingSystem.API.Interfaces.Provider;

public interface IAdminProvider
{
    Task<Response<List<AdminGetVotersResponse>>> GetVoters(AdminGetVotersRequest request);

    Task<Response<AdminGetVoterResponse>> GetVoterDetails(int voterId, int adminId);

    Task<Response<bool>> AdminVerifyId(AdminVerifyIdRequest request);

    Task<Response<bool>> AddElection(AddElectionRequest request);

    Task<Response<GetCandidateResponse>> GetCandidate(int voterId, int adminId);
}
