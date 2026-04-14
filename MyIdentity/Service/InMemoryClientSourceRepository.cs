using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyEducation.MyIdentity.Interface;
using MyEducation.MyIdentity.Models.Auth;

public class InMemoryClientSourceRepository : IClientSourceRepository
{
    private readonly List<ClientSource> _clients = new List<ClientSource>();

    public Task AddAsync(ClientSource client)
    {
        _clients.Add(client);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string clientId)
    {
        var client = _clients.FirstOrDefault(c => c.ClientId == clientId);
        if (client != null)
        {
            _clients.Remove(client);
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<ClientSource>> GetAllAsync()
    {
        return Task.FromResult(_clients.AsEnumerable());
    }

    public Task<ClientSource?> GetByApiKeyAsync(string apiKey)
    {
        var client = _clients.FirstOrDefault(c => c.ApiSecret == apiKey);
        return Task.FromResult(client);
    }

    public Task<ClientSource?> GetByIdAsync(string clientId)
    {
        var client = _clients.FirstOrDefault(c => c.ClientId == clientId);
        return Task.FromResult(client);
    }

    public Task UpdateAsync(ClientSource client)
    {
        var existingClient = _clients.FirstOrDefault(c => c.ClientId == client.ClientId);
        if (existingClient != null)
        {
            existingClient.Update(
                clientName: client.ClientName,
                isEnabled: client.IsEnabled,
                allowedScopes: client.AllowedScopes,
                allowedIPs: client.AllowedIPs,
                rateLimitPerMinute: client.RateLimitPerMinute
            );
        }
        return Task.CompletedTask;
    }

    public Task UpdateLastUsedAsync(string clientId)
    {
        var client = _clients.FirstOrDefault(c => c.ClientId == clientId);
        if (client != null)
        {
            client.UpdateLastUsed();
        }
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string clientId)
    {
        var exists = _clients.Any(c => c.ClientId == clientId);
        return Task.FromResult(exists);
    }
}