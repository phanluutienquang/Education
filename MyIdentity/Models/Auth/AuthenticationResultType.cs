namespace MyEducation.MyIdentity.Models.Auth;

public enum AuthenticationResultType
    {
        Success,
        InvalidToken,
        ExpiredToken,
        InvalidClientSource,
        ClientDisabled,
        RateLimitExceeded,
        IPNotAllowed,
        ScopeNotAuthorized
    }