using MonitorErrores.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<ErrorService>();
builder.Services.AddScoped<ErrorKnowledgeService>();
builder.Services.AddScoped<IIAService, IAService>();
builder.Services.AddScoped<DiagnosticoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.UseCors("Frontend");
app.MapControllers();

app.Run();