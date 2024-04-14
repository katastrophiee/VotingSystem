namespace VotingSystem.API.DTO.DbModels;

public class UserRole
{
    public int Id { get; set; }

    public IEnumerable<int> RoleIds { get; set; }

    public int UserId { get; set; }

    public bool IsAdmin { get; set; }
}
