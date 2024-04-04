using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.Services;

public interface IApiRequestService
{
    Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerInfo(int customerId);

    Task<Response<List<GetVotingHistoryResponse>>> GetCustomerVotingHistory(int customerId);

    Task<Response<LoginResponse>> PostCustomerLogin(LoginRequest loginRequest);

    Task<Response<LoginResponse>> PostCreateCustomerAccount(CreateAccountRequest request);

    Task<Response<bool>> PutUploadCustomerDocument(Document document);

    Task<Response<List<Document>>> GetCustomerDocuments(int customerId);
}
