using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyEducation.Domain.Entities;
public partial class Course
{
    [Key]
    public int CourseId { get; set; }
    [StringLength(100)]
    public string Title { get; set; } = null!;
    [StringLength(500)]
    public string Description { get; set; } = null!;
    
}
