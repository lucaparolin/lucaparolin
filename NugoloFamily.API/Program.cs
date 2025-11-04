using NugoloFamily.Core.Interfaces.Repositories;
using NugoloFamily.Infrastructure.Data.Repositories;

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

// Registrazione Servizi
// builder.Services.AddScoped<IFamiglieService, FamiglieService>();
// Aggiungere qui gli altri servizi quando verranno creati

// Configurazione JWT Authentication (da implementare)
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
