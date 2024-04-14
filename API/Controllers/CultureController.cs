using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace VotingSystem.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class CultureController : Controller
{
    public async Task<IActionResult> SetCulture(string culture, string redirectUri)
    {
        if (!string.IsNullOrWhiteSpace(culture))
        {
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture)));
        }

        return LocalRedirect(redirectUri);
    }
}
