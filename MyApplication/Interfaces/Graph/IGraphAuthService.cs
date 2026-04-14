using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEducation.MyApplication.Interfaces.Graph
{
    public interface IGraphAuthService
    {
        Task<string> GetAccessTokenAsync();
    }
}
