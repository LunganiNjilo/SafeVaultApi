using Application.Extensions;
using Application.Interfaces;
using Infrastructure.Extensions;
using Infrastructure.HttpProviders;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Migrations;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using SafeVaultApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173" // Vue dev server

            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// configf
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IHttpPaymentProvider, HttpPaymentProvider>(client =>
{
    var baseUrl = builder.Configuration["PaymentApi:BaseUrl"];

    if (string.IsNullOrEmpty(baseUrl))
        throw new Exception("PaymentApi:BaseUrl is missing!");

    client.BaseAddress = new Uri(baseUrl);
});


// DbContext
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Infrastructure registrations
builder.Services.AddInfrastructureDependencies();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application services
builder.Services.AddApplicationDependencies();

// Global error handling
builder.Services.AddCustomErrorHandling();

var app = builder.Build();

// apply migrations + seed
if (!app.Environment.IsEnvironment("Testing"))
{
    await app.ApplyMigrationsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseGlobalExceptionHandling();
app.UseCors("AllowFrontend");
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
