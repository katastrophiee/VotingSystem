using Azure.Core;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VotingSystem.Components.Pages.Admin;

public partial class AddElection
{
    private AddElectionRequest AddElectionRequest = new([])
    {
        StartDate = DateTime.Now,
        EndDate = DateTime.Now
    };

    private ElectionOption NewOption = new();

    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IStringLocalizer<AddElection> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    public int AdminId { get; set; }

    public bool ShowAddOptionError { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");
    }

    private async Task HandleValidSubmit()
    {
        AddElectionRequest.AdminId = AdminId;

        Errors.Clear();

        var isValidRequest = ValidateAddElectionRequest();
        if (isValidRequest.Error is null)
        { 
            var response = await ApiRequestService.SendAsync<bool>("Admin/PostAddElection", HttpMethod.Post, AddElectionRequest);

            if (response.Error == null)
            {
                NavigationManager.NavigateTo("/view-elections");
            }
            else
                Errors.Add(response.Error);
        }
        else
        {
            Errors.Add(isValidRequest.Error);
        }
    }

    private Response<bool> ValidateAddElectionRequest()
    {
        if (string.IsNullOrWhiteSpace(AddElectionRequest.ElectionName))
        {
            return new(new ErrorResponse
            {
                Title = Localizer["ElectionNameRequired"],
                Description = Localizer["ElectionNameRequiredDesciption"],
                StatusCode = StatusCodes.Status400BadRequest
            });
        }

        if (string.IsNullOrWhiteSpace(AddElectionRequest.ElectionDescription))
        {
            return new(new ErrorResponse
            {
                Title = Localizer["ElectionDescriptionRequired"],
                Description = Localizer["ElectionDescriptionRequiredDescription"],
                StatusCode = StatusCodes.Status400BadRequest
            });
        }

        if (AddElectionRequest.StartDate <= DateTime.Now)
        {
            return new(new ErrorResponse
            {
                Title = Localizer["InvalidStartDate"],
                Description = Localizer["InvalidStartDateDescription"],
                StatusCode = StatusCodes.Status400BadRequest
            });
        }

        if (AddElectionRequest.EndDate <= DateTime.Now)
        {
            return new(new ErrorResponse
            {
                Title = Localizer["InvalidEndDate"],
                Description = Localizer["InvalidEndDateDescription"],
                StatusCode = StatusCodes.Status400BadRequest
            });
        }

        if (AddElectionRequest.EndDate < AddElectionRequest.StartDate)
        {
            return new(new ErrorResponse
            {
                Title = Localizer["InvalidDateRange"],
                Description = Localizer["InvalidDateRangeDescription"],
                StatusCode = StatusCodes.Status400BadRequest
            });
        }

        return new(true);
    }

    private async Task OnCandidateIdInput(ChangeEventArgs e)
    {
        ShowAddOptionError = false;
        if (int.TryParse(e.Value?.ToString(), out int result))
        {
            NewOption.CandidateId = result;
            var candidateExists = await AutofillCandidate(result);
            if (!candidateExists)
            {
                NewOption.OptionName = "";
                NewOption.OptionDescription = "";
            }
        }
        else
        {
            NewOption.OptionName = "";
            NewOption.OptionDescription = "";
        }
    }

    private async Task<bool> AutofillCandidate(int candidateId)
    {
        var candidate = await ApiRequestService.SendAsync<GetCandidateResponse>($"Admin/GetCandidate?voterId={candidateId}&adminId={AdminId}", HttpMethod.Get);
        if (candidate.Error == null)
        {
            NewOption.OptionName = candidate.Data.Name;
            NewOption.OptionDescription = candidate.Data.Description;
            return true;
        }

        return false;
    }

    private async void AddOption()
    {
        var candidateExists = await AutofillCandidate(NewOption.CandidateId ?? 0);

        if (candidateExists)
        {
            AddElectionRequest.ElectionOptions.Add(NewOption);
            NewOption = new();
        }
        else
        {
            ShowAddOptionError = true;
        }
        StateHasChanged();
    }

    private void RemoveOption(ElectionOption option)
    {
        AddElectionRequest.ElectionOptions.Remove(option);
    }
}
