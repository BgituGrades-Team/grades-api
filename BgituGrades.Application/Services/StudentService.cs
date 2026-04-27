using AutoMapper;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Student;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;
using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace BgituGrades.Application.Services
{

    public partial class StudentService(IStudentRepository studentRepository,
        IGroupService groupService, IMapper mapper) : IStudentService

    {
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly IGroupService _groupService = groupService;
        private readonly IMapper _mapper = mapper;

        private const sbyte STATUS_STUDYING = 1;
        private const short BATCH_SIZE = 2000;
        private const byte COL_CODE = 0;
        private const byte COL_LASTNAME = 1;
        private const byte COL_FIRSTNAME = 2;
        private const byte COL_MIDDLENAME = 3;
        private const byte COL_STATUS = 4;
        private const byte COL_GROUP_CODE = 6;
        private const byte COL_GROUP_NAME = 7;


        public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Student>(request);
            var createdEntity = await _studentRepository.CreateStudentAsync(entity, cancellationToken: cancellationToken);
            return _mapper.Map<StudentResponse>(createdEntity);
        }

        public async Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken)
        {
            return await _studentRepository.DeleteStudentAsync(id, cancellationToken: cancellationToken);
        }

        public async Task<StudentResponse?> GetStudentByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _studentRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            return entity == null ? null : _mapper.Map<StudentResponse>(entity);
        }

        public async Task<List<StudentResponse>> GetStudentsByGroupAsync(GetStudentsByGroupRequest request, CancellationToken cancellationToken)
        {
            var entities = await _studentRepository.GetStudentsByGroupIdsAsync(request.GroupIds.Values, cancellationToken: cancellationToken);
            return _mapper.Map<List<StudentResponse>>(entities);
        }

        public async Task<bool> UpdateStudentAsync(UpdateStudentRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Student>(request);
            return await _studentRepository.UpdateStudentAsync(entity, cancellationToken: cancellationToken);
        }

        public async Task<ImportResult> ImportStudentsFromXlsxAsync(Stream fileStream, CancellationToken cancellationToken)
        {
            var groups = await _groupService.GetAllAsync(cancellationToken);
    
            var groupsByName = groups
                .Where(g => g.Name != null)
                .ToDictionary(
                    g => g.Name!,
                    g => g.Id,
                    StringComparer.OrdinalIgnoreCase);

            var subGroupMap = groupsByName
                .Keys
                .Where(name => name.Contains('(') && name.Contains(')'))
                .GroupBy(name =>
                {
                    var match = Regex.Match(name, @"\([а-яёa-z]\)$",
                        RegexOptions.IgnoreCase);
                    return match.Success
                        ? name[..match.Index].Trim()
                        : name[..name.IndexOf('(')].Trim();
                }, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(name => groupsByName[name]).ToList(),
                    StringComparer.OrdinalIgnoreCase);


            var result = new ImportResult();
            var batch = new List<Student>(BATCH_SIZE);
            var unknownGroups = new HashSet<string>();
            var leavedStudents = new List<int>();

            using var package = new ExcelPackage(fileStream);
            var sheet = package.Workbook.Worksheets[0];

            int totalRows = sheet.Dimension.End.Row;

            for (int row = 2; row <= totalRows; row++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var statusCell = sheet.Cells[row, COL_STATUS + 1].Value;
                if (statusCell == null) continue;

                var officialId = sheet.Cells[row, COL_CODE + 1].GetValue<int>();

                sbyte status = Convert.ToSByte(statusCell);
                if (status != STATUS_STUDYING)
                {
                    leavedStudents.Add(officialId);
                    result.SkippedRows++;
                    continue;
                }


                var groupName = sheet.Cells[row, COL_GROUP_NAME + 1].GetValue<string>()?.Trim();

                if (string.IsNullOrEmpty(groupName))
                {
                    result.SkippedRows++;
                    continue;
                }

                List<int> targetGroupIds;
                if (groupsByName.TryGetValue(groupName, out int exactGroupId))
                {
                    targetGroupIds = [exactGroupId];
                }
                else if (subGroupMap.TryGetValue(groupName, out var subIds))
                {
                    targetGroupIds = subIds;
                }
                else
                {
                    unknownGroups.Add(groupName);
                    result.SkippedRows++;
                    continue;
                }

                var lastName = sheet.Cells[row, COL_LASTNAME + 1].GetValue<string>()?.Trim() ?? "";
                var firstName = sheet.Cells[row, COL_FIRSTNAME + 1].GetValue<string>()?.Trim() ?? "";
                var middleName = sheet.Cells[row, COL_MIDDLENAME + 1].GetValue<string>()?.Trim() ?? "";

                var officialGroupId = sheet.Cells[row, COL_GROUP_CODE + 1].GetValue<int>();

                var fullName = string.Join(" ",
                    new[] { lastName, firstName, middleName }
                        .Where(s => !string.IsNullOrEmpty(s) && s != "NULL"));

                foreach (var gId in targetGroupIds)
                {
                    batch.Add(new Student
                    {
                        OfficialId = officialId,
                        Name = fullName,
                        GroupId = gId,
                        OfficialGroupId = officialGroupId,
                    });
                }
                result.ProcessedRows++;

                if (batch.Count >= BATCH_SIZE)
                {
                    await FlushBatchAsync(batch, leavedStudents, cancellationToken);
                    batch.Clear();
                    leavedStudents.Clear();
                }
            }

            if (batch.Count > 0)
                await FlushBatchAsync(batch, leavedStudents, cancellationToken);

            result.UnknownGroups = unknownGroups;
            return result;
        }

        private async Task FlushBatchAsync(IEnumerable<Student> batch, IEnumerable<int> leavedOfficialIds, CancellationToken cancellationToken)
        {

            await _studentRepository.DeleteByIdsAsync(leavedOfficialIds, cancellationToken: cancellationToken);

            await _studentRepository.BulkInsertAsync(batch, cancellationToken: cancellationToken);
        }

        public async Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            await _studentRepository.DeleteAllAsync(cancellationToken);
        }

        public async Task<List<StudentResponse>> GetArchivedStudentsByGroupAsync(GetStudentsByGroupRequest request, CancellationToken cancellationToken)
        {
            var students = await _studentRepository.GetArchivedByGroupIdsAsync(request.GroupIds.Values, cancellationToken);
            var results = _mapper.Map<List<StudentResponse>>(students);
            return results;
        }
    }
}
