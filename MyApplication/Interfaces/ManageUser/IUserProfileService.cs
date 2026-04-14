using MyEducation.MyDomain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyEducation.MyApplication.DTOs;

namespace MyEducation.MyApplication.Interfaces.ManageUser
{


    public interface IUserProfileService
    {
        Task UpdateUserProfilePicture(int userId, string pictureUrl);
        
        Task<UserProfile?> GetUserInfoAsync(int userId);
    }
}

