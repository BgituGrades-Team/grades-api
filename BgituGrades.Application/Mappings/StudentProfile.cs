using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Student;
using BgituGrades.Domain.Entities;

namespace BgituGrades.Application.Mappings
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<CreateStudentRequest, Student>();
            CreateMap<UpdateStudentRequest, Student>();
            CreateMap<Student, StudentResponse>();
            CreateMap<Student, StudentDTO>();
            CreateMap<StudentDTO, Student>();
        }
    }
}
