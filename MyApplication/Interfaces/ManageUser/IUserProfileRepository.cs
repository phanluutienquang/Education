using System.Threading.Tasks;
using MyEducation.Domain.Entities;

namespace MyEducation.Application.Interfaces.ManageUser;
public interface IUserProfileRepository
{
    Task<UserProfile?> GetUserProfileById(int userId);
    Task UpdateUserProfile(UserProfile userProfile);
}