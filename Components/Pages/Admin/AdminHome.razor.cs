using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Admin;

public partial class AdminHome
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public IStringLocalizer<AdminHome> Localizer { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];

    public int AdminId { get; set; }

    public List<AdminGetTaskResponse>? AdminTasks { get; set; }

    protected override async Task OnInitializedAsync()
    {
        AdminId = await _localStorage.GetItemAsync<int>("adminUserId");

        var getAdminAssignedToTasks = new AdminGetTasksRequest()
        {
            AdminId = AdminId,
            AssignedToAdminId = AdminId
        };

        var tasks = await ApiRequestService.SendAsync<IEnumerable<AdminGetTaskResponse>>("Admin/PostGetTasks", HttpMethod.Post, getAdminAssignedToTasks);
        if (tasks.Error == null)
            AdminTasks = tasks.Data.ToList();
        else
            Errors.Add(tasks.Error);
    }
}
