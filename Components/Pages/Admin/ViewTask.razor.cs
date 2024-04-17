using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class ViewTask
{
    [Parameter]
    public int TaskId { get; set; }

    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<ViewTask> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    public int AdminId { get; set; }

    public AdminGetTaskResponse? Task { get; set; }

    public AdminUpdateTaskRequest UpdateTaskRequest { get; set; } = new();

    public bool Editable { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");
        await GetTask(TaskId);
    }

    private async Task HandleValidSubmit()
    {
        UpdateTaskRequest.AdminId = AdminId;

        var response = await ApiRequestService.SendAsync<bool>($"Admin/PutUpdateTask", HttpMethod.Put, UpdateTaskRequest);
        if (response.Error == null)
            await GetTask(UpdateTaskRequest.TaskId);
        else
            Errors.Add(response.Error);
    }

    private async Task GetTask(int taskId)
    {
        Errors.Clear();

        var task = await ApiRequestService.SendAsync<AdminGetTaskResponse>($"Admin/GetTask", HttpMethod.Get, queryString: $"taskId={taskId}&adminId={AdminId}");
        if (task.Error == null)
        {
            task.Data.Name = task.Data.Name.Trim();
            task.Data.Description = task.Data.Description.Trim();
            task.Data.AdditionalNotes = task.Data.AdditionalNotes is null ? null : task.Data.AdditionalNotes.Trim();
            Task = task.Data;
            UpdateTaskRequest = new AdminUpdateTaskRequest(task.Data);
        }
        else
        {
            Errors.Add(task.Error);
        }
    }
}
