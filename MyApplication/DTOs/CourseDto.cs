using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyEducation.MyApplication.DTOs
{
public class CourseDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }  
}
public class CreateCourseDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }
    public class UpdateCourseDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
    }

}
