using MonitorErrores.Data;
using MonitorErrores.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=monitorerrores.db"));

builder.Services.AddScoped<ErrorService>();

builder.Services.AddScoped<ErrorKnowledgeService>();

builder.Services.AddScoped<IAService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();


