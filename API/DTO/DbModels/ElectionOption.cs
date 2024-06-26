﻿using VotingSystem.API.DTO.ComponentTypes;
using static VotingSystem.Components.Pages.Voter.ViewElection;

namespace VotingSystem.API.DTO.DbModels;

public class ElectionOption
{
    public int? CandidateId { get; set; }

    public int OptionId { get; set; }

    public string OptionName { get; set; }

    public string OptionDescription { get; set; }

    public int ElectionId { get; set; }

    public ElectionOption()
    {
    }

    public ElectionOption(ElectionOptionWithState electionOption)
    {
        OptionId = electionOption.OptionId;
        OptionName = electionOption.OptionName;
        OptionDescription = electionOption.OptionDescription;
        ElectionId = electionOption.ElectionId;
    }

    public ElectionOption(ElectionOptionWithRank electionOption)
    {
        OptionId = electionOption.OptionId;
        OptionName = electionOption.OptionName;
        OptionDescription = electionOption.OptionDescription;
        ElectionId = electionOption.ElectionId;
    }
}
