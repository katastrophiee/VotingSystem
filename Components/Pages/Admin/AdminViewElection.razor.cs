using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ComponentTypes;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class AdminViewElection
{

    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IStringLocalizer<AdminViewElection> Localizer { get; set; }

    [Parameter]
    public int ElectionId { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public AdminGetElectionResponse Election { get; set; }

    public List<ElectionOptionWithState> ElectionOptions { get; set; } = [];

    public AdminUpdateElectionRequest UpdateElectionRequest { get; set; } = new();

    public int AdminId { get; set; }

    public bool Editable { get; set; } = false;

    public bool ShowAddOptionError { get; set; } = false;

    public bool ShowOptionAlreadyAddedError { get; set; } = false;

    public int ElectionOptionId { get; set; } = 1;

    private ElectionOption NewOption = new();

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");

        await GetElection();
    }

    private async Task GetElection()
    {
        var getElectionResponse = await ApiRequestService.SendAsync<AdminGetElectionResponse>($"Admin/GetElection", HttpMethod.Get, queryString: $"electionId={ElectionId}&adminId={AdminId}");
        if (getElectionResponse.Error == null)
        {
            Election = getElectionResponse.Data;
            ElectionOptions = getElectionResponse.Data.ElectionOptions.Select(o => new ElectionOptionWithState(o)).ToList();
            UpdateElectionRequest = new(getElectionResponse.Data);
        }
        else
            Errors.Add(getElectionResponse.Error);
    }

    private async Task HandleUpdateElection()
    {
        var isValidRequest = ValidateAddElectionRequest();
        if (isValidRequest.Error is null)
        {
            UpdateElectionRequest.AdminId = AdminId;

            var response = await ApiRequestService.SendAsync<bool>("Admin/PutUpdateElection", HttpMethod.Put, UpdateElectionRequest);
            if (response.Error == null)
            {
                await GetElection();
                Editable = false;
            }
            else
            {
                Errors.Add(response.Error);
            }
        }
        else
        {
            Errors.Add(isValidRequest.Error);
        }
    }

    private Response<bool> ValidateAddElectionRequest()
    {
        if (string.IsNullOrWhiteSpace(UpdateElectionRequest.ElectionName))
        {
            return new(new ErrorResponse
            {
                Title = Localizer["ElectionNameRequired"],
                Description = Localizer["ElectionNameRequiredDesciption"],
                StatusCode = StatusCodes.Status400BadRequest
            });
        }

        if (string.IsNullOrWhiteSpace(UpdateElectionRequest.ElectionDescription))
        {
            return new(new ErrorResponse
            {
                Title = Localizer["ElectionDescriptionRequired"],
                Description = Localizer["ElectionDescriptionRequiredDescription"],
                StatusCode = StatusCodes.Status400BadRequest
            });
        }

        if (UpdateElectionRequest.StartDate <= DateTime.Now)
        {
            return new(new ErrorResponse
            {
                Title = Localizer["InvalidStartDate"],
                Description = Localizer["InvalidStartDateDescription"],
                StatusCode = StatusCodes.Status400BadRequest
            });
        }

        if (UpdateElectionRequest.EndDate <= DateTime.Now)
        {
            return new(new ErrorResponse
            {
                Title = Localizer["InvalidEndDate"],
                Description = Localizer["InvalidEndDateDescription"],
                StatusCode = StatusCodes.Status400BadRequest
            });
        }

        if (UpdateElectionRequest.EndDate < UpdateElectionRequest.StartDate)
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
        ShowOptionAlreadyAddedError = false;
        if (int.TryParse(e.Value?.ToString(), out int result))
        {
            var candidateExists = await AutofillCandidate(result);
            if (!candidateExists)
            {
                NewOption.OptionName = "";
                NewOption.OptionDescription = "";
            }
        }
        else
        {
            NewOption.CandidateId = null;
            NewOption.OptionName = "";
            NewOption.OptionDescription = "";
        }
    }

    private async Task<bool> AutofillCandidate(int candidateId)
    {
        var candidate = await ApiRequestService.SendAsync<AdminGetCandidateResponse>($"Admin/GetCandidate", HttpMethod.Get, queryString: $"voterId={candidateId}&adminId={AdminId}");
        if (candidate.Error == null)
        {
            NewOption.CandidateId = candidate.Data.CandidateId;
            NewOption.OptionName = candidate.Data.CandidateName;
            NewOption.OptionDescription = candidate.Data.Description;
            return true;
        }

        return false;
    }

    private async Task AddOption()
    {
        var candidateExists = await AutofillCandidate(NewOption.CandidateId ?? 0);

        if (candidateExists)
        {
            if (UpdateElectionRequest.ElectionOptions.Select(o => o.ElectionId == NewOption.CandidateId).Any())
            {
                ShowOptionAlreadyAddedError = true;
            }
            else
            {
                NewOption.OptionId = ElectionOptionId++;
                UpdateElectionRequest.ElectionOptions.Add(NewOption);
                NewOption = new();
            }
        }
        else if (!candidateExists && NewOption.CandidateId is null)
        {
            NewOption.OptionId = ElectionOptionId++;
            UpdateElectionRequest.ElectionOptions.Add(NewOption);
            ShowAddOptionError = false;
            ShowOptionAlreadyAddedError = false;
            NewOption = new();
        }
        else
        {
            NewOption.CandidateId = null;
            ShowAddOptionError = true;
        }
        StateHasChanged();
    }

    private void RemoveOption(ElectionOption option)
    {
        UpdateElectionRequest.ElectionOptions.Remove(option);
        ElectionOptionId--;
        NewOption = new();
        StateHasChanged();
    }
}
