namespace VotingSystem.API.DTO.Requests.Admin;

public class AdminGetTasksRequest
{
    public int AdminId { get; set; }

    public int? TaskId { get; set; }

    public int? ForVoterId { get; set; }

    public int? ForAdminId { get; set; }

    public string? Name { get; set; }

    public Enums.TaskStatus? TaskStatus { get; set; }

    public int? AssignedToAdminId { get; set; }
}
