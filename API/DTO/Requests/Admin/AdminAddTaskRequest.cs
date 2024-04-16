namespace VotingSystem.API.DTO.Requests.Admin;

public class AdminAddTaskRequest
{
    public int AdminId { get; set; }

    public int? ForVoterId { get; set; }

    public int? ForAdminId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}
