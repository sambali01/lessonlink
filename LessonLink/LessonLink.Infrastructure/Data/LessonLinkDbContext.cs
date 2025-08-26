using LessonLink.BusinessLogic.Common;
using LessonLink.BusinessLogic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Data;

public class LessonLinkDbContext : IdentityDbContext<User>
{
    public LessonLinkDbContext(DbContextOptions<LessonLinkDbContext> options)
        : base(options)
    {
    }

    public DbSet<Subject> Subjects { get; set; }
    public DbSet<TeacherSubject> TeacherSubjects { get; set; }
    public DbSet<AvailableSlot> AvailableSlots { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Teacher configuration
        modelBuilder.Entity<Teacher>()
            .HasKey(t => t.UserId);

        modelBuilder.Entity<Teacher>()
            .HasOne(t => t.User)
            .WithOne()
            .HasForeignKey<Teacher>(t => t.UserId);

        // TeacherSubject join entity configuration
        modelBuilder.Entity<TeacherSubject>()
            .HasKey(ts => new { ts.TeacherId, ts.SubjectId });

        modelBuilder.Entity<TeacherSubject>()
            .HasOne(ts => ts.Teacher)
            .WithMany(t => t.TeacherSubjects)
            .HasForeignKey(ts => ts.TeacherId);

        modelBuilder.Entity<TeacherSubject>()
            .HasOne(ts => ts.Subject)
            .WithMany(s => s.TeacherSubjects)
            .HasForeignKey(ts => ts.SubjectId);

        // AvailableSlot configuration
        modelBuilder.Entity<AvailableSlot>()
            .HasOne(slot => slot.Teacher)
            .WithMany(t => t.AvailableSlots)
            .HasForeignKey(slot => slot.TeacherId);

        // Booking configuration
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Student)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.StudentId);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.AvailableSlot)
            .WithMany()
            .HasForeignKey(b => b.AvailableSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        // Roles configuration
        var roles = RoleInitializer.GetIdentityRoles();
        modelBuilder.Entity<IdentityRole>().HasData(roles);

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>()
            .HasKey(rt => rt.Value);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId);
    }
}
