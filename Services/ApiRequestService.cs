using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Requests.Admin;
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
            var response = await _httpClient.GetAsync($"Customer/GetCustomerVotingHistory?customerId={customerId}");

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

    public async Task<Response<LoginResponse>> PostCreateCustomerAccount(CreateCustomerAccountRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Auth/PostCreateCustomerAccount", request);

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

    public async Task<Response<bool>> PostUploadCustomerDocument(Document document)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Customer/PostUploadCustomerDocument", document);

            if (response.IsSuccessStatusCode)
            {
                return new(true);
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
                Description = $"An unknown error occurred when trying to upload document",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<Document>>> GetCustomerDocuments(int customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Customer/GetCustomerDocuments?customerId={customerId}");

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<List<Document>>());
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
                Description = $"An unknown error occurred when trying to retrieve document for customer {customerId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> PutUpdateCustomerProfile(UpdateCustomerProfileRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"Customer/PutUpdateCustomerProfile", request);

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<bool>());
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
                Description = $"An unknown error occurred when trying to update customer profile for customer {request.UserId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetOngoingElectionsResponse>>> GetCustomerOngoingElections(int customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Customer/GetCustomerOngoingElections?customerId={customerId}");

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<List<GetOngoingElectionsResponse>>());
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
                Description = $"An unknown error occurred when trying to fetch ongoing elections for customer {customerId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<LoginResponse>> PostAdminLogin(LoginRequest loginRequest)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Auth/PostAdminLogin", loginRequest);

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
                Description = $"An unknown error occurred when trying to login for admin {loginRequest.Username}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<LoginResponse>> PostCreateAdminAccount(CreateAdminAccountRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Auth/PostCreateAdminAccount", request);

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

