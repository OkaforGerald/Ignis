using Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Services;
using Services.Contracts;
using SharedAPI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Ignis.Presentation.AssemblyReference).Assembly);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("sqlConnection")));

builder.Services.AddScoped<IServiceManager, ServiceManager>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthentication();

builder.Services.AddIdentity<User, IdentityRole>(builder =>
{
    builder.User.RequireUniqueEmail = true;
    builder.Password.RequiredLength = 8;
})
    .AddEntityFrameworkStores<RepositoryContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opts => opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
{
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings")["SigningKey"])),
    ValidIssuer = builder.Configuration.GetSection("JwtSettings")["Issuer"],
    ValidAudience = builder.Configuration.GetSection("JwtSettings")["Audience"],

    ValidateAudience = true,
    ValidateIssuer = true,
    ValidateIssuerSigningKey = true,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(
    error => error.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var contextFeatures = context.Features.Get<IExceptionHandlerFeature>();

        var statusCode = contextFeatures?.Error switch
        {
            _ => 500
        };

        await context.Response.WriteAsync(ResponseDto<string>.Error(statusCode, contextFeatures?.Error.Message).ToString());
    })
    );

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
