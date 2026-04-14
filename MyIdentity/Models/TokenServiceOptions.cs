using Microsoft.IdentityModel.Tokens;

public class TokenServiceOptions
{
    
        public string IssuerSigningKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; set; } = 15;
        public int RefreshTokenExpirationDays { get; set; } = 7;
        public bool UseRSASigning { get; set; } = false;
        public string? RSAKeyPath { get; set; }
}