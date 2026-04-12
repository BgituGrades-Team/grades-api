using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BgituGrades.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Presence> Presences { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<ReportSnapshot> ReportSnapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>()
                .Property(u => u.Type)
                .HasConversion(new EnumToStringConverter<ClassType>());
            modelBuilder.Entity<Presence>()
                .Property(u => u.IsPresent)
                .HasConversion(new EnumToStringConverter<PresenceType>());
            modelBuilder.Entity<ApiKey>().HasKey(k => k.Key);

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.GroupId)
                .HasDatabaseName("IX_Student_GroupId");

            modelBuilder.Entity<Mark>()
                .HasIndex(m => m.StudentId)
                .HasDatabaseName("IX_Mark_StudentId");
            modelBuilder.Entity<Mark>()
                .HasIndex(m => m.WorkId)
                .HasDatabaseName("IX_Mark_WorkId");
            modelBuilder.Entity<Mark>()
                .HasIndex(m => new { m.StudentId, m.WorkId })
                .HasDatabaseName("IX_Mark_StudentId_WorkId");

            modelBuilder.Entity<Presence>()
                .HasIndex(p => p.StudentId)
                .HasDatabaseName("IX_Presence_StudentId");
            modelBuilder.Entity<Presence>()
                .HasIndex(p => p.DisciplineId)
                .HasDatabaseName("IX_Presence_DisciplineId");
            modelBuilder.Entity<Presence>()
                .HasIndex(p => p.ClassId)
                .HasDatabaseName("IX_Presence_ClassId");
            modelBuilder.Entity<Presence>()
                .HasIndex(p => new { p.StudentId, p.Date })
                .HasDatabaseName("IX_Presence_StudentId_Date");

            modelBuilder.Entity<Class>()
                .HasIndex(c => c.GroupId)
                .HasDatabaseName("IX_Class_GroupId");
            modelBuilder.Entity<Class>()
                .HasIndex(c => c.DisciplineId)
                .HasDatabaseName("IX_Class_DisciplineId");
            modelBuilder.Entity<Class>()
                .HasIndex(c => new { c.GroupId, c.DisciplineId })
                .HasDatabaseName("IX_Class_GroupId_DisciplineId");

            modelBuilder.Entity<Work>()
                .HasIndex(w => w.GroupId)
                .HasDatabaseName("IX_Work_GroupId");
            modelBuilder.Entity<Work>()
                .HasIndex(w => w.DisciplineId)
                .HasDatabaseName("IX_Work_DisciplineId");
            modelBuilder.Entity<Work>()
                .HasIndex(w => new { w.DisciplineId, w.GroupId })
                .HasDatabaseName("IX_Work_DisciplineId_GroupId");

            modelBuilder.Entity<Transfer>()
                .HasIndex(t => t.ClassId)
                .HasDatabaseName("IX_Transfer_ClassId");

            modelBuilder.Entity<ApiKey>()
                .HasIndex(k => k.LookupHash)
                .HasDatabaseName("IX_ApiKey_LookupHash");
            modelBuilder.Entity<ApiKey>()
                .HasIndex(k => k.GroupId)
                .HasDatabaseName("IX_ApiKey_GroupId");

            modelBuilder.Entity<ReportSnapshot>()
                .HasIndex(r => r.GroupId)
                .HasDatabaseName("IX_ReportSnapshot_GroupId");
            modelBuilder.Entity<ReportSnapshot>()
                .HasIndex(r => r.DisciplineId)
                .HasDatabaseName("IX_ReportSnapshot_DisciplineId");
            modelBuilder.Entity<ReportSnapshot>()
                .HasIndex(r => new { r.GroupId, r.DisciplineId })
                .HasDatabaseName("IX_ReportSnapshot_GroupId_DisciplineId");
        }
    }
}
