using VotingSystem.API.DTO.DbModels;

namespace VotingSystem.API.DTO.ComponentTypes;

public class ElectionOptionWithState : ElectionOption
{
    public bool IsChecked { get; set; }

    public ElectionOptionWithState()
    {
    }

    public ElectionOptionWithState(ElectionOption option)
    {
        OptionId = option.OptionId;
        OptionName = option.OptionName;
        OptionDescription = option.OptionDescription;
        ElectionId = option.ElectionId;
        IsChecked = false;
    }
}
