
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using ToDoList.Models.Entities;
using ToDoList.Helpers;
using ToDoList.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ToDoList.ActionFilters;
using ToDoList.Services.Auth;
using ToDoList.Services;



namespace ToDoList.Extentions;

public static class AppServicesExtensions
{

    public static IServiceCollection AddIdentityConfig(this IServiceCollection services)
    {
        // Register main User identity (with login, cookies, etc.)
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<AppData>()
        .AddDefaultTokenProviders();

        
        services.AddMemoryCache();
        services.AddSession();
        return services;
    }

    public static IServiceCollection AddDbConnection(this IServiceCollection services)
    {
        var connectionString = ConfigProvider.config.GetConnectionString("C_str");

        services.AddDbContext<AppData>(options =>
        {
            options.UseSqlite(connectionString);
        });

        return services;
    }

    public static IServiceCollection AddCorss(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
        });
        return services;
    }



    public static IServiceCollection AddAuthConfig(this IServiceCollection services)
    {
        string jwtSignInKey = ConfigProvider.config.GetSection("Jwt:SecretKey").Get<string>();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "Supernova-iq.com", // Ensure this matches the issuer in token
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSignInKey))
            };
        });

        services.AddAuthorization();
        return services;
    }


    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(delegate (SwaggerGenOptions opt)
        {
            opt.SwaggerDoc("Main", new OpenApiInfo
            {
                Version = "v1",
                Title = "SuperNova",
                Description = "ToDoList API Swagger",
                Contact = new OpenApiContact
                {
                    Name = "Supernova IQ",
                    Email = "info@supernova-iq.com",
                    Url = new Uri("https://supernova-iq.com/")
                }
            });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header, \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[0]
        } });

        });
        return services;
    }
   
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthServices, AuthServices>();
        services.AddScoped<IToDoListService, ToDoListService>();

        return services;
    }

    public static IServiceCollection AddSizeLimit(this IServiceCollection services)
    {
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 10 * 1024; // 10 KB for example
        });
        return services;
    }


}
