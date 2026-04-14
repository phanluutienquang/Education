using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEducation.MyApplication.Models
{
    public class AdB2CUserModel
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string UserPrincipalName { get; set; } = string.Empty;
        public string? Mail { get; set; }
    }

    public class GraphUserResponse
    {
        public List<GraphUser> Value { get; set; } = new();
    }

    public class GraphUser
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string UserPrincipalName { get; set; } = string.Empty;
        public string? Mail { get; set; }
    }
}
