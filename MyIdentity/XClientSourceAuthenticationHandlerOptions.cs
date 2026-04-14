using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

public class XClientSourceAuthenticationOptions : AuthenticationSchemeOptions
{
    public Func<string, SecurityToken, ClaimsPrincipal, Task<bool>> ClientValidator { get; set; } = (clientSource, token, principal) => Task.FromResult(false);
    public string IssuerSigningKey { get; set; } = string.Empty;
}