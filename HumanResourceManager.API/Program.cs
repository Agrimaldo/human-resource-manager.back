using HumanResourceManager.Domain.Interfaces;
using HumanResourceManager.Domain.Services;
using HumanResourceManager.Infra.Context;
using HumanResourceManager.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

//builder.Host.UseSerilog((context, configuration) =>
//{
//    configuration.WriteTo.Console().CreateLogger();
//});
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);

builder.Services.AddDbContext<PostgresqlContext>(options =>
{
    var mainConnecttion = builder.Configuration.GetConnectionString("postgreSQL");
    options.UseNpgsql(mainConnecttion);
});

builder.Services.AddScoped<IRepository, Repository>(serviceProvider =>
{
    var context = serviceProvider.GetService<PostgresqlContext>()!;
    context.Database.SetCommandTimeout(400);
    return new Repository(context);
});

builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddCors(a => a.AddPolicy("General", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors();
app.Run();

Log.CloseAndFlush();