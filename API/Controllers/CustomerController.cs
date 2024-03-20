using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO;
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
    public async Task<ActionResult<GetCustomerAccountDetailsResponse>> GetCustomerDetails(int id)
    {
        if (id == 0)
        {
            //var errorResponse = ExceptionHelper.CreateMovexErrorResponse(MovexErrorCode.AdminPrices_NoModelPassedIn);
            //return BadRequest(errorResponse);
            return BadRequest("error");
        }

        var response = await _customerProvider.GetCustomerAccountDetails(id);

        //return response.ErrorResponse is null
        //    ? Ok(response)
        //    : StatusCode(response.ErrorResponse.DefaultHttpResponse, response.ErrorResponse);

        return response is not null
        ? Ok(response)
        : Ok("Error");
    }
}
