using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;
using System.Net.Http.Headers;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.Services;

public class ApiRequestService(
    HttpClient httpClient,
    IStringLocalizer<ApiRequestService> localizer,
    ILocalStorageService localStorage) : IApiRequestService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IStringLocalizer<ApiRequestService> _localizer = localizer;
    private readonly ILocalStorageService _localStorage = localStorage;

    public async Task<Response<T>> SendAsync<T>(string endpoint, HttpMethod method, object? data = null, string? queryString = null, bool isNullable = false)
    {
        try
        {
            var fullEndpoint = endpoint + (string.IsNullOrWhiteSpace(queryString) ? "" : "?" + queryString);
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(_httpClient.BaseAddress ?? new(""), fullEndpoint)
            };

            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (data != null)
            {
                request.Content = JsonContent.Create(data);
            }

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                // I used this when trying to allow for nullable types to be returned
                // https://stackoverflow.com/questions/6428781/how-to-check-if-a-generic-type-parameter-is-nullable

                if ((int)response.StatusCode == StatusCodes.Status204NoContent && isNullable)
                    return new(null);

                var result = await response.Content.ReadFromJsonAsync<T>();
                
                if (result != null)
                {
                    return new(result);
                }
                else
                {
                    return new(new ErrorResponse
                    {
                        Title = _localizer["InternalServerError"],
                        Description = _localizer["InternalServerErrorDescription"],
                        StatusCode = StatusCodes.Status500InternalServerError
                    });
                }
            }
            else
            {
                return new(await response.Content.ReadFromJsonAsync<ErrorResponse>() ?? new());
            }
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse
            {
                Title = _localizer["InternalServerError"],
                Description = _localizer["InternalServerErrorDescription"],
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}

