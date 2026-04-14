using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEducation.MyApplication.Interfaces.Common;
public interface IUserClaims
{
    string GetCurrentUserEmail();
    string GetCurrentUserId();
    List<string> GetUserRoles();
    int GetUserId();
}