using System.Text;
using FileGuard.API.ExceptionHandlers;
using FileGuard.API.Extensions;
using FileGuard.Identity.DataAccess;
using FileGuard.Identity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:7370/",
            ValidAudience = "http://localhost:7370/",
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<FileGuardDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AppPolicy",
        builder =>
        {
            builder.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// *******************************
// Configure application services
// *******************************
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();
app.UseExceptionHandler();

app.UseCors("AppPolicy");

app.MapControllers();

app.Run();
