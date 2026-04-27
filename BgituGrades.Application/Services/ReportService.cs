using BgituGrades.Application.Caching;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Features;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Report;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Enums;
using BgituGrades.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Globalization;
using System.Text.Json;

namespace BgituGrades.Application.Services
{
    public class ReportService(
        IReportProgressNotifier notifier,
        IDistributedCache cache,
        IServiceScopeFactory scopeFactory) : IReportService
    {
        private static readonly Color ZebraColor = Color.FromArgb(245, 245, 245);
        private static readonly Color HeaderBlue = Color.FromArgb(96, 165, 250);
        private static readonly Color SubheadGray = Color.LightGray;

        private static readonly DistributedCacheEntryOptions CacheOptions =
            new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));

        public async Task<Guid> GenerateReportAsync(ReportRequest request, string connectionId, CancellationToken cancellationToken)
        {
            var reportId = Guid.NewGuid();
            await notifier.AddToGroupAsync(connectionId, reportId);

            var requestHash = RequestHasher.ComputeRequestCacheKey(request);
            var metaKey = CacheKeys.ReportByRequestHash(requestHash);

            var cachedMeta = await cache.GetAsync(metaKey, cancellationToken);
            if (cachedMeta is not null)
            {
                var cachedBytes = await cache.GetAsync($"report_bytes_{requestHash}", cancellationToken);
                if (cachedBytes is not null)
                {
                    var meta = JsonSerializer.Deserialize<CachedReport>(cachedMeta)!;
                    await cache.SetAsync($"report_{reportId}", cachedBytes, CacheOptions, cancellationToken);
                    await notifier.SendReadyAsync(reportId, $"https://{request.Host}/api/report/{reportId}/download", meta.Preview!);
                    return reportId;
                }
            }

            _ = Task.Run(() => GenerateWithProgress(reportId, request, requestHash, cancellationToken), cancellationToken);
            return reportId;
        }

        protected virtual async Task GenerateWithProgress(Guid reportId, ReportRequest request, string requestHash, CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            try
            {
                await notifier.SendProgressAsync(reportId, 10, "Загрузка данных...");

                var groupRepo = sp.GetRequiredService<IGroupRepository>();
                var disciplineRepo = sp.GetRequiredService<IDisciplineRepository>();
                var studentRepo = sp.GetRequiredService<IStudentRepository>();

                var groups = request.GroupIds != null
                    ? await groupRepo.GetGroupsByIdsAsync(request.GroupIds, cancellationToken: cancellationToken)
                    : await groupRepo.GetAllAsync(cancellationToken: cancellationToken);

                var groupIds = groups.Select(g => g.Id).ToList();

                var disciplinesTask = request.DisciplineIds != null
                    ? disciplineRepo.GetDisciplinesByIdsAsync(request.DisciplineIds, cancellationToken: cancellationToken)
                    : disciplineRepo.GetByGroupIdsAsync(groupIds, cancellationToken: cancellationToken);

                var studentsTask = request.StudentIds != null
                    ? studentRepo.GetStudentsByIdsAsync(request.StudentIds, cancellationToken: cancellationToken)
                    : studentRepo.GetStudentsByGroupIdsAsync(groupIds, cancellationToken: cancellationToken);

                await Task.WhenAll(disciplinesTask, studentsTask);
                var disciplines = await disciplinesTask;
                var students = await studentsTask;

                if (!groups.Any() || !disciplines.Any())
                    throw new Exception("Нет данных для формирования отчета");

                await notifier.SendProgressAsync(reportId, 40, "Генерация Excel файла...");

                var markRepo = sp.GetRequiredService<IMarkRepository>();
                var presenceRepo = sp.GetRequiredService<IPresenceRepository>();
                var classService = sp.GetRequiredService<IClassService>();

                TablePreview result = request.ReportType == ReportType.MARK
                    ? await GenerateMarksExcelAsync(markRepo, groups, disciplines, students, cancellationToken)
                    : await GeneratePresenceExcelAsync(presenceRepo, groups, disciplines, students, classService, cancellationToken);

                await notifier.SendProgressAsync(reportId, 80, "Сохранение...");

                var metaBytes = JsonSerializer.SerializeToUtf8Bytes(new CachedReport { Preview = result.Preview });
                await Task.WhenAll(
                    cache.SetAsync($"report_{reportId}", result.ExcelBytes!, CacheOptions),
                    cache.SetAsync($"report_bytes_{requestHash}", result.ExcelBytes!, CacheOptions),
                    cache.SetAsync($"report_meta_{requestHash}", metaBytes, CacheOptions)
                );

                await notifier.SendReadyAsync(reportId, $"https://{request.Host}/api/report/{reportId}/download", result.Preview!);
            }
            catch (Exception ex)
            {
                await notifier.SendErrorAsync(reportId, ex.Message, ex.StackTrace);
            }
        }

        private static (ExcelWorksheet ws, List<Group> sortedGroups, Dictionary<int, List<Discipline?>> disciplinesByGroup, ILookup<int, Student> studentsLookup, int maxCols, ReportPreviewDto preview)
            BuildSheetSkeleton(ExcelPackage package, string name, IEnumerable<Group> groups, IEnumerable<Discipline> disciplines, IEnumerable<Student> students)
        {
            var ws = package.Workbook.Worksheets.Add(name);
            var sortedGroups = groups.OrderBy(g => g.Name).ToList();
            var allowedIds = disciplines.Select(d => d.Id).ToHashSet();
            var studentsLookup = students.ToLookup(s => s.GroupId);

            var disciplinesByGroup = sortedGroups.ToDictionary(
                g => g.Id,
                g => g.Classes?.Where(c => c.Discipline != null && allowedIds.Contains(c.Discipline.Id))
                               .Select(c => c.Discipline).DistinctBy(d => d!.Id).OrderBy(d => d!.Name).ToList() ?? []
            );

            int maxCols = disciplinesByGroup.Count != 0 ? disciplinesByGroup.Values.Max(v => v.Count) : 1;


            for (int col = 2; col <= maxCols + 1; col++)
            {
                ws.Column(col).Width = 30;
                ws.Column(col).Style.WrapText = true;
            }

            return (ws, sortedGroups, disciplinesByGroup, studentsLookup, maxCols, new ReportPreviewDto());
        }

        private static int WriteGroupHeader(ExcelWorksheet ws, Group group, List<Discipline?> groupDisciplines, int maxCols, int row, ReportPreviewDto preview)
        {
            var range = ws.Cells[row, 1, row, maxCols + 1];
            ws.Cells[row, 1].Value = group.Name;
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(HeaderBlue);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            for (int i = 0; i < groupDisciplines.Count; i++)
            {
                var cell = ws.Cells[row, i + 2];
                cell.Value = groupDisciplines[i]!.Name;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(SubheadGray);
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.WrapText = true;
            }
            ws.Row(row).CustomHeight = false;

            preview.Rows.Add(new PreviewRow { IsGroupHeader = true, Cells = [group.Name!, .. groupDisciplines.Select(d => d!.Name)] });
            return row + 1;
        }

        protected static async Task<TablePreview> GenerateMarksExcelAsync(IMarkRepository markRepo, IEnumerable<Group> groups, IEnumerable<Discipline> disciplines, IEnumerable<Student> students, CancellationToken ct)
        {
            using var package = new ExcelPackage();
            var (ws, sortedGroups, disciplinesByGroup, studentsLookup, maxCols, preview) = BuildSheetSkeleton(package, "Отчёт успеваемости", groups, disciplines, students);

            var allMarks = await markRepo.GetMarksByDisciplinesAndGroupsAsync(disciplines.Select(d => d.Id).ToList(), sortedGroups.Select(g => g.Id).ToList(), cancellationToken: ct);

            var markDict = allMarks
                .Where(m => m.Work != null && !string.IsNullOrEmpty(m.Value))
                .Select(m => new
                {
                    m.StudentId,
                    m.Work!.DisciplineId,
                    Val = double.TryParse(m.Value!.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : (double?)null
                })
                .Where(x => x.Val.HasValue)
                .GroupBy(x => (x.StudentId, x.DisciplineId))
                .ToDictionary(g => g.Key, g => g.Average(x => x.Val!.Value));

            int currentRow = 1;
            foreach (var group in sortedGroups)
            {
                var groupDisciplines = disciplinesByGroup[group.Id];
                currentRow = WriteGroupHeader(ws, group, groupDisciplines, maxCols, currentRow, preview);

                var groupStudents = studentsLookup[group.Id].OrderBy(s => s.Name).ToList();
                for (int sIdx = 0; sIdx < groupStudents.Count; sIdx++)
                {
                    var student = groupStudents[sIdx];
                    ws.Cells[currentRow, 1].Value = student.Name;

                    if (sIdx % 2 != 0)
                    {
                        var rowRange = ws.Cells[currentRow, 1, currentRow, groupDisciplines.Count + 1];
                        rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rowRange.Style.Fill.BackgroundColor.SetColor(ZebraColor);
                    }

                    var previewCells = new List<string> { student.Name! };
                    for (int i = 0; i < groupDisciplines.Count; i++)
                    {
                        var cell = ws.Cells[currentRow, i + 2];
                        if (markDict.TryGetValue((student.Id, groupDisciplines[i]!.Id), out var avg))
                        {
                            cell.Value = avg;
                            cell.Style.Numberformat.Format = "0.0";
                            previewCells.Add(avg.ToString("0.0", CultureInfo.InvariantCulture));
                        }
                        else { cell.Value = 0; previewCells.Add("0"); }
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    preview.Rows.Add(new PreviewRow { IsGroupHeader = false, Cells = previewCells });
                    currentRow++;
                }
            }

            ws.Column(1).AutoFit();
            ApplyFinalStyle(ws, currentRow - 1, maxCols + 1);

            return new TablePreview { ExcelBytes = await package.GetAsByteArrayAsync(ct), Preview = preview };
        }

        protected static async Task<TablePreview> GeneratePresenceExcelAsync(IPresenceRepository presenceRepo, IEnumerable<Group> groups, IEnumerable<Discipline> disciplines, IEnumerable<Student> students, IClassService classService, CancellationToken ct)
        {
            using var package = new ExcelPackage();
            var (ws, sortedGroups, disciplinesByGroup, studentsLookup, maxCols, preview) = BuildSheetSkeleton(package, "Отчёт посещаемости", groups, disciplines, students);

            var groupDisciplinePairs = sortedGroups.SelectMany(g => disciplinesByGroup[g.Id].Select(d => (g.Id, d!.Id))).Distinct().ToList();

            var scheduleTotalDictTask = classService.GetClassDateCountsAsync(groups, groupDisciplinePairs, ct);
            var allPresencesTask = presenceRepo.GetPresencesByDisciplinesAndGroupsAsync(disciplines.Select(d => d.Id).ToList(), sortedGroups.Select(g => g.Id).ToList(), cancellationToken: ct);

            await Task.WhenAll(scheduleTotalDictTask, allPresencesTask);
            var scheduleTotalDict = await scheduleTotalDictTask;
            var absentDict = (await allPresencesTask).GroupBy(m => (m.StudentId, m.DisciplineId)).ToDictionary(g => g.Key, g => g.Count(m => m.IsPresent != PresenceType.PRESENT));

            int currentRow = 1;
            foreach (var group in sortedGroups)
            {
                var groupDisciplines = disciplinesByGroup[group.Id];
                currentRow = WriteGroupHeader(ws, group, groupDisciplines, maxCols, currentRow, preview);

                var groupStudents = studentsLookup[group.Id].OrderBy(s => s.Name).ToList();
                for (int sIdx = 0; sIdx < groupStudents.Count; sIdx++)
                {
                    var student = groupStudents[sIdx];
                    ws.Cells[currentRow, 1].Value = student.Name;

                    if (sIdx % 2 != 0)
                    {
                        var rowRange = ws.Cells[currentRow, 1, currentRow, groupDisciplines.Count + 1];
                        rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rowRange.Style.Fill.BackgroundColor.SetColor(ZebraColor);
                    }

                    var pCells = new List<string> { student.Name! };
                    for (int i = 0; i < groupDisciplines.Count; i++)
                    {
                        var dId = groupDisciplines[i]!.Id;
                        int total = scheduleTotalDict.GetValueOrDefault((group.Id, dId), 0);
                        int absent = absentDict.GetValueOrDefault((student.Id, dId), 0);
                        string val = $"{total - absent}/{total}";

                        var cell = ws.Cells[currentRow, i + 2];
                        cell.Value = val;
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        pCells.Add(val);
                    }
                    preview.Rows.Add(new PreviewRow { IsGroupHeader = false, Cells = pCells });
                    currentRow++;
                }
            }

            ws.Column(1).AutoFit();
            ApplyFinalStyle(ws, currentRow - 1, maxCols + 1);

            return new TablePreview { ExcelBytes = await package.GetAsByteArrayAsync(ct), Preview = preview };
        }

        private static void ApplyFinalStyle(ExcelWorksheet ws, int rows, int cols)
        {
            var range = ws.Cells[1, 1, rows, cols];
            range.Style.Border.Top.Style = range.Style.Border.Bottom.Style =
            range.Style.Border.Left.Style = range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }
    }
}