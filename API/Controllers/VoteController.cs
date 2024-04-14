using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class VoteController(IVoteProvider voteProvider) : Controller
{
    private readonly IVoteProvider _voteProvider = voteProvider;

    [Authorize(Roles = "Voter, Candidate")]
    [HttpPost]
    public async Task<ActionResult> AddVoterVote(AddVoterVoteRequest request)
    {
        var response = await _voteProvider.AddVoterVote(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Voter, Candidate")]
    [HttpGet]
    public async Task<ActionResult> GetVoterVotingHistory(int voterId)
    {
        var response = await _voteProvider.GetVoterVotingHistory(voterId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}
