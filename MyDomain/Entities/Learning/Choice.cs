using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyEducation.MyDomain.Common;

namespace MyEducation.MyDomain.Entities.Learning
{   

[Table("Choice")]
public partial class Choice : BaseEntity
{

    public int QuestionId { get; set; }

    public string ChoiceText { get; set; } = null!;

    public bool IsCode { get; set; }

    public bool IsCorrect { get; set; }

  
    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    
    public virtual Question Question { get; set; } = null!;
}
}