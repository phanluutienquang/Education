using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyEducation.MyDomain.Common;
using MyEducation.MyDomain.Entities.Users;


namespace MyEducation.MyDomain.Entities.Learning
{   

[Table("Course")]
public partial class Course : BaseEntity, IAggregateRoot
{

    [StringLength(100)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }
    public virtual UserProfile CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
}