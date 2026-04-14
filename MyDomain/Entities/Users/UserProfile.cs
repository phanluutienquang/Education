using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyEducation.MyDomain.Common;
using MyEducation.MyDomain.Entities.Communications;
using MyEducation.MyDomain.Entities.Learning;

namespace MyEducation.MyDomain.Entities.Users
{   
public partial class UserProfile : BaseEntity, IAggregateRoot
{

    public int UserId { get; set; }    
    public string DisplayName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string AdObjId { get; set; } = null!;
    public string? ProfileImageUrl { get; set; }
    // public DateTime CreatedOn { get; set; }
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
    // public virtual ICollection<UserActivityLog> UserActivityLogs { get; set; } = new List<UserActivityLog>();
    // public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();  
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    
}
}