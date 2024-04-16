﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
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

    public AdminGetTasksRequest GetTasksRequest { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");
    }

    private async Task SearchTasks()
    {
        GetTasksRequest.AdminId = AdminId;
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

    //TO DO
    //Add popups
    private async Task DeleteTask(int taskId)
    {
        var tasks = await ApiRequestService.SendAsync<bool>($"Admin/DeleteTask?taskId={taskId}&adminId={AdminId}", HttpMethod.Delete);
        if (tasks.Error == null)
            await SearchTasks();
        else
            Errors.Add(tasks.Error);
    }
}
