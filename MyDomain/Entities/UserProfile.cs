using System.ComponentModel.DataAnnotations;
using System;
namespace MyEducation.Domain.Entities;
public partial class UserProfile
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    public string DisplayName { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; }
    [StringLength(50)]
    public string LastName { get; set; }
    [StringLength(100)]
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}