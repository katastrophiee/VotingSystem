using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.Enums;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AdminController(IAdminProvider adminProvider) : Controller
{
    private readonly IAdminProvider _adminProvider = adminProvider;

    [Authorize(Roles = "Admin, Observer")]
    [HttpPost]
    public async Task<ActionResult> PostGetVoters(AdminGetVotersRequest request)
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
    public async Task<ActionResult> PostAddElection(AdminAddElectionRequest request)
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

    [Authorize(Roles = "Admin, Observer")]
    [HttpPost]
    public async Task<ActionResult> PostGetAdmins(AdminGetAdminRequest request)
    {
        var response = await _adminProvider.GetAdmins(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpGet]
    public async Task<ActionResult> GetAdmin(int currentAdminId, int requestedAdminId)
    {
        var response = await _adminProvider.GetAdmin(currentAdminId, requestedAdminId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpPut]
    public async Task<ActionResult> PutUpdateAdmin(AdminUpdateAdminRequest request)
    {
        var response = await _adminProvider.UpdateAdmin(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpPost]
    public async Task<ActionResult> PostAddTask(AdminAddTaskRequest request)
    {
        var response = await _adminProvider.AddTask(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpPost]
    public async Task<ActionResult> PostGetTasks(AdminGetTasksRequest request)
    {
        var response = await _adminProvider.GetTasks(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpGet]
    public async Task<ActionResult> GetTask(int taskId, int adminId)
    {
        var response = await _adminProvider.GetTask(taskId, adminId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpDelete]
    public async Task<ActionResult> DeleteTask(int taskId, int adminId)
    {
        var response = await _adminProvider.DeleteTask(taskId, adminId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpPut]
    public async Task<ActionResult> PutUpdateTask(AdminUpdateTaskRequest request)
    {
        var response = await _adminProvider.UpdateTask(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpPut]
    public async Task<ActionResult> PutUpdateVoterDetails(AdminUpdateVoterDetailsRequest request)
    {
        var response = await _adminProvider.UpdateVoterDetails(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpGet]
    public async Task<ActionResult> GetElections(AdminGetElectionsRequest request)
    {
        var response = await _adminProvider.GetElections(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpGet]
    public async Task<ActionResult> GetElection(int electionId, int adminId)
    {
        var response = await _adminProvider.GetElection(electionId, adminId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpDelete]
    public async Task<ActionResult> DeleteElection(int electionId, int adminId)
    {
        var response = await _adminProvider.DeleteElection(electionId, adminId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpPut]
    public async Task<ActionResult> PutUpdateElection(AdminUpdateElectionRequest request)
    {
        var response = await _adminProvider.UpdateElection(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Admin, Observer")]
    [HttpGet]
    public async Task<ActionResult> GetAvailableVotingSystems(UserCountry country, int adminId)
    {
        var response = await _adminProvider.GetAvailableVotingSystems(country, adminId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}
