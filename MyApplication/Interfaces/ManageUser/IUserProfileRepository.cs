using System.Threading.Tasks;
using MyEducation.MyDomain.Entities.Users ;

namespace MyEducation.MyApplication.Interfaces.ManageUser
{
public interface IUserProfileRepository
{
    Task<UserProfile?> GetUserProfileById(int userId);
    Task UpdateUserProfilePicture(int userId, string pictureUrl);
        
    Task<UserProfile?> GetUserInfoAsync(int userId);
    
    Task UpdateUserProfile(UserProfile userProfile);
}
}