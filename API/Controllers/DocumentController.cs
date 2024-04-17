using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class DocumentController(IDocumentProvider documentProvider) : Controller
{
    private readonly IDocumentProvider _documentProvider = documentProvider;

    [Authorize(Roles = "Voter, Candidate")]
    [HttpPost]
    public async Task<ActionResult> PostUploadVoterDocument(Document document)
    {
        var response = await _documentProvider.UploadVoterDocument(document);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Voter, Candidate")]
    [HttpGet]
    public async Task<ActionResult> GetVoterDocuments(int voterId)
    {
        var response = await _documentProvider.GetVoterDocuments(voterId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [Authorize(Roles = "Voter, Candidate")]
    [HttpGet]
    public async Task<ActionResult> GetCurrentVoterDocument(int voterId)
    {
        var response = await _documentProvider.GetCurrentVoterDocument(voterId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}
