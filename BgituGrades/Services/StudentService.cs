using AutoMapper;
using BgituGrades.DTO;
using BgituGrades.Entities;
using BgituGrades.Models.Student;
using BgituGrades.Repositories;

namespace BgituGrades.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentResponse>> GetAllStudentsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<StudentResponse>> GetStudentsByGroupAsync(GetStudentsByGroupRequest request, CancellationToken cancellationToken);
        Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request, CancellationToken cancellationToken);
        Task<StudentResponse?> GetStudentByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateStudentAsync(UpdateStudentRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<StudentDTO>> GetAllStudentsDtoAsync(CancellationToken cancellationToken);
        Task<IEnumerable<StudentDTO>> GetStudentsDtoByGroupAsync(int groupId, CancellationToken cancellationToken);
        Task<StudentDTO?> GetStudentDtoByIdAsync(int id, CancellationToken cancellationToken);
    }
    public class StudentService(IStudentRepository studentRepository, IPresenceRepository presenceRepository,
        IClassService classService, IDisciplineRepository disciplineRepository, IMapper mapper) : IStudentService
    {
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly IPresenceRepository _presenceRepository = presenceRepository;
        private readonly IDisciplineRepository _disciplineRepository = disciplineRepository;
        private readonly IClassService _classService = classService;
        private readonly IMapper _mapper = mapper;

        public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Student>(request);
            var createdEntity = await _studentRepository.CreateStudentAsync(entity, cancellationToken: cancellationToken);

            var disciplines = await _disciplineRepository.GetByGroupIdAsync(request.GroupId, cancellationToken: cancellationToken);

            var disciplinesDict = new Dictionary<int, IEnumerable<DateOnly>>();
            foreach (var d in disciplines)
            {
                var classes = await _classService.GenerateScheduleDatesAsync(request.GroupId, d.Id, cancellationToken);
                disciplinesDict[d.Id] = classes.Select(c => c.Date);
            }

            await _presenceRepository.AddNewStudentPresences(createdEntity.Id, disciplinesDict, cancellationToken: cancellationToken);
            return _mapper.Map<StudentResponse>(createdEntity);
        }

        public async Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken)
        {
            return await _studentRepository.DeleteStudentAsync(id, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<StudentResponse>> GetAllStudentsAsync(CancellationToken cancellationToken)
        {
            var entities = await _studentRepository.GetAllStudentsAsync(cancellationToken: cancellationToken);
            return _mapper.Map<IEnumerable<StudentResponse>>(entities);
        }

        public async Task<StudentResponse?> GetStudentByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _studentRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            return entity == null ? null : _mapper.Map<StudentResponse>(entity);
        }

        public async Task<IEnumerable<StudentResponse>> GetStudentsByGroupAsync(GetStudentsByGroupRequest request, CancellationToken cancellationToken)
        {
            var entities = await _studentRepository.GetStudentsByGroupAsync(request.GroupId, cancellationToken: cancellationToken);
            return _mapper.Map<IEnumerable<StudentResponse>>(entities);
        }

        public async Task<bool> UpdateStudentAsync(UpdateStudentRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Student>(request);
            return await _studentRepository.UpdateStudentAsync(entity, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<StudentDTO>> GetAllStudentsDtoAsync(CancellationToken cancellationToken)
        {
            var entities = await _studentRepository.GetAllStudentsAsync(cancellationToken: cancellationToken);
            return _mapper.Map<IEnumerable<StudentDTO>>(entities);
        }

        public async Task<IEnumerable<StudentDTO>> GetStudentsDtoByGroupAsync(int groupId, CancellationToken cancellationToken)
        {
            var entities = await _studentRepository.GetStudentsByGroupAsync(groupId, cancellationToken: cancellationToken);
            return _mapper.Map<IEnumerable<StudentDTO>>(entities);
        }

        public async Task<StudentDTO?> GetStudentDtoByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _studentRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            return entity == null ? null : _mapper.Map<StudentDTO>(entity);
        }
    }

}
