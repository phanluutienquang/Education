using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyEducation.MyDomain.Common;
using MyEducation.MyDomain.Entities.Users;

namespace MyEducation.MyDomain.Entities.Learning
{   
[Table("Exam")]
public partial class Exam : BaseEntity, IAggregateRoot
{

    public int CourseId { get; set; }

    public int UserId { get; set; }

    
    public string Status { get; set; } = null!;

    public DateTime StartedOn { get; set; }

    public DateTime? FinishedOn { get; set; }

        
    public string? Feedback { get; set; }

    
    public virtual Course Course { get; set; } = null!;

    
    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    
    public virtual UserProfile User { get; set; } = null!;
}
}