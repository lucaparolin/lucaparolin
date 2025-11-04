using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Core.Interfaces.Services;
using NugoloFamily.Core.Services;
using NugoloFamily.Infrastructure.Data.Repositories;
using NugoloFamily.Shared.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Configurazione servizi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "NugoloFamily API",
        Version = "v1",
        Description = "API per l'assistente familiare digitale NugoloFamily"
    });
});

// Configurazione CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Registrazione Repository (ADO.NET)
builder.Services.AddScoped<IFamiglieRepository, FamiglieRepository>();
builder.Services.AddScoped<IUtentiRepository, UtentiRepository>();
builder.Services.AddScoped<IAssistentiRepository, AssistentiRepository>();
builder.Services.AddScoped<IAssistentiFamiglieRepository, AssistentiFamiglieRepository>();
builder.Services.AddScoped<IConversazioniRepository, ConversazioniRepository>();
builder.Services.AddScoped<IMessaggiRepository, MessaggiRepository>();
builder.Services.AddScoped<IDocumentiRepository, DocumentiRepository>();
builder.Services.AddScoped<IConfigurazioniAIRepository, ConfigurazioniAIRepository>();
builder.Services.AddScoped<ILogAttivitaRepository, LogAttivitaRepository>();

// Configurazione JWT Helper
var jwtSecret = builder.Configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret non configurato");
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "NugoloFamily";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "NugoloFamilyClients";
var jwtExpiration = int.Parse(builder.Configuration["JwtSettings:ExpirationMinutes"] ?? "60");

builder.Services.AddSingleton(new JwtHelper(jwtSecret, jwtIssuer, jwtAudience, jwtExpiration));

// Registrazione Servizi Business Logic
builder.Services.AddScoped<IFamiglieService, FamiglieService>();
builder.Services.AddScoped<IUtentiService, UtentiService>();
builder.Services.AddScoped<IAssistentiService, AssistentiService>();
builder.Services.AddScoped<IConversazioniService, ConversazioniService>();
builder.Services.AddScoped<IMessaggiService, MessaggiService>();
builder.Services.AddScoped<IDocumentiService, DocumentiService>();

// Configurazione JWT Authentication (da implementare completamente)
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options => { ... });

var app = builder.Build();

// Configurazione HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Endpoint di health check
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Service = "NugoloFamily API"
}));

app.Run();
