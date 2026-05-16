using ApiGuevaraLibrerias.Constants;
using ApiGuevaraLibrerias.Extensions;
using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Responses;
using ApiGuevaraLibrerias.Repository;
using ApiGuevaraLibrerias.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
ApiGuevaraLibrerias.Mapping.MapsterConfig.RegisterMappings();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddJwtAuthenticationConfiguration(builder.Configuration);
builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value!.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray()
            );

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "Errores de validación",
            Data = errors
        };

        return new BadRequestObjectResult(response);
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddCorsConfiguration();

var app = builder.Build();

app.UseSwaggerConfiguration();

await app.UseDatabaseMigrationWithSeedConfiguration();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors(PolicyNames.AllowSpecificOrigin);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();