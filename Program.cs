using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudentManagementAPI.Data;
using StudentManagementAPI.Repositories;
using StudentManagementAPI.Services;
using StudentManagementAPI.Middleware;
using System.Text;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 🔥 SERILOG SETUP (IMPORTANT)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Controllers
builder.Services.AddControllers();

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("ThisIsMySuperSecretKeyForJwtToken12345"))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware (IMPORTANT)
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>(); // 🔥 GLOBAL ERROR HANDLING

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();