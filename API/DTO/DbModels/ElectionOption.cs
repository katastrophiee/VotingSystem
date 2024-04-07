namespace VotingSystem.API.DTO.DbModels;

public class ElectionOption
{
    public int OptionId { get; set; }

    public string OptionName { get; set; }

    public string OptionDescription { get; set; }

    public int ElectionId { get; set; }
}
