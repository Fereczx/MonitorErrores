using MonitorErrores.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<ErrorService>();
builder.Services.AddScoped<ErrorKnowledgeService>();
builder.Services.AddScoped<IAService>();
builder.Services.AddScoped<DiagnosticoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.MapControllers();

app.Run();