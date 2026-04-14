using System;
using System.ComponentModel.DataAnnotations;

namespace MyEducation.MyApplication.DTOs.Auth
{
    /// <summary>
    /// Request model cho login endpoint
    /// </summary>
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Client ID is required")]
        [StringLength(100, MinimumLength = 3)]
        public string ClientId { get; set; } = string.Empty;

        [Required(ErrorMessage = "API Secret is required")]
        [StringLength(500, MinimumLength = 10)]
        public string ApiSecret { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model cho refresh token endpoint
    /// </summary>
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "Refresh Token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response model cho các endpoints trả về token
    /// </summary>
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public TokenPairData? Data { get; set; }
    }

    /// <summary>
    /// Token Pair data để trả về client
    /// </summary>
    public class TokenPairData
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiration { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; } // Seconds until access token expires
    }

    /// <summary>
    /// Revoke token request
    /// </summary>
    public class RevokeTokenRequestDto
    {
        [Required(ErrorMessage = "Refresh Token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
