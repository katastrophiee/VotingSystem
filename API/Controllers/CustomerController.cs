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
        // not needed here as the check for if customer exists in provider will handle null/0
        // keeping for future reference
        //if (id == 0)
        //    return BadRequest(new ErrorResponse(ErrorCode.NoModelPassedIn));

        var response = await _customerProvider.GetCustomerAccountDetails(id);

        return response.Error is null
            ? Ok(response.Data)
            : StatusCode(response.Error.StatusCode, response.Error);
    }

    [HttpGet]
    public async Task<ActionResult<string>> Test()
    {
        return Ok("Test");
    }
}
