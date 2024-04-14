using VotingSystem.API.DTO.Responses;

namespace VotingSystem.Services;

public interface IApiRequestService
{
    Task<Response<T>> SendAsync<T>(string endpoint, HttpMethod method, object? data = null, string? queryString = null);
}
