using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface IAuthProvider
{
    Task<Response<LoginResponse>> CustomerLogin(LoginRequest request);

    Task<Response<LoginResponse>> CreateCustomerAccount(CreateCustomerAccountRequest request);

    Task<Response<LoginResponse>> AdminLogin(LoginRequest request);

    Task<Response<LoginResponse>> CreateAdminAccount(CreateAdminAccountRequest request);
}
