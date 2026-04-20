using AutoMapper;
using BgituGrades.Application.Caching;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Class;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using BgituGrades.Domain.Models;
using Microsoft.Extensions.Caching.Hybrid;

namespace BgituGrades.Application.Services
{
    
    public class ClassService(IClassRepository classRepository, IGroupRepository groupRepository, ITransferService transferService,
        IStudentRepository studentRepository, IWorkRepository workRepository, IMapper mapper, ICacheService cacheService) : IClassService
    {
        private readonly IClassRepository _classRepository = classRepository;
        private readonly IGroupRepository _groupRepository = groupRepository;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly IWorkRepository _workRepository = workRepository;
        private readonly ITransferService _transferService = transferService;
        private readonly IMapper _mapper = mapper;
        private readonly ICacheService _cacheService = cacheService;

        private static readonly HybridCacheEntryOptions DefaultOptions = new()
        {
            Expiration = TimeSpan.FromHours(24),
            LocalCacheExpiration = TimeSpan.FromHours(2)
        };

        public async Task<ClassDTO> CreateClassAsync(ClassDTO classDto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Class>(classDto);
            var createdEntity = await _classRepository.CreateClassAsync(entity, cancellationToken: cancellationToken);
            await _cacheService.RemoveByTagAsync(CacheTags.Class(), ct: cancellationToken);
            return _mapper.Map<ClassDTO>(createdEntity);
        }

        public async Task<List<ClassDTO>> CreateClassAsync(IEnumerable<ClassDTO> classDto, CancellationToken cancellationToken)
        {
            var entities = _mapper.Map<List<Class>>(classDto);
            var createdEntities = await _classRepository.CreateClassAsync(entities, cancellationToken: cancellationToken);
            await _cacheService.RemoveByTagAsync(CacheTags.Class(), ct: cancellationToken);
            return _mapper.Map<List<ClassDTO>>(createdEntities);
        }

        public async Task<List<ClassDateResponse>> GetClassDatesAsync(int groupId, int disciplineId, CancellationToken cancellationToken)
        {
            return await _cacheService.GetOrCreateAsync(
                key: CacheKeys.ClassByGroupAndDiscipline(groupId, disciplineId),
                factory: async token => await GenerateScheduleDatesAsync(groupId, disciplineId, token),
                tags: CacheTags.ClassAll(),
                options: DefaultOptions, ct: cancellationToken);
        }


        public async Task<List<ClassDateResponse>> GenerateScheduleDatesAsync(int groupId, int disciplineId, CancellationToken cancellationToken,
            DateOnly? startDateOverride = null, DateOnly? endDateOverride = null)
        {
            var group = await _groupRepository.GetByIdAsync(groupId, cancellationToken: cancellationToken);
            if (group == null) return [];

            var classes = await _classRepository.GetClassesByDisciplineAndGroupAsync(disciplineId, groupId, cancellationToken: cancellationToken);
            var transfers = await _transferService.GetTransfersByGroupAndDisciplineAsync(groupId, disciplineId, cancellationToken: cancellationToken);

            var startDate = startDateOverride ?? group.StudyStartDate;
            var endDate = endDateOverride ?? group.StudyEndDate;
            var firstWeekStart = group.StartWeekNumber;

            var dates = new List<ClassDateResponse>();


            var studyStartDayOfWeek = startDate.DayOfWeek;
            var daysToMonday = ((int)DayOfWeek.Monday - (int)studyStartDayOfWeek + 7) % 7;
            var firstMonday = startDate.AddDays(daysToMonday);


            var week1Start = firstMonday.AddDays(-7 * (firstWeekStart - 1));

            var transferMap = transfers
                .ToDictionary(t => (t.OriginalDate, t.ClassId), t => t.NewDate);

            var seen = new HashSet<(int ClassId, DateOnly ActualDate)>();


            if (week1Start > endDate.AddDays(7))
                return dates;

            var currentWeekStart = week1Start;

            while (currentWeekStart <= endDate.AddDays(7))
            {
                foreach (var _class in classes)
                {
                    var lessonDate = currentWeekStart
                        .AddDays(_class.WeekDay - 1)
                        .AddDays(7 * (_class.Weeknumber - 1));

                    var actualDate = transferMap.TryGetValue((lessonDate, _class.Id), out var newDate)
                        ? newDate
                        : lessonDate;
                    if (!seen.Add((_class.Id, actualDate)))
                        continue;

                    if (lessonDate >= startDate && lessonDate <= endDate)
                    {
                        dates.Add(new ClassDateResponse
                        {
                            OriginalDate = lessonDate,
                            Date = actualDate,
                            ClassType = _class.Type,
                            StartTime = _class.StartTime,
                            Id = _class.Id
                        });
                    }
                }
                currentWeekStart = currentWeekStart.AddDays(14);
            }

            return dates.OrderBy(d => d.Date).ToList();
        }

        public async Task<List<ClassDateResponse>> GenerateScheduleDatesAsync(Group group, IEnumerable<Class> classes,
            IEnumerable<Transfer> transfers, DateOnly? startDateOverride = null, DateOnly? endDateOverride = null)
        {
            var startDate = startDateOverride ?? group.StudyStartDate;
            var endDate = endDateOverride ?? group.StudyEndDate;
            var firstWeekStart = group.StartWeekNumber;

            var dates = new List<ClassDateResponse>();


            var studyStartDayOfWeek = startDate.DayOfWeek;
            var daysToMonday = ((int)DayOfWeek.Monday - (int)studyStartDayOfWeek + 7) % 7;
            var firstMonday = startDate.AddDays(daysToMonday);


            var week1Start = firstMonday.AddDays(-7 * (firstWeekStart - 1));

            var transferMap = transfers
                .ToDictionary(t => t.OriginalDate, t => t.NewDate);


            if (week1Start > endDate.AddDays(7))
                return dates;

            var currentWeekStart = week1Start;

            while (currentWeekStart <= endDate.AddDays(7))
            {
                foreach (var _class in classes)
                {
                    var lessonDate = currentWeekStart
                        .AddDays(_class.WeekDay - 1)
                        .AddDays(7 * (_class.Weeknumber - 1));

                    var actualDate = transferMap.TryGetValue(lessonDate, out var newDate)
                        ? newDate
                        : lessonDate;

                    if (lessonDate >= startDate && lessonDate <= endDate)
                    {
                        dates.Add(new ClassDateResponse
                        {
                            Date = actualDate,
                            ClassType = _class.Type,
                            StartTime = _class.StartTime,
                            Id = _class.Id
                        });
                    }
                }
                currentWeekStart = currentWeekStart.AddDays(14);
            }

            return dates.OrderBy(d => d.Date).ToList();
        }

        public async Task<List<FullGradePresenceResponse>> GetPresenceByScheduleAsync(int groupId, int disciplineId, CancellationToken cancellationToken)
        {
            var scheduleDates = await GetClassDatesAsync(groupId, disciplineId, cancellationToken);
            var scheduleDatesDomain = _mapper.Map<List<ScheduleDate>>(scheduleDates);
            var students = await _studentRepository.GetPresenseGrade(scheduleDatesDomain, groupId, disciplineId, cancellationToken: cancellationToken);
            var grade = _mapper.Map<List<FullGradePresenceResponse>>(students);
            return grade;
        }

        public async Task<List<FullGradeMarkResponse>> GetMarksByWorksAsync(int groupId, int disciplineId, CancellationToken cancellationToken)
        {
            var works = await _cacheService.GetOrCreateAsync(
                    key: CacheKeys.WorkByGroupAndDiscipline(groupId, disciplineId),
                    factory: async token => await _workRepository.GetByDisciplineAndGroupAsync(disciplineId, groupId, cancellationToken: token),
                    tags: CacheTags.WorkAll(),
                    options: DefaultOptions, ct: cancellationToken);
                

            var students = await _studentRepository.GetMarksGrade(works, groupId, disciplineId, cancellationToken: cancellationToken);
            var grade = _mapper.Map<List<FullGradeMarkResponse>>(students);
            return grade;
        }
    }
}
