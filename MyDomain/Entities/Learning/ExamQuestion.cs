using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyEducation.MyDomain.Common;
using MyEducation.MyDomain.Entities.Users;

namespace MyEducation.MyDomain.Entities.Learning
{

[Table("ExamQuestion")]
public partial class ExamQuestion : BaseEntity
{

    public int ExamId { get; set; }

    public int QuestionId { get; set; }

    public int? SelectedChoiceId { get; set; }

    public bool? IsCorrect { get; set; }

    public bool? ReviewLater { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;
  
    public virtual Choice? SelectedChoice { get; set; }
}
}