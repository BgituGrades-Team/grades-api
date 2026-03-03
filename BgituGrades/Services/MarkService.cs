using AutoMapper;
using BgituGrades.DTO;
using BgituGrades.Entities;
using BgituGrades.Models.Class;
using BgituGrades.Models.Mark;
using BgituGrades.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BgituGrades.Services
{
    public interface IMarkService
    {
        Task<IEnumerable<MarkResponse>> GetAllMarksAsync();
        Task<MarkResponse> CreateMarkAsync(CreateMarkRequest request);
        Task<IEnumerable<MarkResponse>> GetMarksByDisciplineAndGroupAsync(GetMarksByDisciplineAndGroupRequest request);
        Task<bool> UpdateMarkAsync(UpdateMarkRequest request);
        Task<bool> DeleteMarkByStudentAndWorkAsync(DeleteMarkByStudentAndWorkRequest request);
        Task<FullGradeMarkResponse> UpdateOrCreateMarkAsync(UpdateMarkGradeRequest request);
        Task<IEnumerable<MarkDTO>> GetAllMarksDtoAsync();
        Task<MarkDTO?> GetMarkDtoByIdAsync(int id);
    }
    public class MarkService(IMarkRepository markRepository, IMapper mapper) : IMarkService
    {
        private readonly IMarkRepository _markRepository = markRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<MarkResponse> CreateMarkAsync(CreateMarkRequest request)
        {
            var entity = _mapper.Map<Mark>(request);
            var createdEntity = await _markRepository.CreateMarkAsync(entity);
            return _mapper.Map<MarkResponse>(createdEntity);
        }

        public async Task<IEnumerable<MarkResponse>> GetAllMarksAsync()
        {
            var entities = await _markRepository.GetAllMarksAsync();
            return _mapper.Map<IEnumerable<MarkResponse>>(entities);
        }

        public async Task<IEnumerable<MarkResponse>> GetMarksByDisciplineAndGroupAsync(GetMarksByDisciplineAndGroupRequest request)
        {
            var entities = await _markRepository.GetMarksByDisciplineAndGroupAsync(request.DisciplineId, request.GroupId);
            return _mapper.Map<IEnumerable<MarkResponse>>(entities);
        }

        public async Task<bool> UpdateMarkAsync(UpdateMarkRequest request)
        {
            var entity = _mapper.Map<Mark>(request);
            return await _markRepository.UpdateMarkAsync(entity);
        }

        public async Task<bool> DeleteMarkByStudentAndWorkAsync(DeleteMarkByStudentAndWorkRequest request)
        {
            return await _markRepository.DeleteMarkByStudentAndWorkAsync(request.StudentId, request.WorkId);
        }

        public async Task<FullGradeMarkResponse> UpdateOrCreateMarkAsync(UpdateMarkGradeRequest request)
        {
            var mark = await _markRepository.GetMarkByStudentAndWorkAsync(request.StudentId, request.WorkId);

            if (mark != null)
            {
                mark.Value = request.Value;
                await _markRepository.UpdateMarkAsync(mark);
            }
            else
            {
                mark = _mapper.Map<Mark>(request);
                await _markRepository.CreateMarkAsync(mark);
            }

            var response = new FullGradeMarkResponse
            {
                StudentId = request.StudentId,
                Marks = [new GradeMarkResponse
                {
                    WorkId = request.WorkId,
                    Value = request.Value,
                }]
            };
            return response;
        }

        public async Task<IEnumerable<MarkDTO>> GetAllMarksDtoAsync()
        {
            var entities = await _markRepository.GetAllMarksAsync();
            return _mapper.Map<IEnumerable<MarkDTO>>(entities);
        }

        public async Task<MarkDTO?> GetMarkDtoByIdAsync(int id)
        {
            var entity = await _markRepository.GetMarkByIdAsync(id);
            return entity == null ? null : _mapper.Map<MarkDTO>(entity);
        }
    }
}
