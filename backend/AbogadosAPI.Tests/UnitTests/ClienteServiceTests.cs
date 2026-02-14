using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using AbogadosAPI.Data;
using AbogadosAPI.Models;
using AbogadosAPI.Services;
using AbogadosAPI.DTOs;

namespace AbogadosAPI.Tests.UnitTests;

/// <summary>
/// Tests unitarios simplificados para ClienteService
/// </summary>
public class ClienteServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        // Crear base de datos en memoria para cada test
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        var mockLogger = new Mock<ILogger<ClienteService>>();
        _service = new ClienteService(_context, mockLogger.Object);

        // Agregar datos de prueba
        _context.Clientes.AddRange(
            new Cliente
            {
                Id = 1,
                Nombre = "Juan",
                Apellidos = "Pérez",
                DniCif = "12345678A",
                Email = "juan@test.com",
                Telefono = "600111222",
                FechaAlta = DateTime.Now
            },
            new Cliente
            {
                Id = 2,
                Nombre = "María",
                Apellidos = "García",
                DniCif = "87654321B",
                Email = "maria@test.com",
                Telefono = "600333444",
                FechaAlta = DateTime.Now
            }
        );
        _context.SaveChanges();
    }

    /// <summary>
    /// TEST UNITARIO 1: Verificar que GetAllAsync devuelve todos los clientes
    /// </summary>
    [Fact]
    public async Task GetAllAsync_DebeRetornarTodosLosClientes()
    {
        // Act
        var resultado = await _service.GetAllAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First().Nombre.Should().Be("Juan");
        resultado.Last().Nombre.Should().Be("María");
    }

    /// <summary>
    /// TEST UNITARIO 2: Verificar que GetByIdAsync devuelve el cliente correcto
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ConIdValido_DebeRetornarCliente()
    {
        // Act
        var resultado = await _service.GetByIdAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(1);
        resultado.Nombre.Should().Be("Juan");
        resultado.DniCif.Should().Be("12345678A");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
