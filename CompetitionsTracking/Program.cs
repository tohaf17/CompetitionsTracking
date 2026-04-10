using CompetitionsTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CompetitionsTracking.Repositories.Interfaces;
using CompetitionsTracking.Repositories.Repositories;
using CompetitionsTracking.Services.Implementations;
using FluentValidation;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CompetitionsTrackingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Data Access
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Auto-Register all specific Repositories via Reflection
var repoTypes = typeof(Repository<>).Assembly.GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository") && t.Name != "Repository`1");
foreach (var type in repoTypes)
{
    var iType = type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}");
    if (iType != null) builder.Services.AddScoped(iType, type);
}

// Auto-Register all Services via Reflection
var serviceTypes = typeof(ApparatusService).Assembly.GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"));
foreach (var type in serviceTypes)
{
    var iType = type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}");
    if (iType != null) builder.Services.AddScoped(iType, type);
}

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CompetitionsTracking.Application.Validators.Person.PersonRequestDtoValidator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CompetitionsTrackingDbContext>();
    // Optionally apply migrations before seeding: context.Database.Migrate();
    DatabaseSeeder.Seed(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI();
    app.UseSwagger();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
