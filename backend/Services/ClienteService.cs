using Microsoft.EntityFrameworkCore;
using AbogadosAPI.Data;
using AbogadosAPI.DTOs;
using AbogadosAPI.Models;

namespace AbogadosAPI.Services;

/// <summary>
/// Servicio para gestionar clientes
/// </summary>
/// <remarks>
/// Implementa la lógica de negocio para operaciones CRUD de clientes del despacho
/// </remarks>
public class ClienteService : IClienteService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ClienteService> _logger;

    /// <summary>
    /// Constructor del servicio de clientes
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public ClienteService(ApplicationDbContext context, ILogger<ClienteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ClienteDto>> GetAllAsync()
    {
        return await _context.Clientes
            .Include(c => c.Expedientes)
            .Select(c => new ClienteDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellidos = c.Apellidos,
                DniCif = c.DniCif,
                Telefono = c.Telefono,
                Email = c.Email,
                Direccion = c.Direccion,
                Ciudad = c.Ciudad,
                CodigoPostal = c.CodigoPostal,
                Observaciones = c.Observaciones,
                FechaAlta = c.FechaAlta,
                FechaModificacion = c.FechaModificacion,
                TotalExpedientes = c.Expedientes.Count
            })
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Expedientes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null) return null;

        return new ClienteDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Apellidos = cliente.Apellidos,
            DniCif = cliente.DniCif,
            Telefono = cliente.Telefono,
            Email = cliente.Email,
            Direccion = cliente.Direccion,
            Ciudad = cliente.Ciudad,
            CodigoPostal = cliente.CodigoPostal,
            Observaciones = cliente.Observaciones,
            FechaAlta = cliente.FechaAlta,
            FechaModificacion = cliente.FechaModificacion,
            TotalExpedientes = cliente.Expedientes.Count
        };
    }

    /// <inheritdoc/>
    public async Task<ClienteDto> CreateAsync(ClienteCreateDto clienteDto)
    {
        var cliente = new Cliente
        {
            Nombre = clienteDto.Nombre,
            Apellidos = clienteDto.Apellidos,
            DniCif = clienteDto.DniCif,
            Telefono = clienteDto.Telefono,
            Email = clienteDto.Email,
            Direccion = clienteDto.Direccion,
            Ciudad = clienteDto.Ciudad,
            CodigoPostal = clienteDto.CodigoPostal,
            Observaciones = clienteDto.Observaciones,
            FechaAlta = DateTime.Now
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return new ClienteDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Apellidos = cliente.Apellidos,
            DniCif = cliente.DniCif,
            Telefono = cliente.Telefono,
            Email = cliente.Email,
            Direccion = cliente.Direccion,
            Ciudad = cliente.Ciudad,
            CodigoPostal = cliente.CodigoPostal,
            Observaciones = cliente.Observaciones,
            FechaAlta = cliente.FechaAlta,
            FechaModificacion = cliente.FechaModificacion,
            TotalExpedientes = 0
        };
    }

    /// <inheritdoc/>
    public async Task<ClienteDto?> UpdateAsync(int id, ClienteUpdateDto clienteDto)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Expedientes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null) return null;

        cliente.Nombre = clienteDto.Nombre;
        cliente.Apellidos = clienteDto.Apellidos;
        cliente.DniCif = clienteDto.DniCif;
        cliente.Telefono = clienteDto.Telefono;
        cliente.Email = clienteDto.Email;
        cliente.Direccion = clienteDto.Direccion;
        cliente.Ciudad = clienteDto.Ciudad;
        cliente.CodigoPostal = clienteDto.CodigoPostal;
        cliente.Observaciones = clienteDto.Observaciones;
        cliente.FechaModificacion = DateTime.Now;

        await _context.SaveChangesAsync();

        return new ClienteDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Apellidos = cliente.Apellidos,
            DniCif = cliente.DniCif,
            Telefono = cliente.Telefono,
            Email = cliente.Email,
            Direccion = cliente.Direccion,
            Ciudad = cliente.Ciudad,
            CodigoPostal = cliente.CodigoPostal,
            Observaciones = cliente.Observaciones,
            FechaAlta = cliente.FechaAlta,
            FechaModificacion = cliente.FechaModificacion,
            TotalExpedientes = cliente.Expedientes.Count
        };
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null) return false;

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ClienteDto>> SearchAsync(string searchTerm)
    {
        var query = _context.Clientes
            .Include(c => c.Expedientes)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(c =>
                c.Nombre.ToLower().Contains(searchTerm) ||
                c.Apellidos.ToLower().Contains(searchTerm) ||
                c.DniCif.ToLower().Contains(searchTerm) ||
                (c.Email != null && c.Email.ToLower().Contains(searchTerm)));
        }

        return await query.Select(c => new ClienteDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Apellidos = c.Apellidos,
            DniCif = c.DniCif,
            Telefono = c.Telefono,
            Email = c.Email,
            Direccion = c.Direccion,
            Ciudad = c.Ciudad,
            CodigoPostal = c.CodigoPostal,
            Observaciones = c.Observaciones,
            FechaAlta = c.FechaAlta,
            FechaModificacion = c.FechaModificacion,
            TotalExpedientes = c.Expedientes.Count
        }).ToListAsync();
    }
}
