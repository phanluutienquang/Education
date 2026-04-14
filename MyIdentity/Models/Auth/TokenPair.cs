using System;
using MyEducation.MyDomain.Common;

namespace MyEducation.MyIdentity.Models.Auth
{
    /// <summary>
    /// Token Pair - Cặp Access Token và Refresh Token
    /// Dùng để trả về response khi login thành công
    /// </summary>
    public class TokenPair
    {
        /// <summary>
        /// Access Token (JWT) - Ngắn hạn (15 phút)
        /// Dùng để gọi API
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Refresh Token - Dài hạn (7 ngày)
        /// Dùng để xin lại access token mới khi hết hạn
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian hết hạn của Access Token (UTC)
        /// </summary>
        public DateTime AccessTokenExpiration { get; set; }

        /// <summary>
        /// Thời gian hết hạn của Refresh Token (UTC)
        /// </summary>
        public DateTime RefreshTokenExpiration { get; set; }

        /// <summary>
        /// Loại token (luôn là "Bearer")
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Client ID associated with this token pair
        /// </summary>
        public string ClientId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Refresh Token Entity - Lưu thông tin refresh token trong DB
    /// Để có thể revoke khi cần
    /// </summary>
    public class RefreshToken : BaseEntity
    {
        /// <summary>
        /// Client ID sở hữu token này
        /// </summary>
        public int ClientSourceId { get; protected set; }

        /// <summary>
        /// Reference to ClientSource
        /// </summary>
        public ClientSource ClientSource { get; protected set; } = null!;

        /// <summary>
        /// Refresh token value (hashed)
        /// </summary>
        public string Token { get; protected set; } = string.Empty;

        /// <summary>
        /// JWT ID (jti) của Access Token đi kèm
        /// Dùng để link giữa Access Token và Refresh Token
        /// </summary>
        public string JwtId { get; protected set; } = string.Empty;

        /// <summary>
        /// Thời gian hết hạn
        /// </summary>
        public DateTime ExpiryDate { get; protected set; }

        /// <summary>
        /// Đã được dùng chưa (mỗi refresh token chỉ dùng 1 lần)
        /// </summary>
        public bool IsUsed { get; protected set; } = false;

        /// <summary>
        /// Đã bị revoke (thu hồi) chưa
        /// </summary>
        public bool IsRevoked { get; protected set; } = false;

        /// <summary>
        /// IP address đã sử dụng token này
        /// </summary>
        public string? UsedByIP { get; protected set; }

        /// <summary>
        /// Thời điểm tạo
        /// </summary>
        public DateTime CreatedDate { get; protected set; } = DateTime.UtcNow;

        // ==========================================
        // BUSINESS LOGIC METHODS
        // ==========================================

        /// <summary>
        /// Đánh dấu token đã được sử dụng
        /// </summary>
        public void MarkAsUsed(string ipAddress)
        {
            IsUsed = true;
            UsedByIP = ipAddress;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Revoke (thu hồi) token
        /// </summary>
        public void Revoke()
        {
            IsRevoked = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Kiểm tra token còn hợp lệ không
        /// </summary>
        public bool IsValid()
        {
            return !IsRevoked && !IsUsed && ExpiryDate > DateTime.UtcNow;
        }

        // ==========================================
        // FACTORY METHODS
        // ==========================================

        /// <summary>
        /// Tạo mới Refresh Token
        /// </summary>
        public static RefreshToken Create(
            int clientSourceId,
            string token,
            string jwtId,
            DateTime expiryDate)
        {
            return new RefreshToken
            {
                ClientSourceId = clientSourceId,
                Token = token,
                JwtId = jwtId,
                ExpiryDate = expiryDate
            };
        }
    }
}
