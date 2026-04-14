
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyEducation.MyApplication.Models;

namespace MyEducation.MyApplication.Interfaces.Graph
{
    public interface IGraphService
    {
        public Task<List<AdB2CUserModel>> GetADB2CUsersAsync();
    }
}
