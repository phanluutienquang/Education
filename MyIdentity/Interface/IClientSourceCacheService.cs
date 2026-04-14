using System.Threading.Tasks;
using MyEducation.MyIdentity.Models.Auth;

public interface IClientSourceCacheService
    {
        Task<ClientSource?> GetClientAsync(string clientId);
        Task InvalidateCacheAsync(string clientId);
        Task<bool> IsClientValidAsync(string clientId);
    }