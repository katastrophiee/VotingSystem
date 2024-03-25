using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

//https://localhost:44389/api/Customer/ name of method
[Route("api/[controller]/[action]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerProvider _customerProvider;

    public CustomerController(
        ICustomerProvider customerProvider)
    {
        _customerProvider = customerProvider;
    }

    //https://localhost:44389/api/Customer/GetCustomerDetails?id=1
    [HttpGet]
    //[Authorize("Voter", "Candidate")]
    public async Task<ActionResult> GetCustomerDetails(int id)
    {
        var response = await _customerProvider.GetCustomerAccountDetails(id);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult> GetCustomerVotingHistory(int id)
    {
        var response = await _customerProvider.GetCustomerVotingHistory(id);

        return response.Error is null
            ? Ok(response.Data)
            : BadRequest(response.Error);
    }

    [HttpGet]
    public async Task<ActionResult<string>> Test()
    {
        return Ok("Test");
    }
}
