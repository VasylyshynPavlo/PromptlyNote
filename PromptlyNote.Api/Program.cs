using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PromptlyNote.Api.Middlewares;
using PromptlyNote.Core.Interfaces;
using PromptlyNote.Core.Interfaces.Repositories;
using PromptlyNote.Core.Interfaces.Services;
using PromptlyNote.Data;
using PromptlyNote.Data.Repositories;
using PromptlyNote.Services.Mapping;
using PromptlyNote.Services.Services;
using Scalar.AspNetCore;
using System.Data.Entity;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskListService, TaskListService>();
builder.Services.AddScoped<IToDoTaskService, ToDoTaskService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGoogleTokenProtector, GoogleTokenProtector>();
builder.Services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();

// Repositories
builder.Services.AddScoped<IToDoTaskRepository, ToDoTaskRepository>();
builder.Services.AddScoped<ITaskListRepository, TaskListRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IGoogleCalendarConnectionRepository, GoogleCalendarConnectionRepository>();

// Database settings
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection")
    ?? throw new InvalidOperationException("Database connection string is missing in appsettings.json");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
    options.AddPolicy("ProductionFrontendPolicy", policy =>
    {
        policy.WithOrigins("https://promptlynote.pashhun.pp.ua")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];
if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException(
        "JWT secret is missing. Set 'JwtSettings:Secret' via user-secrets (dev) or environment variables.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.TryGetValue("auth_token", out var token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(UserProfile).Assembly);
    cfg.AddMaps(typeof(CategoryProfile).Assembly);
    cfg.AddMaps(typeof(TaskListProfile).Assembly);
    cfg.AddMaps(typeof(ToDoTaskProfile).Assembly);
    cfg.AddMaps(typeof(SubTaskProfile).Assembly);
});

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<ApplicationDbContext>();

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddValidation();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        ApplicationDbSeeder.SeedAsync(context).GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseHttpsRedirection();
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseCors("FrontendPolicy");

}
else
{
    app.UseCors("ProductionFrontendPolicy");
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();