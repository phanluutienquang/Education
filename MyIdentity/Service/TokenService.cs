using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyEducation.MyIdentity.Interface;
using MyEducation.MyIdentity.Models.Auth;

public class TokenService : ITokenService
{
    private readonly IClientSourceCacheService _clientSourceCacheService;
    private readonly IClientSourceRepository _clientRepository;
    private readonly ITokenPairRepository _tokenPairRepository;
    private readonly ILogger<TokenService> _logger;

    private readonly TokenServiceOptions _options;
    private readonly SecurityKey? _symmetricKey;
    private readonly RSA? _rsaKey;

    public TokenService(IClientSourceCacheService clientSourceCacheService, 
                 IClientSourceRepository clientRepository,
                 ITokenPairRepository tokenPairRepository, 
                 ILogger<TokenService> logger,
                 IOptions<TokenServiceOptions> options)
    {
        _clientSourceCacheService = clientSourceCacheService;
        _clientRepository = clientRepository;
        _tokenPairRepository = tokenPairRepository;
        _logger = logger;
        _options = options.Value;

        // Initialize signing key based on configuration
        if (_options.UseRSASigning)
        {
            _rsaKey = RSA.Create(2048);
        }
        else
        {
            _symmetricKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_options.IssuerSigningKey));
        }
    }

    public async Task<TokenPair> GenerateTokenPair(ClientSource client)
    {
        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes);
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays);

        var accessToken = GenerateAccessToken(client, accessTokenExpiration);
        var refreshToken = GenerateRefreshToken();

        // Lưu refresh token vào DB
        var refreshTokenEntity = RefreshToken.Create(
            client.Id,
            refreshToken,
            Guid.NewGuid().ToString(),
            refreshTokenExpiration
        );
        await _tokenPairRepository.AddAsync(refreshTokenEntity);

        return new TokenPair
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration,
            ClientId = client.ClientId
        };
    }

    // public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(string clientId, string ipAddress)
    // {
    //     var client = await _clientSourceCacheService.GetByIdAsync(clientId);
    //     if (client == null)
    //     {
    //         throw new ArgumentException("Invalid client ID");
    //     }
    // }

    public async Task<AuthenticationResult> ValidateAccessToken(string token)
    {
       try
        {
             var handler = new JwtSecurityTokenHandler();
                
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _options.UseRSASigning 
                        ? new RsaSecurityKey(_rsaKey!) { KeyId = Guid.NewGuid().ToString() }
                        : _symmetricKey!,
                    ValidateIssuer = true,
                    ValidIssuer = _options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _options.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireSignedTokens = true,
                    RequireAudience = true,
                    RequireExpirationTime = true
                };

            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;    

            if (jwtToken == null)
                {
                    return new AuthenticationResult
                    {
                        Type = AuthenticationResultType.InvalidToken,
                        Message = "Invalid token format"
                    };
                }

                var clientId = jwtToken.Subject;
                var client = await _clientRepository.GetByIdAsync(clientId);

                if (client == null)
                {
                    return new AuthenticationResult
                    {
                        Type = AuthenticationResultType.InvalidClientSource,
                        Message = "Client not found"
                    };
                }

                if (!client.IsValid())
                {
                    return new AuthenticationResult
                    {
                        Type = AuthenticationResultType.ClientDisabled,
                        Message = "Client is disabled"
                    };
                }

                //Update last used time
                await _clientRepository.UpdateLastUsedAsync(clientId);
                 return new AuthenticationResult
                {   
                    Type = AuthenticationResultType.Success,
                    ClientId = clientId,
                    Scopes = jwtToken.Claims
                        .Where(c => c.Type == "scope")
                        .Select(c => c.Value)
                        .ToList()
                };  
            }
       catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning(ex, "Token expired");
                return new AuthenticationResult
                {
                    Type = AuthenticationResultType.ExpiredToken,
                    Message = "Token has expired"
                };
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Invalid token");
                return new AuthenticationResult
                {
                    Type = AuthenticationResultType.InvalidToken,
                    Message = "Invalid token"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed");
                return new AuthenticationResult
                {
                    Type = AuthenticationResultType.InvalidToken,
                    Message = "Token validation failed"
                };
            }
    }

    public async Task<AuthenticationResult> RefreshAccessToken(string refreshToken)
    {
        // Validate the refresh token
        var principal = GetPrincipalFromExpiredToken(refreshToken);
        if (principal == null)
        {
            return new AuthenticationResult
            {
                Type = AuthenticationResultType.InvalidToken,
                Message = "Invalid refresh token"
            };
        }

        var clientId = principal.FindFirst("client_id")?.Value;
        var client = await _clientRepository.GetByIdAsync(clientId);

        if (client == null)
        {
            return new AuthenticationResult
            {
                Type = AuthenticationResultType.InvalidClientSource,
                Message = "Client not found"
            };
        }

        if (!client.IsValid())
        {
            return new AuthenticationResult
            {
                Type = AuthenticationResultType.ClientDisabled,
                Message = "Client is disabled"
            };
        }

        // Update last used time
        await _clientRepository.UpdateLastUsedAsync(clientId);

        return new AuthenticationResult
        {
            Type = AuthenticationResultType.Success,
            ClientId = clientId,
            Scopes = principal.FindAll("scope").Select(c => c.Value).ToList()
        };
    }

    public async Task RevokeRefreshToken(string refreshToken, string reason)
    {
        await Task.CompletedTask;
        _logger.LogInformation("Revoking refresh token: {RefreshToken}, Reason: {Reason}", refreshToken, reason);
    }

    public async Task<bool> IsRefreshTokenRevoked(string refreshToken)
    {
        await Task.CompletedTask;
        _logger.LogInformation("Checking if refresh token is revoked: {RefreshToken}", refreshToken);
        return false;
    }

    private string GenerateAccessToken(ClientSource client, DateTime datetime)
    {
        var handler = new JwtSecurityTokenHandler();

       // I chooose kind of signing key based on configuration
        var credentials = _options.UseRSASigning 
            ? new SigningCredentials(new RsaSecurityKey(_rsaKey!) { KeyId = Guid.NewGuid().ToString() }, SecurityAlgorithms.RsaSha256)
            : new SigningCredentials(_symmetricKey!, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        { 
            Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, client.ClientId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(datetime.ToUniversalTime()).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                }),
                Expires = datetime,
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = credentials
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _options.UseRSASigning 
                    ? new RsaSecurityKey(_rsaKey!) { KeyId = Guid.NewGuid().ToString() }
                    : _symmetricKey!,
                ValidateIssuer = true,
                ValidIssuer = _options.Issuer,
                ValidateAudience = true,
                ValidAudience = _options.Audience,
                ValidateLifetime = false // Don't validate lifetime for expired token refresh
            };

            var principal = handler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    private RSA LoadRSAKey(string keyPath)
        {
            var rsa = RSA.Create(2048);
            
            if (!File.Exists(keyPath))
            {
                // Generate new RSA key pair
                var privateKey = rsa.ExportRSAPrivateKeyPem();
                File.WriteAllText(keyPath, privateKey);
                _logger.LogInformation("Generated new RSA key pair at {KeyPath}", keyPath);
            }
            else
            {
                var privateKeyPem = File.ReadAllText(keyPath);
                rsa.ImportFromPem(privateKeyPem);
                _logger.LogInformation("Loaded RSA key pair from {KeyPath}", keyPath);
            }

            return rsa;
        }
}