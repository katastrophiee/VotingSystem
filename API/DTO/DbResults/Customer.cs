namespace VotingSystem.API.DTO.DbResults;

public class Customer
{
    public int Id { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool NewUser { get; set; }
}
