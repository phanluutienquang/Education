using System.Threading.Tasks;
using MyEducation.MyIdentity.Models.Auth;

public interface ITokenService
    {
        Task<TokenPair> GenerateTokenPair(ClientSource client);
        Task<AuthenticationResult> ValidateAccessToken(string token);
        Task<AuthenticationResult> RefreshAccessToken(string refreshToken);
        Task RevokeRefreshToken(string refreshToken, string reason);
        Task<bool> IsRefreshTokenRevoked(string refreshToken);
    }