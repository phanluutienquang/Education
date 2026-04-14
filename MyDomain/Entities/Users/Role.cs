using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyEducation.MyDomain.Entities.Users
{
    [Table("Role")]
    public partial class Role
    {

        public int RoleId { get; set; }

  
    public string Name { get; set; } = null!;

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
}