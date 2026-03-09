using Microsoft.EntityFrameworkCore;
using MyEducation.Domain.Entities;
using MyEducation.Application.Interfaces.ManageUser;
using System.Threading.Tasks;

namespace MyEducation.Infra;
public class UserProfileRepository : IUserProfileRepository
{
    private readonly SmartCertifyContext _context;

    public UserProfileRepository(SmartCertifyContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetUserProfileById(int userId)
    {
        var user =  await _context.UserProfiles.FindAsync(userId);
        return user;
    }

    public async Task UpdateUserProfile(UserProfile userProfile)
    {
        _context.UserProfiles.Update(userProfile);
        await _context.SaveChangesAsync();
    }
}
