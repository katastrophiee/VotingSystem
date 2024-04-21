using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels.Admin;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class ViewTasks
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<ViewTasks> Localizer { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    public int AdminId { get; set; }

    public List<AdminGetTaskResponse>? Tasks { get; set; }

    public List<AdminGetAdminResponse> Admins { get; set; } = [];

    public AdminGetTasksRequest GetTasksRequest { get; set; } = new();

    public string AssignedAdminIdString { get; set; } = "";

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");

        var getAdminsRequest = new AdminGetAdminRequest()
        {
            RequestingAdminId = AdminId,
            IsActive = true
        };

        var admins = await ApiRequestService.SendAsync<List<AdminGetAdminResponse>>($"Admin/PostGetAdmins", HttpMethod.Post, getAdminsRequest);
        if (admins.Error == null)
            Admins = admins.Data;
        else
            Errors.Add(admins.Error);
    }

    private async Task SearchTasks()
    {
        GetTasksRequest.AdminId = AdminId;
        GetTasksRequest.AssignedToAdminId = int.TryParse(AssignedAdminIdString, out int assignedToAdminId) ? assignedToAdminId : null;
        var tasks = await ApiRequestService.SendAsync<IEnumerable<AdminGetTaskResponse>>("Admin/PostGetTasks", HttpMethod.Post, GetTasksRequest);
        if (tasks.Error == null)
            Tasks = tasks.Data.ToList();
        else
            Errors.Add(tasks.Error);
    }

    private void ClearSearch()
    {
        GetTasksRequest = new();
        Tasks?.Clear();
    }

    private async Task DeleteTask(int taskId)
    {
        var tasks = await ApiRequestService.SendAsync<bool>($"Admin/DeleteTask", HttpMethod.Delete, queryString: $"taskId={taskId}&adminId={AdminId}");
        if (tasks.Error == null)
            await SearchTasks();
        else
            Errors.Add(tasks.Error);
    }
}
