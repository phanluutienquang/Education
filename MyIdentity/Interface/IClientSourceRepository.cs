using System.Collections.Generic;
using System.Threading.Tasks;
using MyEducation.MyIdentity.Models.Auth;

public interface IClientSourceRepository
    {
        Task<ClientSource?> GetByIdAsync(string clientId);
        Task<ClientSource?> GetByApiKeyAsync(string apiKey);
        Task<bool> ExistsAsync(string clientId);
        Task UpdateLastUsedAsync(string clientId);
        Task<IEnumerable<ClientSource>> GetAllAsync();
        Task AddAsync(ClientSource client);
        Task UpdateAsync(ClientSource client);
        Task DeleteAsync(string clientId);
    }