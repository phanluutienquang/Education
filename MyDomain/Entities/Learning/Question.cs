using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyEducation.MyDomain.Common;


namespace MyEducation.MyDomain.Entities.Learning
{
[Table("Question")]
public partial class Question : BaseEntity, IAggregateRoot
{

    public int CourseId { get; set; }

    public string QuestionText { get; set; } = null!;

    [StringLength(20)]
    public string DifficultyLevel { get; set; } = null!;

    public bool IsCode { get; set; }

    public bool HasMultipleAnswers { get; set; }

    public virtual ICollection<Choice> Choices { get; set; } = new List<Choice>();

    public virtual Course Course { get; set; } = null!;
    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

}
}