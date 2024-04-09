using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

//https://localhost:44389/api/Customer/ name of method
[Route("api/[controller]/[action]")]
[ApiController]
public class CustomerController(ICustomerProvider customerProvider) : ControllerBase
{
    private readonly ICustomerProvider _customerProvider = customerProvider;

    //https://localhost:44389/api/Customer/GetCustomerDetails?id=1
    [HttpGet]
    //[Authorize(Roles = ("Voter, Candidate"))]
    public async Task<ActionResult> GetCustomerDetails(int id)
    {
        var response = await _customerProvider.GetCustomerAccountDetails(id);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetCustomerVotingHistory(int customerId)
    {
        var response = await _customerProvider.GetCustomerVotingHistory(customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpPost]
    public async Task<ActionResult> PostUploadCustomerDocument(Document document)
    {
        var response = await _customerProvider.PostUploadCustomerDocument(document);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetCustomerDocuments(int customerId)
    {
        var response = await _customerProvider.GetCustomerDocuments(customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpPut]
    public async Task<ActionResult> PutUpdateCustomerProfile(UpdateCustomerProfileRequest request)
    {
        var response = await _customerProvider.PutUpdateCustomerProfile(request);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
    
    [HttpGet]
    public async Task<ActionResult> GetCustomerOngoingElections(int customerId)
    {
        var response = await _customerProvider.GetCustomerOngoingElections(customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetCurrentCustomerDocument(int customerId)
    {
        var response = await _customerProvider.GetCurrentCustomerDocument(customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

}
