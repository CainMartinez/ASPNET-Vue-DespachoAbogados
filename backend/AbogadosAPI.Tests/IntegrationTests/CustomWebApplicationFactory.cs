using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AbogadosAPI.Data;

namespace AbogadosAPI.Tests.IntegrationTests;

/// <summary>
/// Factory personalizada para crear la aplicación web de pruebas
/// </summary>
/// <remarks>
/// Configura la aplicación con una base de datos en memoria para tests de integración
/// </remarks>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Configura el host web para los tests
    /// </summary>
    /// <param name="builder">Constructor del host web</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Establecer variable de entorno para evitar configuración de MySQL en Program.cs
        Environment.SetEnvironmentVariable("USE_IN_MEMORY_DATABASE", "true");

        builder.ConfigureServices(services =>
        {
            // Agregar contexto de base de datos en memoria para tests
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            // Crear y sembrar la base de datos
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                SeedTestData(db);
            }
        });
    }

    /// <summary>
    /// Inserta datos de prueba en la base de datos
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
    private static void SeedTestData(ApplicationDbContext context)
    {
        var cliente = new AbogadosAPI.Models.Cliente
        {
            Id = 100,
            Nombre = "Test",
            Apellidos = "Integration",
            DniCif = "TEST123",
            Email = "test@integration.com",
            Telefono = "600000000",
            FechaAlta = DateTime.Now
        };

        context.Clientes.Add(cliente);
        context.SaveChanges();
    }
}
