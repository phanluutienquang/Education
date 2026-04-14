using System.Threading.Tasks;
using MyEducation.MyIdentity.Models.Auth;

namespace MyEducation.MyApplication.Interfaces.Auth
{
    /// <summary>
    /// Interface cho Service quản lý Authentication
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Đăng nhập với Client ID và API Secret
        /// Trả về Access Token + Refresh Token
        /// </summary>
        Task<TokenPair> LoginAsync(string clientId, string apiSecret, string ipAddress);

        /// <summary>
        /// Làm mới access token bằng refresh token
        /// </summary>
        Task<TokenPair> RefreshTokenAsync(string refreshToken, string ipAddress);

        /// <summary>
        /// Thu hồi refresh token
        /// </summary>
        Task RevokeTokenAsync(string refreshToken);

        /// <summary>
        /// Xác thực JWT token
        /// Trả về ClientId nếu valid
        /// </summary>
        Task<string?> ValidateTokenAsync(string token);

        /// <summary>
        /// Lấy thông tin ClientSource từ cache hoặc DB
        /// </summary>
        Task<ClientSource?> GetClientSourceAsync(string clientId);
    }
}
