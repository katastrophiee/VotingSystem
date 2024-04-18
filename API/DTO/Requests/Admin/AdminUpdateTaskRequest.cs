using VotingSystem.API.DTO.Responses.Admin;

namespace VotingSystem.API.DTO.Requests.Admin;

public class AdminUpdateTaskRequest
{
    public int AdminId { get; set; }

    public int TaskId { get; set; }

    public int? ForVoterId { get; set; }

    public int? ForAdminId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public Enums.TaskStatus? TaskStatus { get; set; }

    public int? AssignedToAdminId { get; set; }

    public string? AdditionalNotes { get; set; }

    public AdminUpdateTaskRequest()
    {
    }

    public AdminUpdateTaskRequest(AdminGetTaskResponse task)
    {
        TaskId = task.Id;
        ForVoterId = task.ForVoterId;
        ForAdminId = task.ForAdminId;
        Name = task.Name;
        Description = task.Description;
        TaskStatus = task.TaskStatus;
        AssignedToAdminId = task.AssignedToAdminId;
    }
}
