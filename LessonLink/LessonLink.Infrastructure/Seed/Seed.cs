using LessonLink.BusinessLogic.Models;
using LessonLink.Infrastructure.Data;
using LessonLink.Infrastructure.Seed.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LessonLink.Infrastructure.Seed;

public class Seed
{
    private static string? ReadEmbeddedResource(string resourceName)
    {
        var assembly = typeof(Seed).Assembly;
        var fullResourceName = $"LessonLink.Infrastructure.Seed.SeedData.{resourceName}";

        Console.WriteLine($"Looking for embedded resource: {fullResourceName}");

        using var stream = assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
        {
            var availableResources = assembly.GetManifestResourceNames();
            Console.WriteLine($"Available resources: {string.Join(", ", availableResources)}");
            throw new InvalidOperationException($"Could not find embedded resource: {fullResourceName}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task SeedData(LessonLinkDbContext context, UserManager<User> userManager)
    {
        // If there is data already present in any of the key tables, do not seed
        if (await userManager.Users.AnyAsync() ||
            await context.Subjects.AnyAsync() ||
            await context.Teachers.AnyAsync() ||
            await context.TeacherSubjects.AnyAsync() ||
            await context.AvailableSlots.AnyAsync())
        {
            Console.WriteLine("Database already contains data. Skipping seed.");
            return;
        }

        // Use a transaction for all seed operations
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            Console.WriteLine("Starting seed process...");

            // 1. Seed Subjects
            await SeedSubjects(context);

            // 2. Seed Admin User
            await SeedAdminUser(userManager);

            // 3. Seed Students
            await SeedStudents(userManager);

            // 4. Seed Teachers
            await SeedTeachers(context, userManager);

            // 5. Seed TeacherSubjects
            await SeedTeacherSubjects(context);

            // 6. Seed AvailableSlots
            await SeedAvailableSlots(context);

            // 7. Seed Bookings
            await SeedBookings(context);

            await transaction.CommitAsync();
            Console.WriteLine("All seed data successfully loaded!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Seed failed: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public static async Task ClearAllData(LessonLinkDbContext context, UserManager<User> userManager)
    {
        // Clear in reverse order of dependencies
        context.Bookings.RemoveRange(context.Bookings);
        context.AvailableSlots.RemoveRange(context.AvailableSlots);
        context.TeacherSubjects.RemoveRange(context.TeacherSubjects);
        context.Teachers.RemoveRange(context.Teachers);
        context.Subjects.RemoveRange(context.Subjects);
        context.RefreshTokens.RemoveRange(context.RefreshTokens);

        await context.SaveChangesAsync();

        var users = await userManager.Users.ToListAsync();
        foreach (var user in users)
        {
            await userManager.DeleteAsync(user);
        }
    }

    private static async Task SeedAdminUser(UserManager<User> userManager)
    {
        Console.WriteLine("Seeding admin user...");

        var admin = new User
        {
            UserName = "admin@lessonlink.com",
            Email = "admin@lessonlink.com",
            FirstName = "Admin",
            SurName = "User",
            NickName = "Admin",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, "Pa$$w0rd");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
            Console.WriteLine("Admin user created successfully");
        }
        else
        {
            Console.WriteLine($"Failed to create admin: {result.Errors.First().Description}");
        }
    }

    private static async Task SeedStudents(UserManager<User> userManager)
    {
        Console.WriteLine("Seeding students...");

        var studentData = ReadEmbeddedResource("StudentSeedData.json");
        if (studentData == null)
        {
            Console.WriteLine("No student data found");
            return;
        }

        var students = JsonSerializer.Deserialize<List<StudentSeedModel>>(studentData, jsonSerializerOptions);

        if (students == null || students.Count == 0)
        {
            Console.WriteLine("No students in seed data");
            return;
        }

        foreach (var studentModel in students)
        {
            var student = new User
            {
                Id = studentModel.Id,
                Email = studentModel.Email,
                UserName = studentModel.Email,
                FirstName = studentModel.FirstName,
                SurName = studentModel.SurName,
                NickName = studentModel.NickName,
                ImageUrl = studentModel.ImageUrl,
                EmailConfirmed = true,
                Bookings = [] // Initialize collection
            };

            var result = await userManager.CreateAsync(student, "Pa$$w0rd");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(student, "Student");
                Console.WriteLine($"Student created: {student.Email}");
            }
            else
            {
                Console.WriteLine($"Failed to create student {student.Email}: {result.Errors.First().Description}");
            }
        }
    }

    private static async Task SeedTeachers(LessonLinkDbContext context, UserManager<User> userManager)
    {
        Console.WriteLine("Seeding teachers...");

        var teacherData = ReadEmbeddedResource("TeacherSeedData.json");
        if (teacherData == null)
        {
            Console.WriteLine("No teacher data found");
            return;
        }

        var teacherModels = JsonSerializer.Deserialize<List<TeacherSeedModel>>(teacherData, jsonSerializerOptions);

        if (teacherModels == null || teacherModels.Count == 0)
        {
            Console.WriteLine("No teachers in seed data");
            return;
        }

        var teacherProfiles = new List<Teacher>();

        foreach (var teacherModel in teacherModels)
        {
            // Create User with initialized collections
            var user = new User
            {
                Id = teacherModel.UserId,
                Email = teacherModel.Email,
                UserName = teacherModel.Email,
                FirstName = teacherModel.FirstName,
                SurName = teacherModel.SurName,
                NickName = teacherModel.NickName,
                ImageUrl = teacherModel.ImageUrl,
                EmailConfirmed = true,
                Bookings = [] // Initialize collection
            };

            var result = await userManager.CreateAsync(user, "Pa$$w0rd");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Teacher");

                // Create Teacher profile with navigation property and initialized collections
                var teacherProfile = new Teacher
                {
                    UserId = teacherModel.UserId,
                    User = user, // Set navigation property
                    AcceptsOnline = teacherModel.AcceptsOnline,
                    AcceptsInPerson = teacherModel.AcceptsInPerson,
                    Location = teacherModel.Location,
                    HourlyRate = teacherModel.HourlyRate,
                    Description = teacherModel.Description,
                    Contact = teacherModel.Contact,
                    TeacherSubjects = [], // Initialize collection
                    AvailableSlots = [] // Initialize collection
                };

                teacherProfiles.Add(teacherProfile);
                Console.WriteLine($"Teacher created: {user.Email}");
            }
            else
            {
                Console.WriteLine($"Failed to create teacher {user.Email}: {result.Errors.First().Description}");
            }
        }

        // Add all teacher profiles and save
        context.Teachers.AddRange(teacherProfiles);
        await context.SaveChangesAsync();
        Console.WriteLine($"Saved {teacherProfiles.Count} teacher profiles");
    }

    private static async Task SeedSubjects(LessonLinkDbContext context)
    {
        Console.WriteLine("Seeding subjects...");

        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Subjects ON");

        var subjectData = ReadEmbeddedResource("SubjectSeedData.json");
        if (subjectData == null)
        {
            Console.WriteLine("No subject data found");
            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Subjects OFF");
            return;
        }

        var subjectModels = JsonSerializer.Deserialize<List<SubjectSeedModel>>(subjectData, jsonSerializerOptions);

        if (subjectModels == null || subjectModels.Count == 0)
        {
            Console.WriteLine("No subjects in seed data");
            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Subjects OFF");
            return;
        }

        var subjects = new List<Subject>();
        foreach (var model in subjectModels)
        {
            var subject = new Subject
            {
                Id = model.Id,
                Name = model.Name,
                TeacherSubjects = []
            };
            subjects.Add(subject);
        }

        context.Subjects.AddRange(subjects);
        await context.SaveChangesAsync();

        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Subjects OFF");
        Console.WriteLine($"Saved {subjects.Count} subjects");
    }

    private static async Task SeedTeacherSubjects(LessonLinkDbContext context)
    {
        Console.WriteLine("Seeding teacher-subject relationships...");

        var teacherSubjectData = ReadEmbeddedResource("TeacherSubjectSeedData.json");
        if (teacherSubjectData == null)
        {
            Console.WriteLine("No teacher subject data found");
            return;
        }

        var teacherSubjectModels = JsonSerializer.Deserialize<List<TeacherSubjectSeedModel>>(teacherSubjectData, jsonSerializerOptions);

        if (teacherSubjectModels == null || teacherSubjectModels.Count == 0)
        {
            Console.WriteLine("No teacher subjects in seed data");
            return;
        }

        var teacherSubjects = new List<TeacherSubject>();

        foreach (var model in teacherSubjectModels)
        {
            // Load tracked entities from context
            var teacher = await context.Teachers.FindAsync(model.TeacherId);
            var subject = await context.Subjects.FindAsync(model.SubjectId);

            if (teacher != null && subject != null)
            {
                var teacherSubject = new TeacherSubject
                {
                    TeacherId = model.TeacherId,
                    Teacher = teacher, // Set navigation property
                    SubjectId = model.SubjectId,
                    Subject = subject // Set navigation property
                };

                // Add to both sides of the relationship
                teacher.TeacherSubjects.Add(teacherSubject);
                subject.TeacherSubjects.Add(teacherSubject);

                teacherSubjects.Add(teacherSubject);
            }
            else
            {
                Console.WriteLine($"Warning: Could not find Teacher {model.TeacherId} or Subject {model.SubjectId}");
            }
        }

        context.TeacherSubjects.AddRange(teacherSubjects);
        await context.SaveChangesAsync();
        Console.WriteLine($"Saved {teacherSubjects.Count} teacher-subject relationships");
    }

    private static async Task SeedAvailableSlots(LessonLinkDbContext context)
    {
        Console.WriteLine("Seeding available slots...");

        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT AvailableSlots ON");

        var slotData = ReadEmbeddedResource("AvailableSlotSeedData.json");
        if (slotData == null)
        {
            Console.WriteLine("No slot data found");
            return;
        }

        var slotModels = JsonSerializer.Deserialize<List<AvailableSlotSeedModel>>(slotData, jsonSerializerOptions);

        if (slotModels == null || slotModels.Count == 0)
        {
            Console.WriteLine("No available slots in seed data");
            return;
        }

        var slots = new List<AvailableSlot>();
        var today = DateTime.UtcNow.Date;

        foreach (var model in slotModels)
        {
            // Load tracked teacher entity
            var teacher = await context.Teachers.FindAsync(model.TeacherId);

            if (teacher != null)
            {
                // Calculate the date based on DayOffset
                var slotDate = today.AddDays(model.DayOffset);

                // Parse time strings (format: "HH:mm")
                var startTimeParts = model.StartTime.Split(':');
                var endTimeParts = model.EndTime.Split(':');

                var startTime = new DateTime(
                    slotDate.Year, slotDate.Month, slotDate.Day,
                    int.Parse(startTimeParts[0]), int.Parse(startTimeParts[1]), 0,
                    DateTimeKind.Utc);

                var endTime = new DateTime(
                    slotDate.Year, slotDate.Month, slotDate.Day,
                    int.Parse(endTimeParts[0]), int.Parse(endTimeParts[1]), 0,
                    DateTimeKind.Utc);

                var slot = new AvailableSlot
                {
                    Id = model.Id,
                    TeacherId = model.TeacherId,
                    Teacher = teacher, // Set navigation property
                    StartTime = startTime,
                    EndTime = endTime
                };

                // Add to teacher's collection
                teacher.AvailableSlots.Add(slot);

                slots.Add(slot);
            }
            else
            {
                Console.WriteLine($"Warning: Could not find Teacher {model.TeacherId}");
            }
        }

        context.AvailableSlots.AddRange(slots);
        await context.SaveChangesAsync();

        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT AvailableSlots OFF");
        Console.WriteLine($"Saved {slots.Count} available slots");
    }

    private static async Task SeedBookings(LessonLinkDbContext context)
    {
        Console.WriteLine("Seeding bookings...");

        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Bookings ON");

        var bookingData = ReadEmbeddedResource("BookingSeedData.json");
        if (bookingData == null)
        {
            Console.WriteLine("No booking data found");
            return;
        }

        var bookingModels = JsonSerializer.Deserialize<List<BookingSeedModel>>(bookingData, jsonSerializerOptions);

        if (bookingModels == null || bookingModels.Count == 0)
        {
            Console.WriteLine("No bookings in seed data");
            return;
        }

        var bookings = new List<Booking>();
        var now = DateTime.UtcNow;

        foreach (var model in bookingModels)
        {
            // Load tracked entities
            var availableSlot = await context.AvailableSlots.FindAsync(model.AvailableSlotId);
            var student = await context.Users.FindAsync(model.StudentId);

            if (availableSlot != null && student != null)
            {
                // Calculate CreatedAt based on DaysAgoCreated
                var createdAt = now.AddDays(-model.DaysAgoCreated);

                var booking = new Booking
                {
                    Id = model.Id,
                    AvailableSlotId = model.AvailableSlotId,
                    AvailableSlot = availableSlot, // Set navigation property
                    StudentId = model.StudentId,
                    Student = student, // Set navigation property
                    CreatedAt = createdAt,
                    Status = (BookingStatus)model.Status
                };

                // Add to student's collection
                student.Bookings.Add(booking);

                bookings.Add(booking);
            }
            else
            {
                Console.WriteLine($"Warning: Could not find AvailableSlot {model.AvailableSlotId} or Student {model.StudentId}");
            }
        }

        context.Bookings.AddRange(bookings);
        await context.SaveChangesAsync();

        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Bookings OFF");
        Console.WriteLine($"Saved {bookings.Count} bookings");
    }
}
