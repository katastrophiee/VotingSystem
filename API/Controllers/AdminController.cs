using Microsoft.AspNetCore.Mvc;
using VotingSystem.API.Interfaces.Provider;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AdminController(IAdminProvider adminProvider) : Controller
{
    private readonly IAdminProvider _adminProvider = adminProvider;
}
