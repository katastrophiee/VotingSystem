using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ElectionController(IElectionProvider electionProvider) : Controller
{
    private readonly IElectionProvider _electionProvider = electionProvider;

    [HttpGet]
    public async Task<ActionResult> GetCustomerOngoingElections(int customerId)
    {
        var response = await _electionProvider.GetCustomerOngoingElections(customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetCustomerVotedInElections(int customerId)
    {
        var response = await _electionProvider.GetCustomerVotedInElections(customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetRecentlyEndedElections(int customerId)
    {
        var response = await _electionProvider.GetRecentlyEndedElections(customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetCustomerUpcomingElections(int customerId)
    {
        var response = await _electionProvider.GetCustomerUpcomingElections(customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetElection(int electionId, int customerId)
    {
        var response = await _electionProvider.GetElection(electionId, customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}
