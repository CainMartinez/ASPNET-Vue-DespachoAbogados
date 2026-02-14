using Microsoft.EntityFrameworkCore;
using AbogadosAPI.Data;
using AbogadosAPI.Services;
using AbogadosAPI.Services.Reports;
using Microsoft.OpenApi.Models;
using System.Reflection;

/// <summary>
/// Punto de entrada de la aplicación
/// </summary>
/// <remarks>
/// Configura todos los servicios, middleware y base de datos del sistema
/// </remarks>
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuración de Swagger con información del proyecto y comentarios XML
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Gestión de Expedientes - Despacho de Abogados",
        Version = "v1",
        Description = "Sistema de gestión centralizada para expedientes jurídicos, clientes, actuaciones y citas de un despacho de abogados",
        Contact = new OpenApiContact
        {
            Name = "Despacho de Abogados",
            Email = "info@despachoabogados.com"
        }
    });
    
    // Incluir comentarios XML en Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Registrar servicios de la capa de negocio
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IExpedienteService, ExpedienteService>();
builder.Services.AddScoped<IActuacionService, ActuacionService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<IDocumentoService, DocumentoService>();

// Registrar servicios de generación de reportes PDF
builder.Services.AddScoped<ClientesReportService>();
builder.Services.AddScoped<ExpedientesPorEstadoReportService>();
builder.Services.AddScoped<ActuacionesPorExpedienteReportService>();
builder.Services.AddScoped<PdfReportService>();

// Configuración de Base de Datos (MySQL para producción, se omite para tests)
var useInMemoryDatabase = Environment.GetEnvironmentVariable("USE_IN_MEMORY_DATABASE") == "true";
if (!useInMemoryDatabase)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
}
// Si USE_IN_MEMORY_DATABASE=true, no configuramos DbContext aquí
// El CustomWebApplicationFactory lo configurará con InMemory

// Configuración de CORS para desarrollo
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gestión Abogados v1");
        c.RoutePrefix = "swagger"; // Swagger en /swagger
    });
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Crear base de datos si no existe y aplicar migraciones
// Solo ejecutar si no estamos usando InMemory database (tests)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    // Verificar si estamos usando InMemory database (tests)
    var isInMemory = db.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
    
    if (!isInMemory)
    {
        try
        {
            logger.LogInformation("Creando/actualizando base de datos...");
            db.Database.EnsureCreated();
            logger.LogInformation("Base de datos lista");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al crear la base de datos");
        }
    }
    else
    {
        logger.LogInformation("Usando base de datos en memoria (modo test)");
    }
}

app.Run();

/// <summary>
/// Clase Program pública para permitir acceso desde tests de integración
/// </summary>
public partial class Program { }
