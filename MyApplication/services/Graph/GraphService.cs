    using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MyEducation.MyApplication.Interfaces.Graph;
using MyEducation.MyApplication.Models;


namespace MyApplication.Services.Graph;
public class GraphService : IGraphService
{
    private readonly IGraphAuthService graphAuthService;
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    public GraphService(IGraphAuthService graphAuthService, HttpClient httpClient, IConfiguration configuration)
    {
        this.graphAuthService = graphAuthService;
        this.httpClient = httpClient;
        this.configuration = configuration;
    }

    public async Task<List<AdB2CUserModel>> GetADB2CUsersAsync()
    {
        var accessToken = await graphAuthService.GetAccessTokenAsync();
        var tenantId = configuration["AzureAdB2CGraph:TenantId"];
        var requestUrl = $"https://graph.microsoft.com/v1.0/{tenantId}/users";

        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var graphResponse = JsonSerializer.Deserialize<GraphUserResponse>(jsonResponse);

        return graphResponse.Value.Select(u => new AdB2CUserModel
        {
            Id = u.Id,
            DisplayName = u.DisplayName,
            UserPrincipalName = u.UserPrincipalName,
            Mail = u.Mail
        }).ToList();
    }
}