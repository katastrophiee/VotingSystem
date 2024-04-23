using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class AddTask
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<AddTask> Localizer { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    public int AdminId { get; set; }

    public AdminAddTaskRequest AddTaskRequest { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");
    }

    private async Task HandleAddTask()
    {
        AddTaskRequest.AdminId = AdminId;
        var response = await ApiRequestService.SendAsync<int>("Admin/PostAddTask", HttpMethod.Post, AddTaskRequest);
        if (response.Error == null)
            NavigationManager.NavigateTo($"/view-task/taskId={response.Data}");
        else
            Errors.Add(response.Error);
    }
}
