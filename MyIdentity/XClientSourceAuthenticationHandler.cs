using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyEducation.MyApplication.Interfaces.Auth;
using MyEducation.MyIdentity.Interface;
using MyEducation.MyIdentity.Models.Auth;

namespace MyEducation.MyIdentity;

public class XClientSourceAuthenticationHandler : AuthenticationHandler<XClientSourceAuthenticationOptions>
{
    private readonly ITokenService _tokenService;
        private readonly IClientSourceCacheService _clientCacheService;
        private readonly IClientSourceRepository _clientSourceRepository;
        private readonly ILogger<XClientSourceAuthenticationHandler> _logger;
        private readonly ActivitySource _activitySource = new("Education.Authorization");

    public XClientSourceAuthenticationHandler(
        IOptionsMonitor<XClientSourceAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IClientSourceCacheService clientSourceCacheService,
        IClientSourceRepository clientSourceRepository,
        ITokenService tokenService) : base(options, logger, encoder, clock)
    {
        _clientCacheService = clientSourceCacheService;
        _clientSourceRepository = clientSourceRepository;
        _tokenService = tokenService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        using var activity = _activitySource.StartActivity("AuthenticateRequest");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var clientSource = Context.Request.Headers["X-Client-Source"];
            var tokenHeader = Context.Request.Headers["Token"];
            var refreshTokenHeader = Context.Request.Headers["Refresh-Token"];

            // Validate required headers
                if (clientSource.Count == 0)
                {
                    _logger.LogWarning("Missing X-Client-Source header from IP {IP}", GetClientIp());
                    return AuthenticateResult.Fail("Missing X-Client-Source header");
                }

                if (tokenHeader.Count == 0)
                {
                    _logger.LogWarning("Missing Token header from IP {IP}", GetClientIp());
                    return AuthenticateResult.Fail("Missing Token header");
                }

                var clientSourceValue = clientSource.FirstOrDefault();
                var tokenValue = tokenHeader.FirstOrDefault();

                _logger.LogDebug("Authenticating client {ClientId} from IP {IP}", 
                    clientSourceValue, GetClientIp());

                // Validate token
                var validationResult = await _tokenService.ValidateAccessToken(tokenValue); 

                if(!validationResult.IsSuccess)
                {
                   _logger.LogWarning("Token validation failed for client {ClientId}: {Reason}", 
                        clientSourceValue, validationResult.Type);
                   
                   activity?.SetTag("auth.failure", validationResult.Type.ToString());
                   return AuthenticateResult.Fail($"Token validation failed: {validationResult.Type}");
                }

                 // Additional client validation
                if (!await Options.ClientValidator(clientSourceValue, null!, CreatePrincipalFromValidationResult(validationResult)))
                {
                    _logger.LogWarning("Client validator rejected client {ClientId}", clientSourceValue);
                    return AuthenticateResult.Fail("Invalid Client Source");
                }

                // Add claims to principal
                var claimsIdentity = new ClaimsIdentity();
                claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, validationResult.ClientId));
                claimsIdentity.AddClaim(new Claim("PartnerId", clientSourceValue));

                if (validationResult.Scopes != null)
                {
                    foreach (var scope in validationResult.Scopes)
                    {
                        claimsIdentity.AddClaim(new Claim("scope", scope));
                    }
                }

                var principal = new ClaimsPrincipal(claimsIdentity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                stopwatch.Stop();
                _logger.LogInformation("Successfully authenticated client {ClientId} in {ElapsedMilliseconds} ms", 
                    clientSourceValue, stopwatch.ElapsedMilliseconds);

                activity?.SetTag("auth.success", true);
                activity?.SetTag("client.id", clientSourceValue);

                return AuthenticateResult.Success(ticket);
            }
        catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Authentication error after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                activity?.SetTag("auth.error", ex.Message);
                
                return AuthenticateResult.Fail("Authentication failed");
            }
    }
    private string? GetClientIp()
    {
        if(Context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            return forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
        }
        return Context.Connection.RemoteIpAddress?.ToString();
    }

    private ClaimsPrincipal? CreateClaimsPrincipal(AuthenticationResult result)
    {
        if (string.IsNullOrEmpty(result.ClientId))
                return null;

        var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, result.ClientId));
            return new ClaimsPrincipal(identity);
    }

    private ClaimsPrincipal CreatePrincipalFromValidationResult(AuthenticationResult result)
    {
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, result.ClientId ?? string.Empty));
        
        if (result.Scopes != null)
        {
            foreach (var scope in result.Scopes)
            {
                claimsIdentity.AddClaim(new Claim("scope", scope));
            }
        }
        
        return new ClaimsPrincipal(claimsIdentity);
    }
}
