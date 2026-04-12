using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Class;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using BgituGrades.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BgituGrades.Application.Services
{
    
    public class ClassService(IClassRepository classRepository, IGroupRepository groupRepository, ITransferService transferService,
        IStudentRepository studentRepository, IWorkRepository workRepository, IMapper mapper, IDistributedCache cache) : IClassService
    {
        private readonly IClassRepository _classRepository = classRepository;
        private readonly IGroupRepository _groupRepository = groupRepository;
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly IWorkRepository _workRepository = workRepository;
        private readonly ITransferService _transferService = transferService;
        private readonly IMapper _mapper = mapper;
        private readonly IDistributedCache _cache = cache;
        private const string CacheKeyPrefix = "class:schedule:";

        public async Task<ClassResponse> CreateClassAsync(CreateClassRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Class>(request);
            var createdEntity = await _classRepository.CreateClassAsync(entity, cancellationToken: cancellationToken);
            return _mapper.Map<ClassResponse>(createdEntity);
        }

        public async Task<List<ClassResponse>> CreateClassAsync(CreateClassBulkRequest request, CancellationToken cancellationToken)
        {
            var entities = _mapper.Map<List<Class>>(request.Classes);
            var createdEntities = await _classRepository.CreateClassAsync(entities, cancellationToken: cancellationToken);
            return _mapper.Map<List<ClassResponse>>(createdEntities);
        }

        public async Task<List<ClassDateResponse>> GetClassDatesAsync(GetClassDateRequest request, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheKeyPrefix}group:{request.GroupId}:discipline:{request.DisciplineId}";

            var cached = await GetFromCacheAsync<List<ClassDateResponse>>(cacheKey);
            if (cached != null)
                return cached;

            var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken: cancellationToken);
            if (group == null) return [];

            var classDates = await GenerateScheduleDatesAsync(request.GroupId, request.DisciplineId, cancellationToken);

            await SetCacheAsync(cacheKey, classDates.ToList(), TimeSpan.FromDays(7));

            return classDates;
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
                .ToDictionary(t => t.OriginalDate, t => t.NewDate);

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

                    var actualDate = transferMap.TryGetValue(lessonDate, out var newDate)
                        ? newDate
                        : lessonDate;
                    if (!seen.Add((_class.Id, actualDate)))
                        continue;

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

        public async Task<bool> DeleteClassAsync(int id, CancellationToken cancellationToken)
        {
            return await _classRepository.DeleteClassAsync(id, cancellationToken: cancellationToken);
        }

        public async Task<ClassResponse?> GetClassByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _classRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            return entity == null ? null : _mapper.Map<ClassResponse>(entity);
        }

        public async Task<List<FullGradePresenceResponse>> GetPresenceByScheduleAsync(GetClassDateRequest request, CancellationToken cancellationToken)
        {
            var scheduleDates = await GenerateScheduleDatesAsync(request.GroupId, request.DisciplineId, cancellationToken);
            var scheduleDatesDomain = _mapper.Map<List<ScheduleDate>>(scheduleDates);
            var students = await _studentRepository.GetPresenseGrade(scheduleDatesDomain, request.GroupId, request.DisciplineId, cancellationToken: cancellationToken);
            var grade = _mapper.Map<List<FullGradePresenceResponse>>(students);
            return grade;
        }

        public async Task<List<FullGradeMarkResponse>> GetMarksByWorksAsync(GetClassDateRequest request, CancellationToken cancellationToken)
        {
            var works = await _workRepository.GetByDisciplineAndGroupAsync(request.DisciplineId, request.GroupId, cancellationToken: cancellationToken);

            var students = await _studentRepository.GetMarksGrade(works, request.GroupId, request.DisciplineId, cancellationToken: cancellationToken);
            var grade = _mapper.Map<List<FullGradeMarkResponse>>(students);
            return grade;
        }

        public async Task<List<ClassDTO>> GetAllClassesDtoAsync(CancellationToken cancellationToken)
        {
            var entities = await _classRepository.GetAllClassesAsync(cancellationToken: cancellationToken);
            return _mapper.Map<List<ClassDTO>>(entities);
        }

        private async Task<T?> GetFromCacheAsync<T>(string key)
        {
            try
            {
                var value = await _cache.GetStringAsync(key);
                if (value == null)
                    return default;
                return JsonSerializer.Deserialize<T>(value);
            }
            catch
            {
                return default;
            }
        }

        private async Task SetCacheAsync<T>(string key, T value, TimeSpan expiration)
        {
            try
            {
                var serialized = JsonSerializer.Serialize(value);
                var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
                await _cache.SetStringAsync(key, serialized, options);
            }
            catch
            {

            }
        }
    }
}
