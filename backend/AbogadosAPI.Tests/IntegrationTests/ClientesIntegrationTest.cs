using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using AbogadosAPI.DTOs;

namespace AbogadosAPI.Tests.IntegrationTests;

/// <summary>
/// Test de integración para el controlador de Clientes
/// </summary>
public class ClientesIntegrationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ClientesIntegrationTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// TEST DE INTEGRACIÓN: Verificar que GET /api/clientes retorna la lista de clientes
    /// </summary>
    [Fact]
    public async Task GET_Clientes_DebeRetornar200ConListaDeClientes()
    {
        // Act - Hacer petición HTTP GET a la API
        var response = await _client.GetAsync("/api/clientes");

        // Assert - Verificar que la respuesta es correcta
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var clientes = await response.Content.ReadFromJsonAsync<List<ClienteDto>>();
        clientes.Should().NotBeNull();
        clientes.Should().NotBeEmpty();
        
        // Verificar que el cliente de prueba existe
        var clienteTest = clientes!.FirstOrDefault(c => c.DniCif == "TEST123");
        clienteTest.Should().NotBeNull();
        clienteTest!.Nombre.Should().Be("Test");
    }
}
