using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;

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

    Task<Response<List<GetOngoingElectionsResponse>>> GetCustomerOngoingElections(int customerId);

    Task<Response<LoginResponse>> PostAdminLogin(LoginRequest loginRequest);

}
