
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MyEducation.MyApplication.Interfaces.Common;

namespace MyEducation.MyApplication.Services.Common
{
    public class UserClaim : IUserClaims
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

    public UserClaim(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetClaimValue(string claimType)
    {
        var claim = _httpContextAccessor.HttpContext?.User.FindFirst(claimType);
        return claim?.Value ?? "";
    }

    public string GetCurrentUserEmail()
    {
        return GetClaimValue(ClaimTypes.Email);
    }

    public string GetCurrentUserId()
    {
        return GetClaimValue(ClaimTypes.NameIdentifier);
    }

    public List<string> GetUserRoles()
    {
        var roles = _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role);
        return roles != null ? roles.Select(r => r.Value).ToList() : new List<string>();
    }

    public int GetUserId()
    {
        var userId = GetClaimValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userId, out var id) ? id : 0;
    }
}
}