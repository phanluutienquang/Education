using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MyEducation.MyApplication.Interfaces.Graph;
public class GraphAuthService : IGraphAuthService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public GraphAuthService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var tenantId = _configuration["AzureAdB2CGraph:TenantId"];
        var clientId = _configuration["AzureAdB2CGraph:ClientId"];
        var clientSecret = _configuration["AzureAdB2CGraph:ClientSecret"];
        var tokenUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

        var requestBody = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "scope", "https://graph.microsoft.com/.default" },
            { "grant_type", "client_credentials" }
        };

        var requestContent = new FormUrlEncodedContent(requestBody);
        var response = await _httpClient.PostAsync(tokenUrl, requestContent);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
            return tokenResponse["access_token"];
        }

        throw new Exception("Failed to retrieve access token");
    }
}