namespace LessonLink.BusinessLogic.Common;

public class JwtSettings
{
    public const string JwtSettingsKey = "Jwt";

    public string SigningKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenValidityInMinutes { get; set; }
    public int RefreshTokenValidityInMinutes { get; set; }
}
