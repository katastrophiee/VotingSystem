using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class VoteController(IVoteProvider voteProvider) : Controller
{
    private readonly IVoteProvider _voteProvider = voteProvider;

    [HttpPost]
    public async Task<ActionResult> AddCustomerVote(AddCustomerVoteRequest request)
    {
        var response = await _voteProvider.AddCustomerVote(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetCustomerVotingHistory(int customerId)
    {
        var response = await _voteProvider.GetCustomerVotingHistory(customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}
