using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using SocialNetwork.Application;
using SocialNetwork.Application.Interfaces;
using SocialNetwork.Application.Services;
using SocialNetwork.Database.Models;
using SocialNetwork.Domain.Models;
using SocialNetwork.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var _configuration = builder.Configuration;
// Add services to the container.

{
    // for ef
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        //options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        options.UseNpgsql(_configuration.GetConnectionString("PostgresqlConnection"));
    });

    // identity settings
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    // automapper 
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // di
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(_configuration);

    // configuring identity
    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
        options.SignIn.RequireConfirmedAccount = false;
    });

    builder.Services.AddControllers();
    
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    // bearer feature
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
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
                new string[]{ }
            }
        });
    });

}

// Serilog
builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

{

    // serilog middleware?
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
