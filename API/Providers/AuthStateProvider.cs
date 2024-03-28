using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

public class AuthStateProvider : AuthenticationStateProvider
{
    private IJSRuntime _jsRuntime;

    public AuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string accessToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");

        ClaimsIdentity identity;
        if (!string.IsNullOrEmpty(accessToken))
        {
            identity = new ClaimsIdentity(new[] { new Claim("accessToken", accessToken) });
        }
        else
        {
            identity = new ClaimsIdentity();
        }

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public void MarkUserAsAuthenticated(ClaimsPrincipal user)
    {
        var authState = Task.FromResult(new AuthenticationState(user));
        NotifyAuthenticationStateChanged(authState);
    }
}
