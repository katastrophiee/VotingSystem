namespace VotingSystem.API.DTO.Requests;

public class UpdateCandidateRequest
{
    public int VoterId { get; set; }
    public string CandidateName { get; set; }
    public string CandidateDescription { get; set; }
}
