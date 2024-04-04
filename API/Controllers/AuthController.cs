﻿using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController(IAuthProvider authProvider) : ControllerBase
{
    private readonly IAuthProvider _authProvider = authProvider;

    [HttpPost]
    public async Task<ActionResult> PostCustomerLogin(LoginRequest request)
    {
        var response = await _authProvider.CustomerLogin(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpPost]
    public async Task<ActionResult> PostCreateAccount(CreateAccountRequest request)
    {
        var response = await _authProvider.CreateAccount(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}