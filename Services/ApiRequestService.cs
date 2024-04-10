using System.Net.Http.Json;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.DTO.Responses.Admin;

namespace VotingSystem.Services;

public class ApiRequestService(HttpClient httpClient) : IApiRequestService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerInfo(int customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Customer/GetCustomerDetails?customerId={customerId}");

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
            var response = await _httpClient.GetAsync($"Vote/GetCustomerVotingHistory?customerId={customerId}");

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
            var response = await _httpClient.PostAsJsonAsync($"Document/PostUploadCustomerDocument", document);

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
            var response = await _httpClient.GetAsync($"Document/GetCustomerDocuments?customerId={customerId}");

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

    public async Task<Response<List<GetElectionResponse>>> GetCustomerOngoingElections(int customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Election/GetCustomerOngoingElections?customerId={customerId}");

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<List<GetElectionResponse>>());
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

    public async Task<Response<List<AdminGetVotersResponse>>> AdminGetCustomers(AdminGetCustomersRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Admin/GetCustomers", request);

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<List<AdminGetVotersResponse>>());
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
                Description = $"An unknown error occurred when trying to retrieve customers for admin {request.AdminId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<AdminGetCustomerResponse>> AdminGetCustomerDetails(int customerId, int adminId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Admin/GetCustomerDetails?customerId={customerId}&adminId={adminId}");

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<AdminGetCustomerResponse>());
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
                Description = $"An unknown error occurred when trying to retrieve customer details for admin {adminId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> AdminVerifyCustomerIdDocument(AdminVerifyIdRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Admin/PostAdminVerifyId", request);

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
                Description = $"An unknown error occurred when trying to verify customer ID document for admin {request.AdminId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<Document>> GetCurrentCustomerDocument(int customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Document/GetCurrentCustomerDocument?customerId={customerId}");

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<Document>());
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


    public async Task<Response<bool>> AdminPostAddElection(AddElectionRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Admin/PostAddElection", request);

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
                Description = $"An unknown error occurred when trying to add a new election for admin {request.AdminId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetElectionResponse>>> GetCustomerVotedInElections(int customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Election/GetCustomerVotedInElections?customerId={customerId}");

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<List<GetElectionResponse>>());
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
                Description = $"An unknown error occurred when trying to fetch voted in elections for customer {customerId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetElectionResponse>>> GetRecentlyEndedElections(int customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Election/GetRecentlyEndedElections?customerId={customerId}");

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<List<GetElectionResponse>>());
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
                Description = $"An unknown error occurred when trying to fetch recently ended elections for customer {customerId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetElectionResponse>>> GetCustomerUpcomingElections(int customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Election/GetCustomerUpcomingElections?customerId={customerId}");

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<List<GetElectionResponse>>());
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
                Description = $"An unknown error occurred when trying to fetch recently ended elections for customer {customerId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<GetElectionResponse>> GetElection(int electionId, int customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Election/GetElection?electionId={electionId}&customerId={customerId}");

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<GetElectionResponse>());
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
                Description = $"An unknown error occurred when trying to fetch election for customer {customerId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> AddCustomerVote(AddCustomerVoteRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Customer/AddCustomerVote", request);

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
                Description = $"An unknown error occurred when trying to add vote for customer {request.CustomerId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetCandidateResponse>>> GetActiveCandidates(int customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Customer/GetActiveCandidates?customerId={customerId}");

            if (response.IsSuccessStatusCode)
            {
                return new(await response.Content.ReadFromJsonAsync<List<GetCandidateResponse>>());
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
                Description = $"An unknown error occurred when trying to add vote for customer {customerId}",
                StatusCode = 500,
                AdditionalDetails = ex.Message
            });
        }
    }
}

