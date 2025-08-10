using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Venta.Api.Middleware;
using Venta.API.Configurations;
using Venta.API.Security;
using Venta.Application;
using Venta.CrossCutting;
using Venta.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddConfigServer(
    LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    })
);

var dataProtectionSection = builder.Configuration.GetSection("DataProtection");
var certificate = Helper.LoadCertificate(builder.Configuration.GetSection("DataProtection:Certificate"));

// ------------ Optional: connect Redis ------------
IConnectionMultiplexer? redis = null;
var redisCxn = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrWhiteSpace(redisCxn))
{
    // throws if invalid -> fail fast
    redis = ConnectionMultiplexer.Connect(redisCxn);
}

var dataProtection = builder.Services.AddDataProtection().SetApplicationName("VentaApi");

// Persist key ring either to Redis or to filesystem
if (redis is not null)
{
    // Single Redis key "DataProtection-Keys" stores the key ring XML (encrypted by the cert below)
    dataProtection.PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
}
else
{
    var folder = dataProtectionSection["KeysFolder"] ?? "./keyring";
    dataProtection.PersistKeysToFileSystem(new DirectoryInfo(folder));
}

// Encrypt the key ring at rest with an X.509 certificate (required for Redis; recommended for FS)
if (certificate is not null)
{
    if (!certificate.HasPrivateKey)
        throw new InvalidOperationException("The X.509 certificate must include a private key.");

    dataProtection.ProtectKeysWithCertificate(certificate);
}

//builder.Services.AddDataProtection()
//    .SetApplicationName("VentaApi")
//    .ProtectKeysWithAzureKeyVault(new Uri("https://<kv-name>.vault.azure.net/keys/<key-name>/<version>"), new DefaultAzureCredential())
//    .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
//    {
//        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
//        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
//    });

// Add services to the container.

builder.Services.AddControllers();
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

// 0) Hide server banner
builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (ctx, next) =>
{
    var h = ctx.Response.Headers;

    // Stop MIME sniffing (safe for JSON)
    h["X-Content-Type-Options"] = "nosniff";

    // Don’t leak referrers (API doesn’t need them)
    h["Referrer-Policy"] = "no-referrer";

    // Optional: block framing (harmless for APIs)
    h["X-Frame-Options"] = "DENY";

    // Disable caching for sensitive paths (auth, tokens). Tweak as needed.
    if (ctx.Request.Path.StartsWithSegments("/auth") ||
        ctx.Request.Path.StartsWithSegments("/token"))
    {
        h["Cache-Control"] = "no-store";
        h["Pragma"] = "no-cache";
        h["Expires"] = "0";
    }

    await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.UseHealthCheckConfiguration();

//Adicionar middleware customizado para tratar las excepciones
app.UseCustomExceptionHandler();

// ------------ Demo endpoints using per-purpose protectors ------------
app.MapGet("/protect/{text}", (IDataProtectionProvider provider, string text) =>
{
    var protector = provider.CreateProtector("VentaApi:Demo:v1");
    var ciphertext = protector.Protect(text);
    return Results.Text(ciphertext, "text/plain");
});

app.MapPost("/unprotect", async (IDataProtectionProvider provider, HttpRequest req) =>
{
    using var reader = new StreamReader(req.Body);
    var blob = await reader.ReadToEndAsync();
    var protector = provider.CreateProtector("VentaApi:Demo:v1");
    try
    {
        var plaintext = protector.Unprotect(blob);
        return Results.Ok(new { plaintext });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/health", () => Results.Ok(new { ok = true }));

app.MapControllers();

app.Run();
