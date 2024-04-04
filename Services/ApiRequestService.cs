using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.Services;

public class ApiRequestService(HttpClient httpClient) : IApiRequestService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerInfo(int customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Customer/GetCustomerDetails?id={customerId}");

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<GetCustomerAccountDetailsResponse>());
            }
            else
            {
                return new(await response.Content.ReadFromJsonAsync<ErrorResponse>());
            }
        }
        catch (Exception ex)
        {
            
            return new(new ErrorResponse
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occurred when trying to retrieve customer details for customer {customerId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetVotingHistoryResponse>>> GetCustomerVotingHistory(int customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Customer/GetCustomerVotingHistory?id={customerId}");

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<List<GetVotingHistoryResponse>>() ?? []);
            }
            else
            {
                return new(await response.Content.ReadFromJsonAsync<ErrorResponse>());
            }
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occurred when trying to retrieve voting history for customer {customerId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<LoginResponse>> PostCustomerLogin(LoginRequest loginRequest)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Auth/PostCustomerLogin", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<LoginResponse>());
            }
            else
            {
                return new(await response.Content.ReadFromJsonAsync<ErrorResponse>());
            }
        }
        catch(Exception ex) 
        {
            return new(new ErrorResponse
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occurred when trying to login for customer {loginRequest.Username}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<LoginResponse>> PostCreateCustomerAccount(CreateAccountRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Auth/PostCreateAccount", request);

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<LoginResponse>());
            }
            else
            {
                return new(await response.Content.ReadFromJsonAsync<ErrorResponse>());
            }
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occurred when trying to create an account for customer {request.Username}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }
}
