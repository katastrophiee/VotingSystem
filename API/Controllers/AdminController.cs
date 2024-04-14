using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AdminController(IAdminProvider adminProvider) : Controller
{
    private readonly IAdminProvider _adminProvider = adminProvider;

    [Authorize(Roles = "Admin, Observer")]
    [HttpPost]
    public async Task<ActionResult> GetVoters(AdminGetVotersRequest request)
    {
        var response = await _adminProvider.GetVoters(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpGet]
    public async Task<ActionResult> GetVoterDetails(int voterId, int adminId)
    {
        var response = await _adminProvider.GetVoterDetails(voterId, adminId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpPost]
    public async Task<ActionResult> PostAdminVerifyId(AdminVerifyIdRequest request)
    {
        var response = await _adminProvider.AdminVerifyId(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpPost]
    public async Task<ActionResult> PostAddElection(AddElectionRequest request)
    {
        var response = await _adminProvider.AddElection(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpGet]
    public async Task<ActionResult> GetCandidate(int voterId, int adminId)
    {
        var response = await _adminProvider.GetCandidate(voterId, adminId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}
