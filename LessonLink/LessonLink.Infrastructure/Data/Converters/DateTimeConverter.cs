using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LessonLink.Infrastructure.Data.Converters;

/// <summary>
/// Provides EF Core value converters for DateTime handling to ensure all dates are stored and retrieved in UTC.
/// </summary>
public static class DateTimeConverter
{
    /// <summary>
    /// Converter for non-nullable DateTime properties.
    /// Converts to UTC when saving to database and ensures UTC kind when reading from database.
    /// </summary>
    public static ValueConverter<DateTime, DateTime> UtcConverter { get; } = new ValueConverter<DateTime, DateTime>(
        v => v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
    );

    /// <summary>
    /// Converter for nullable DateTime properties.
    /// Converts to UTC when saving to database and ensures UTC kind when reading from database.
    /// </summary>
    public static ValueConverter<DateTime?, DateTime?> NullableUtcConverter { get; } = new ValueConverter<DateTime?, DateTime?>(
        v => v.HasValue ? v.Value.ToUniversalTime() : null,
        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
    );
}
