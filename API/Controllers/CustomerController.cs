using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.DTO;
using VotingSystem.API.Enums;

namespace VotingSystem.API.Controllers;

//https://localhost:44389/api/Customer/ name of method
[Route("api/[controller]/[action]")]
[ApiController]
public class CustomerController : ControllerBase
{
    //https://localhost:44389/api/Customer/GetCustomerDetails?id=1
    [HttpGet]
    //[Authorize("Voter", "Candidate")]
    public ActionResult GetCustomerDetails(int id)
    {
        //db call

        var result = new GetCustomerAccountDetailsResponse()
        {
            UserId = 1,
            FirstName = "Kat",
          //  Country = CustomerCountry.UnitedKingdom,
            NewUser = true,
        };

        return Ok(result);
    }
}
