using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

//https://localhost:44389/api/Voter/ name of method
[Route("api/[controller]/[action]")]
[ApiController]
public class VoterController(IVoterProvider voterProvider) : ControllerBase
{
    private readonly IVoterProvider _voterProvider = voterProvider;

    //[Authorize(Roles = ("Voter, Candidate"))]
    [HttpGet]
    public async Task<ActionResult> GetVoterDetails(int voterId)
    {
        var response = await _voterProvider.GetVoterAccountDetails(voterId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpPut]
    public async Task<ActionResult> PutUpdateVoterProfile(UpdateVoterProfileRequest request)
    {
        var response = await _voterProvider.PutUpdateVoterProfile(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetActiveCandidates(int voterId)
    {
        var response = await _voterProvider.GetActiveCandidates(voterId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpPut]
    public async Task<ActionResult> PutMakeVoterACandidate(BecomeCandidateRequest request)
    {
        var response = await _voterProvider.MakeVoterACandidate(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetCandidate(int voterId, int candidateId)
    {
        var response = await _voterProvider.GetCandidate(voterId, candidateId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpPut]
    public async Task<ActionResult> PutUpateCandidate(UpdateCandidateRequest request)
    {
        var response = await _voterProvider.UpdateCandidate(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetInPersonVotingEligibility(int voterId)
    {
        var response = await _voterProvider.GetInPersonVotingEligibility(voterId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}
