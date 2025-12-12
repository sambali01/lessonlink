using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.BusinessLogic.Services;
using LessonLink.Infrastructure.Data;
using LessonLink.Infrastructure.Repositories;
using LessonLink.Infrastructure.Seed;
using LessonLink.WebApi.MiddleWare;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Cross-Origin Resource Sharing (CORS)
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// Configure cookie policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = CookieSecurePolicy.Always;
});

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<LessonLinkDbContext>()
.AddDefaultTokenProviders();

// Configure JWT authentication
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection(JwtSettings.JwtSettingsKey).Bind(jwtSettings);
var key = Encoding.UTF8.GetBytes(jwtSettings.SigningKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,

        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddSingleton(jwtSettings);

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("LessonLinkDb");
builder.Services.AddDbContext<LessonLinkDbContext>(options => options.UseSqlServer(connectionString));

// Add UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add services to the container
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<SubjectService>();
builder.Services.AddScoped<AvailableSlotService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<PhotoService>();

var cloudinarySettingsKey = builder.Configuration.GetSection(CloudinarySettings.CloudinarySettingsKey);
if (cloudinarySettingsKey.Exists())
{
    builder.Services.Configure<CloudinarySettings>(cloudinarySettingsKey);
}

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseMiddleware<ExceptionMiddleWare>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed database with test data
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<LessonLinkDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();

    await context.Database.MigrateAsync();

    if (app.Environment.IsDevelopment())
    {
        await Seed.ClearAllData(context, userManager);
    }

    await Seed.SeedData(context, userManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration or seeding");
}

app.Run();
