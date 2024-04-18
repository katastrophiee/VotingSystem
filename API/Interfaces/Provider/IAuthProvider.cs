using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface IAuthProvider
{
    Task<Response<LoginResponse>> VoterLogin(LoginRequest request);

    Task<Response<LoginResponse>> CreateVoterAccount(CreateVoterAccountRequest request);

    Task<Response<LoginResponse>> AdminLogin(LoginRequest request);

    Task<Response<LoginResponse>> CreateAdminAccount(AdminCreateAdminAccountRequest request);

    Task<Response<LoginResponse>> PutUpdatePassword(UpdatePasswordRequest request);
}
