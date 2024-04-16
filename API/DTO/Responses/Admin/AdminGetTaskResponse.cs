using VotingSystem.API.DTO.DbModels.Admin;

namespace VotingSystem.API.DTO.Responses.Admin;

public class AdminGetTaskResponse
{
    public int Id { get; set; }

    public int? ForVoterId { get; set; }

    public int? ForAdminId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Enums.TaskStatus TaskStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastEdited { get; set; }

    public int? AssignedToAdminId { get; set; }

    public int? CreatedByAdminId { get; set; }

    public string? AdditionalNotes { get; set; }

    public AdminGetTaskResponse()
    {
    }

    public AdminGetTaskResponse(AdminTask task)
    {
        Id = task.Id;
        ForVoterId = task.ForVoterId;
        ForAdminId = task.ForAdminId;
        Name = task.Name;
        Description = task.Description;
        TaskStatus = task.TaskStatus;
        CreatedAt = task.CreatedAt;
        LastEdited = task.LastEdited;
        AssignedToAdminId = task.AssignedToAdminId;
        CreatedByAdminId = task.CreatedByAdminId;
        AdditionalNotes = task.AdditionalNotes;
    }
}
