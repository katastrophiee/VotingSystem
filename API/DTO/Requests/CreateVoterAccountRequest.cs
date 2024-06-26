﻿using System.ComponentModel.DataAnnotations;
using VotingSystem.API.DTO.ComponentTypes;

namespace VotingSystem.API.DTO.Requests;

public class CreateVoterAccountRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [PasswordRules]
    public string Password { get; set; }
}
