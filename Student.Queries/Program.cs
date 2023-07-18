using System.Reflection;
using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StudentQueries.Data;
using StudentQueries.Extensions;
using StudentQueries.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<EventListener>();

var connectionString = builder.Configuration.GetConnectionString("MssqlDatabase");

builder.Services.AddDbContext<AppDbContext>(
    opt => { opt.UseSqlServer(connectionString); });

builder.Services.AddSingleton(
    sp =>
    {
        var opt = sp.GetRequiredService<IOptions<ServiceBusSettings>>();
        return new ServiceBusClient(opt.Value.ConnectionString);
    });

builder.Services.Configure<ServiceBusSettings>(opt
    => builder.Configuration.GetSection(ServiceBusSettings.ServiceBus).Bind(opt));

builder.Services.AddMediatR(opt
    => opt.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddGrpcWithValidators();

var app = builder.Build();

app.MapGrpcService<StudentQueries.Services.StudentQueries>();


// Configure the HTTP request pipeline.
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

public partial class Program { }