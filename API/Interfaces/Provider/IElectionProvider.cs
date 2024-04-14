using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface IElectionProvider
{
    Task<Response<List<GetElectionResponse>>> GetVoterOngoingElections(int voterId);

    Task<Response<List<GetElectionResponse>>> GetVoterVotedInElections(int voterId);

    Task<Response<List<GetElectionResponse>>> GetRecentlyEndedElections(int voterId);

    Task<Response<List<GetElectionResponse>>> GetVoterUpcomingElections(int voterId);

    Task<Response<GetElectionResponse>> GetElection(int electionId, int voterId);

    Task<Response<List<GetElectionResponse>>> GetElections(GetElectionsRequest request);
}
