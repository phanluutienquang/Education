using System.Collections.Generic;
using System.Threading.Tasks;
using MyEducation.MyIdentity.Models.Auth;

namespace MyEducation.MyIdentity.Interface;

public interface ITokenPairRepository
{
    Task<RefreshToken?> GetByIdAsync(int id);
    Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken);
    Task AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task DeleteAsync(int id);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByClientIdAsync(int clientSourceId);
}
