using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Providers;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AdminController(IAdminProvider adminProvider) : Controller
{
    private readonly IAdminProvider _adminProvider = adminProvider;

    [HttpPost]
    public async Task<ActionResult> GetCustomers(AdminGetCustomersRequest request)
    {
        var response = await _adminProvider.GetCustomers(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetCustomerDetails(int customerId, int adminId)
    {
        var response = await _adminProvider.GetCustomerDetails(customerId, adminId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpPost]
    public async Task<ActionResult> PostAdminVerifyId(AdminVerifyIdRequest request)
    {
        var response = await _adminProvider.AdminVerifyId(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpPost]
    public async Task<ActionResult> PostAddElection(AddElectionRequest request)
    {
        var response = await _adminProvider.AddElection(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetCandidate(int customerId, int adminId)
    {
        var response = await _adminProvider.GetCandidate(customerId, adminId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}
