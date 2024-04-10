using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class DocumentController(IDocumentProvider documentProvider) : Controller
{
    private readonly IDocumentProvider _documentProvider = documentProvider;

    [HttpPost]
    public async Task<ActionResult> PostUploadCustomerDocument(Document document)
    {
        var response = await _documentProvider.PostUploadCustomerDocument(document);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetCustomerDocuments(int customerId)
    {
        var response = await _documentProvider.GetCustomerDocuments(customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetCurrentCustomerDocument(int customerId)
    {
        var response = await _documentProvider.GetCurrentCustomerDocument(customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}
