using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface IElectionProvider
{
    Task<Response<IEnumerable<GetElectionResponse>>> GetVoterOngoingElections(int voterId);

    Task<Response<IEnumerable<GetElectionResponse>>> GetVoterVotedInElections(int voterId);

    Task<Response<IEnumerable<GetElectionResponse>>> GetRecentlyEndedElections(int voterId);

    Task<Response<IEnumerable<GetElectionResponse>>> GetVoterUpcomingElections(int voterId);

    Task<Response<GetElectionResponse>> GetElection(int electionId, int voterId);

    Task<Response<IEnumerable<GetElectionResponse>>> GetElections(GetElectionsRequest request);
}
