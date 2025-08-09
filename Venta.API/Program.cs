using Microsoft.AspNetCore.RateLimiting;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Venta.Api.Middleware;
using Venta.API.Configurations;
using Venta.API.Security;
using Venta.Application;
using Venta.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddConfigServer(
    LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    })
    );
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Capa de aplicacion
builder.Services.AddApplication();

builder.Configuration.AddAKeyVault(builder.Configuration);

//Capa de infra
//var connectionString = builder.Configuration.GetConnectionString("dbVenta-cnx");
var connectionString = builder.Configuration["dbVenta-cnx"];
builder.Services.AddInfraestructure(builder.Configuration);
builder.Services.AddAthenticationByJWT(builder.Configuration);
builder.Services.AddHealthCheckConfiguration(builder.Configuration);
//Rate limit
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("AuthPolicy", configure =>
    {
        configure.PermitLimit = 5;
        configure.Window = TimeSpan.FromMinutes(1);
        configure.QueueLimit = 0;
    });

    options.AddSlidingWindowLimiter("ApiPolicy", configure =>
    {
        configure.PermitLimit = 100;
        configure.Window = TimeSpan.FromMinutes(1);
        configure.SegmentsPerWindow = 4;
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseHealthCheckConfiguration();

//Adicionar middleware customizado para tratar las excepciones
app.UseCustomExceptionHandler();

app.MapControllers();

app.Run();
