namespace MyEducation.MyIdentity.Models.Auth;

public class AuthenticationResult
    {
        public bool IsSuccess => Type == AuthenticationResultType.Success;
        public AuthenticationResultType Type { get; set; }
        public string? Message { get; set; }
        public string? ClientId { get; set; }
        public List<string>? Scopes { get; set; }
    }