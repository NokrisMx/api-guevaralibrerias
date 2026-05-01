using System.Text;
using ApiEcommerce.Data;
using ApiGuevaraLibrerias.Constants;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ApiGuevaraLibrerias.Extensions;

public static class ServiceExtensions
{
    public static void AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
       {
           options.AddPolicy(PolicyNames.AllowSpecificOrigin, builder =>
           {
               builder.AllowAnyOrigin()// Permitir todas las fuentes (se pueden especificar domionios especificos con WithOrigins())
                      .AllowAnyHeader()
                      .AllowAnyMethod();
           });
       });
    }

    public static void AddApiVersioningConfiguration(this IServiceCollection services)
    {
        var apiVersioningBuilder = services.AddApiVersioning(option =>
        {
            option.AssumeDefaultVersionWhenUnspecified = true;
            option.DefaultApiVersion = new ApiVersion(1, 0);
            option.ReportApiVersions = true;
        });

        apiVersioningBuilder.AddApiExplorer(option =>
        {
            option.GroupNameFormat = "'v'VVV";
            option.SubstituteApiVersionInUrl = true;
        });
    }

    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // JWT Bearer
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                      "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                      "Ejemplo: \"12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
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
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            // Swagger v1
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "API Guevara Librerias",
                Description = "API para gestionar libros y usuarios",
                TermsOfService = new Uri("https://cv-guevaraaldo-dev.netlify.app/"),
                Contact = new OpenApiContact
                {
                    Name = "Aldo Guevara Muñoz",
                    Url = new Uri("https://cv-guevaraaldo-dev.netlify.app")
                },
                License = new OpenApiLicense
                {
                    Name = "Licencia de uso",
                    Url = new Uri("https://cv-guevaraaldo-dev.netlify.app/")
                }
            });
        });
    }

    public static void AddJwtAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");

        if (string.IsNullOrEmpty(secretKey))
            throw new InvalidOperationException("La clave secreta no está configurada.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // dev false, prod true
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)
                ),

                ValidateIssuer = false, // No validamos el emisor en este ejemplo, pero en producción deberías hacerlo
                ValidateAudience = false // No validamos la audiencia en este ejemplo, pero en producción deberías hacerlo
            };
        });
    }

    public static void UseDatabaseMigrationWithSeedConfiguration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Database.Migrate();

        DataSeeder.SeedData(db);
    }

    public static void UseSwaggerConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        }
    }
}
