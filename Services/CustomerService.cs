using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.Services;

public class CustomerService(HttpClient httpClient) : ICustomerService
{
    private readonly HttpClient _httpClient = httpClient;

    //private static readonly string ControllerUrl = "https://localhost:44389/api/";
    //DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer") }

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
}
