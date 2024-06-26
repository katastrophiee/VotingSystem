﻿using VotingSystem.API.DTO.ComponentTypes;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests.Admin;

public class AdminUpdateVoterDetailsRequest
{
    public int VoterId { get; set; }

    public int AdminId { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [PasswordRules]
    public string? Password { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public UserCountry? Country { get; set; }

    public bool? NewUser { get; set; }

    public bool? IsVerified { get; set; }

    public bool? IsCandidate { get; set; }

    public bool? IsActive { get; set; }

    public string? CandidateName { get; set; }

    public string? CandidateDescription { get; set; }

    public AdminUpdateVoterDetailsRequest()
    {
    }

    public AdminUpdateVoterDetailsRequest(AdminGetVoterResponse voterDetails)
    {
        Email = voterDetails.Email;
        FirstName = voterDetails.FirstName;
        LastName = voterDetails.LastName;
        Address = voterDetails.Address;
        DateOfBirth = voterDetails.DateOfBirth;
        Country = voterDetails.Country;
        NewUser = voterDetails.NewUser;
        IsVerified = voterDetails.IsVerified;
        IsCandidate = voterDetails.IsCandidate;
        CandidateName = voterDetails.CandidateName;
        CandidateDescription = voterDetails.CandidateDescription;
    }
}
