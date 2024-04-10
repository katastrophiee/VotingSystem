using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

//https://localhost:44389/api/Customer/ name of method
[Route("api/[controller]/[action]")]
[ApiController]
public class CustomerController(ICustomerProvider customerProvider) : ControllerBase
{
    private readonly ICustomerProvider _customerProvider = customerProvider;

    //[Authorize(Roles = ("Voter, Candidate"))]
    [HttpGet]
    public async Task<ActionResult> GetCustomerDetails(int customerId)
    {
        var response = await _customerProvider.GetCustomerAccountDetails(customerId);

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
    public async Task<ActionResult> GetActiveCandidates(int customerId)
    {
        var response = await _customerProvider.GetActiveCandidates(customerId);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }
}
