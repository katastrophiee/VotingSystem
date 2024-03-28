using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface IAuthProvider
{
    Task<Response<LoginResponse>> CustomerLogin(LoginRequest request);
}
