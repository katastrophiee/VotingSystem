using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Responses;

public class GetCandidateResponse
{
    public int CandidateId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public VoterCountry Country { get; set; }

    public DateTime StartDateOfCandidacy { get; set; }

    public List<int> OngoingEnteredElectionsIds { get; set; }

    public GetCandidateResponse()
    {
    }

    public GetCandidateResponse(Voter candidate)
    {
        CandidateId = candidate.Id;
        Name = candidate.CandidateName ?? "";
        Description = candidate.CandidateDescription ?? "Unknown";
        Country = candidate.Country;
        StartDateOfCandidacy = candidate.DateOfCandidacy ?? DateTime.MaxValue;
    }
}
