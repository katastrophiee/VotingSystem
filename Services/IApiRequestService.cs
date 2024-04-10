using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.DTO.Responses.Admin;

namespace VotingSystem.Services;

public interface IApiRequestService
{
    Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerInfo(int customerId);

    Task<Response<List<GetVotingHistoryResponse>>> GetCustomerVotingHistory(int customerId);

    Task<Response<LoginResponse>> PostCustomerLogin(LoginRequest loginRequest);

    Task<Response<LoginResponse>> PostCreateCustomerAccount(CreateCustomerAccountRequest request);

    Task<Response<bool>> PostUploadCustomerDocument(Document document);

    Task<Response<List<Document>>> GetCustomerDocuments(int customerId);

    Task<Response<bool>> PutUpdateCustomerProfile(UpdateCustomerProfileRequest request);

    Task<Response<List<GetElectionResponse>>> GetCustomerOngoingElections(int customerId);

    Task<Response<LoginResponse>> PostAdminLogin(LoginRequest loginRequest);

    Task<Response<List<AdminGetVotersResponse>>> AdminGetCustomers(AdminGetCustomersRequest request);

    Task<Response<AdminGetCustomerResponse>> AdminGetCustomerDetails(int customerId, int adminId);

    Task<Response<bool>> AdminVerifyCustomerIdDocument(AdminVerifyIdRequest request);

    Task<Response<Document>> GetCurrentCustomerDocument(int customerId);

    Task<Response<bool>> AdminPostAddElection(AddElectionRequest request);

    Task<Response<List<GetElectionResponse>>> GetCustomerVotedInElections(int customerId);

    Task<Response<List<GetElectionResponse>>> GetRecentlyEndedElections(int customerId);

    Task<Response<List<GetElectionResponse>>> GetCustomerUpcomingElections(int customerId);

    Task<Response<GetElectionResponse>> GetElection(int electionId, int customerId);

    Task<Response<bool>> AddCustomerVote(AddCustomerVoteRequest request);

    Task<Response<List<GetCandidateResponse>>> GetActiveCandidates(int customerId);

    Task<Response<bool>> PutMakeCustomerACandidate(BecomeCandidateRequest request);

    Task<Response<GetCandidateResponse>> GetCandidate(int customerId, int adminId);
}
