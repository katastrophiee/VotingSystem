using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController(IAuthProvider authProvider) : ControllerBase
{
    private readonly IAuthProvider _authProvider = authProvider;

    [HttpPost]
    public async Task<ActionResult> PostVoterLogin(LoginRequest request)
    {
        var response = await _authProvider.VoterLogin(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpPost]
    public async Task<ActionResult> PostCreateVoterAccount(CreateVoterAccountRequest request)
    {
        var response = await _authProvider.CreateVoterAccount(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpPost]
    public async Task<ActionResult> PostAdminLogin(LoginRequest request)
    {
        var response = await _authProvider.AdminLogin(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> PostCreateAdminAccount(AdminCreateAdminAccountRequest request)
    {
        var response = await _authProvider.CreateAdminAccount(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpPut]
    public async Task<ActionResult> PutUpdatePasswordRequest(UpdatePasswordRequest request)
    {
        var response = await _authProvider.PutUpdatePassword(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}
