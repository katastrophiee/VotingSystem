using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Responses.Admin;

public class AdminGetCandidateResponse
{
    public int CandidateId { get; set; }

    public string CandidateName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Description { get; set; }

    public UserCountry Country { get; set; }

    public DateTime StartDateOfCandidacy { get; set; }

    public bool IsVerified { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public DateTime LastLoggedIn { get; set; }

    public bool IsActive { get; set; }

    public AdminGetCandidateResponse()
    {
    }

    public AdminGetCandidateResponse(Voter candidate)
    {
        CandidateId = candidate.Id;
        CandidateName = candidate.CandidateName ?? "";
        Description = candidate.CandidateDescription ?? "Unknown";
        Country = candidate.Country;
        StartDateOfCandidacy = candidate.DateOfCandidacy ?? DateTime.MaxValue;
        IsVerified = candidate.IsVerified;
        Username = candidate.Username;
        Email = candidate.Email;
        LastLoggedIn = candidate.LastLoggedIn;
        IsActive = candidate.IsActive;
    }
}
