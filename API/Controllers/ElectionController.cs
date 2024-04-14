using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ElectionController(IElectionProvider electionProvider) : Controller
{
    private readonly IElectionProvider _electionProvider = electionProvider;

    [HttpGet]
    public async Task<ActionResult> GetVoterOngoingElections(int voterId)
    {
        var response = await _electionProvider.GetVoterOngoingElections(voterId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetVoterVotedInElections(int voterId)
    {
        var response = await _electionProvider.GetVoterVotedInElections(voterId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetRecentlyEndedElections(int voterId)
    {
        var response = await _electionProvider.GetRecentlyEndedElections(voterId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetVoterUpcomingElections(int voterId)
    {
        var response = await _electionProvider.GetVoterUpcomingElections(voterId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetElection(int electionId, int voterId)
    {
        var response = await _electionProvider.GetElection(electionId, voterId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpPost]
    public async Task<ActionResult> PostGetElections(GetElectionsRequest request)
    {
        var response = await _electionProvider.GetElections(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}
