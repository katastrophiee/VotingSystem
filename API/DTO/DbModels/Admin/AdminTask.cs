namespace VotingSystem.API.DTO.DbModels.Admin;

public class AdminTask
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
}
