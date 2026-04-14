using AutoMapper;
using MyEducation.MyDomain.Entities.Learning;
using MyEducation.MyApplication.DTOs;

namespace MyEducation.MyApplication.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Course, CourseDto>().ReverseMap();
            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}