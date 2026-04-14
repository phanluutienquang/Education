using Microsoft.EntityFrameworkCore;
using MyEducation.MyDomain.Entities.Users;
using MyEducation.MyApplication.Interfaces.ManageUser;
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

    public async Task UpdateUserProfilePicture(int userId, string pictureUrl)
    {
        var user = await _context.UserProfiles.FindAsync(userId);
        if (user != null)
        {
            // Update profile picture logic here
            await _context.SaveChangesAsync();
        }
    }

    public async Task<UserProfile?> GetUserInfoAsync(int userId)
    {
        return await _context.UserProfiles.FindAsync(userId);
    }
}
