using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface IElectionProvider
{
    Task<Response<List<GetElectionResponse>>> GetCustomerOngoingElections(int customerId);

    Task<Response<List<GetElectionResponse>>> GetCustomerVotedInElections(int customerId);

    Task<Response<List<GetElectionResponse>>> GetRecentlyEndedElections(int customerId);

    Task<Response<List<GetElectionResponse>>> GetCustomerUpcomingElections(int customerId);

    Task<Response<GetElectionResponse>> GetElection(int electionId, int customerId);
}
