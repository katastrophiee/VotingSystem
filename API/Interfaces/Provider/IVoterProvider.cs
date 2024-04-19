using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface IVoterProvider
{
    Task<Response<GetVoterAccountDetailsResponse>> GetVoterAccountDetails(int voterId);

    Task<Response<bool>> PutUpdateVoterProfile(UpdateVoterProfileRequest request);

    Task<Response<IEnumerable<GetCandidateResponse>>> GetActiveCandidates(int voterId);

    Task<Response<bool>> MakeVoterACandidate(BecomeCandidateRequest request);

    Task<Response<GetCandidateResponse>> GetCandidate(int voterId, int candidateId);

    Task<Response<bool>> UpdateCandidate(UpdateCandidateRequest request);

    Task<Response<bool>> GetInPersonVotingEligibility(int voterId);

    Task<Response<bool>> RevokeCandidacy(int voterId);
}
