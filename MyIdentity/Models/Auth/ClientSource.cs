using MyEducation.MyDomain.Common;
using System;
using System.Collections.Generic;

namespace MyEducation.MyIdentity.Models.Auth
{
    /// <summary>
    /// Client Source - Lưu thông tin các ứng dụng client được phép gọi API
    /// Tương ứng với thiết kế trong ARCHITECTURE.md (Section 8: Data Models)
    /// </summary>
    public class ClientSource : BaseEntity
    {
        /// <summary>
        /// Unique Client Identifier (API Key public)
        /// Ví dụ: "mobile-app", "web-portal", "partner-api"
        /// </summary>
        public string ClientId { get; protected set; } = string.Empty;

        /// <summary>
        /// Tên hiển thị của client app
        /// </summary>
        public string ClientName { get; protected set; } = string.Empty;

        /// <summary>
        /// API Secret (hashed) - Dùng để authenticate khi login
        /// </summary>
        public string ApiSecret { get; protected set; } = string.Empty;

        /// <summary>
        /// Trạng thái hoạt động của client
        /// </summary>
        public bool IsEnabled { get; protected set; } = true;

        /// <summary>
        /// Thời gian hiệu lực bắt đầu
        /// </summary>
        public DateTime ValidFrom { get; protected set; } = DateTime.UtcNow;

        /// <summary>
        /// Thời gian hết hạn (null = vô thời hạn)
        /// </summary>
        public DateTime? ValidTo { get; protected set; }

        /// <summary>
        /// Danh sách scopes/permissions được phép
        /// Ví dụ: ["courses.read", "courses.write", "exams.submit"]
        /// </summary>
        public List<string> AllowedScopes { get; protected set; } = new();

        /// <summary>
        /// Danh sách IPs được phép (nếu có, empty = tất cả)
        /// </summary>
        public List<string> AllowedIPs { get; protected set; } = new();

        /// <summary>
        /// Giới hạn số requests per minute (Rate Limiting)
        /// </summary>
        public int RateLimitPerMinute { get; protected set; } = 100;

        /// <summary>
        /// Thời điểm sử dụng cuối cùng
        /// </summary>
        public DateTime? LastUsedAt { get; protected set; }

        /// <summary>
        /// Metadata bổ sung (JSON)
        /// </summary>
        public string? Metadata { get; protected set; }

        // ==========================================
        // BUSINESS LOGIC METHODS
        // ==========================================

        /// <summary>
        /// Kiểm tra client có còn hiệu lực không
        /// </summary>
        public bool IsValid()
        {
            if (!IsEnabled) return false;
            
            var now = DateTime.UtcNow;
            if (now < ValidFrom) return false;
            
            if (ValidTo.HasValue && now > ValidTo.Value) return false;
            
            return true;
        }

        /// <summary>
        /// Kiểm tra IP có được phép không
        /// </summary>
        public bool IsIPAllowed(string ipAddress)
        {
            // Nếu không có whitelist IP nào => cho phép tất cả
            if (AllowedIPs == null || AllowedIPs.Count == 0) return true;
            
            return AllowedIPs.Contains(ipAddress);
        }

        /// <summary>
        /// Kiểm tra client có scope yêu cầu không
        /// </summary>
        public bool HasScope(string scope)
        {
            if (AllowedScopes == null || AllowedScopes.Count == 0) return false;
            
            return AllowedScopes.Contains(scope);
        }

        /// <summary>
        /// Cập nhật thời gian sử dụng cuối cùng
        /// </summary>
        public void UpdateLastUsed()
        {
            LastUsedAt = DateTime.UtcNow;
        }

        // ==========================================
        // FACTORY METHODS (Khởi tạo an toàn)
        // ==========================================

        /// <summary>
        /// Factory method để tạo mới ClientSource
        /// </summary>
        public static ClientSource Create(
            string clientId,
            string clientName,
            string apiSecret,
            int rateLimitPerMinute = 100,
            List<string>? allowedScopes = null,
            List<string>? allowedIPs = null)
        {
            return new ClientSource
            {
                ClientId = clientId,
                ClientName = clientName,
                ApiSecret = apiSecret, // Should be hashed before passing in
                IsEnabled = true,
                ValidFrom = DateTime.UtcNow,
                RateLimitPerMinute = rateLimitPerMinute,
                AllowedScopes = allowedScopes ?? new List<string>(),
                AllowedIPs = allowedIPs ?? new List<string>()
            };
        }

        /// <summary>
        /// Cập nhật thông tin client
        /// </summary>
        public void Update(
            string? clientName = null,
            bool? isEnabled = null,
            DateTime? validTo = null,
            List<string>? allowedScopes = null,
            List<string>? allowedIPs = null,
            int? rateLimitPerMinute = null)
        {
            if (clientName != null) ClientName = clientName;
            if (isEnabled.HasValue) IsEnabled = isEnabled.Value;
            if (validTo.HasValue) ValidTo = validTo;
            if (allowedScopes != null) AllowedScopes = allowedScopes;
            if (allowedIPs != null) AllowedIPs = allowedIPs;
            if (rateLimitPerMinute.HasValue) RateLimitPerMinute = rateLimitPerMinute.Value;
            
            MarkAsUpdated("system");
        }
    }
}
