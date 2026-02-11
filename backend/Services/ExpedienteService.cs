using Microsoft.EntityFrameworkCore;
using AbogadosAPI.Data;
using AbogadosAPI.DTOs;
using AbogadosAPI.Models;

namespace AbogadosAPI.Services;

/// <summary>
/// Servicio para gestionar expedientes
/// </summary>
/// <remarks>
/// Implementa la lógica de negocio para operaciones CRUD de expedientes jurídicos
/// </remarks>
public class ExpedienteService : IExpedienteService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExpedienteService> _logger;

    /// <summary>
    /// Constructor del servicio de expedientes
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public ExpedienteService(ApplicationDbContext context, ILogger<ExpedienteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ExpedienteDto>> GetAllAsync()
    {
        return await _context.Expedientes
            .Include(e => e.Cliente)
            .Include(e => e.Actuaciones)
            .Include(e => e.Citas)
            .Select(e => MapToDto(e))
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ExpedienteResumenDto>> GetResumenAsync()
    {
        return await _context.Expedientes
            .Include(e => e.Cliente)
            .Select(e => new ExpedienteResumenDto
            {
                Id = e.Id,
                NumeroExpediente = e.NumeroExpediente,
                Asunto = e.Asunto,
                TipoExpediente = e.TipoExpediente,
                Estado = e.Estado,
                ClienteNombre = $"{e.Cliente.Nombre} {e.Cliente.Apellidos}",
                FechaApertura = e.FechaApertura
            })
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<ExpedienteDto?> GetByIdAsync(int id)
    {
        var expediente = await _context.Expedientes
            .Include(e => e.Cliente)
            .Include(e => e.Actuaciones)
            .Include(e => e.Citas)
            .FirstOrDefaultAsync(e => e.Id == id);

        return expediente == null ? null : MapToDto(expediente);
    }

    /// <inheritdoc/>
    public async Task<ExpedienteDto> CreateAsync(ExpedienteCreateDto expedienteDto)
    {
        var expediente = new Expediente
        {
            NumeroExpediente = expedienteDto.NumeroExpediente,
            Asunto = expedienteDto.Asunto,
            Descripcion = expedienteDto.Descripcion,
            TipoExpediente = expedienteDto.TipoExpediente,
            Estado = Estado.Abierto,
            ClienteId = expedienteDto.ClienteId,
            JuzgadoTribunal = expedienteDto.JuzgadoTribunal,
            NumeroProcedimiento = expedienteDto.NumeroProcedimiento,
            FechaApertura = expedienteDto.FechaApertura ?? DateTime.Now,
            Observaciones = expedienteDto.Observaciones
        };

        _context.Expedientes.Add(expediente);
        await _context.SaveChangesAsync();

        // Cargar relaciones
        await _context.Entry(expediente).Reference(e => e.Cliente).LoadAsync();

        return MapToDto(expediente);
    }

    /// <inheritdoc/>
    public async Task<ExpedienteDto?> UpdateAsync(int id, ExpedienteUpdateDto expedienteDto)
    {
        var expediente = await _context.Expedientes
            .Include(e => e.Cliente)
            .Include(e => e.Actuaciones)
            .Include(e => e.Citas)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expediente == null) return null;

        expediente.NumeroExpediente = expedienteDto.NumeroExpediente;
        expediente.Asunto = expedienteDto.Asunto;
        expediente.Descripcion = expedienteDto.Descripcion;
        expediente.TipoExpediente = expedienteDto.TipoExpediente;
        expediente.ClienteId = expedienteDto.ClienteId;
        expediente.JuzgadoTribunal = expedienteDto.JuzgadoTribunal;
        expediente.NumeroProcedimiento = expedienteDto.NumeroProcedimiento;
        expediente.FechaCierre = expedienteDto.FechaCierre;
        expediente.Observaciones = expedienteDto.Observaciones;
        expediente.FechaModificacion = DateTime.Now;

        await _context.SaveChangesAsync();

        return MapToDto(expediente);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var expediente = await _context.Expedientes.FindAsync(id);
        if (expediente == null) return false;

        _context.Expedientes.Remove(expediente);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<ExpedienteDto?> CambiarEstadoAsync(int id, ExpedienteCambiarEstadoDto estadoDto)
    {
        var expediente = await _context.Expedientes
            .Include(e => e.Cliente)
            .Include(e => e.Actuaciones)
            .Include(e => e.Citas)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expediente == null) return null;

        expediente.Estado = estadoDto.Estado;
        
        if (estadoDto.Estado == Estado.Cerrado || estadoDto.Estado == Estado.Archivado)
        {
            expediente.FechaCierre = DateTime.Now;
        }
        
        if (!string.IsNullOrWhiteSpace(estadoDto.Observaciones))
        {
            expediente.Observaciones = estadoDto.Observaciones;
        }

        expediente.FechaModificacion = DateTime.Now;

        await _context.SaveChangesAsync();

        return MapToDto(expediente);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ExpedienteDto>> GetByClienteIdAsync(int clienteId)
    {
        return await _context.Expedientes
            .Include(e => e.Cliente)
            .Include(e => e.Actuaciones)
            .Include(e => e.Citas)
            .Where(e => e.ClienteId == clienteId)
            .Select(e => MapToDto(e))
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ExpedienteDto>> GetByEstadoAsync(Estado estado)
    {
        return await _context.Expedientes
            .Include(e => e.Cliente)
            .Include(e => e.Actuaciones)
            .Include(e => e.Citas)
            .Where(e => e.Estado == estado)
            .Select(e => MapToDto(e))
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ExpedienteDto>> SearchAsync(string searchTerm)
    {
        var query = _context.Expedientes
            .Include(e => e.Cliente)
            .Include(e => e.Actuaciones)
            .Include(e => e.Citas)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(e =>
                e.NumeroExpediente.ToLower().Contains(searchTerm) ||
                e.Asunto.ToLower().Contains(searchTerm) ||
                e.TipoExpediente.ToLower().Contains(searchTerm) ||
                (e.NumeroProcedimiento != null && e.NumeroProcedimiento.ToLower().Contains(searchTerm)));
        }

        return await query.Select(e => MapToDto(e)).ToListAsync();
    }

    /// <summary>
    /// Mapea una entidad Expediente a un DTO
    /// </summary>
    /// <param name="expediente">Entidad a mapear</param>
    /// <returns>DTO mapeado con toda la información del expediente</returns>
    private static ExpedienteDto MapToDto(Expediente expediente)
    {
        return new ExpedienteDto
        {
            Id = expediente.Id,
            NumeroExpediente = expediente.NumeroExpediente,
            Asunto = expediente.Asunto,
            Descripcion = expediente.Descripcion,
            TipoExpediente = expediente.TipoExpediente,
            Estado = expediente.Estado,
            ClienteId = expediente.ClienteId,
            ClienteNombre = $"{expediente.Cliente.Nombre} {expediente.Cliente.Apellidos}",
            JuzgadoTribunal = expediente.JuzgadoTribunal,
            NumeroProcedimiento = expediente.NumeroProcedimiento,
            FechaApertura = expediente.FechaApertura,
            FechaCierre = expediente.FechaCierre,
            FechaModificacion = expediente.FechaModificacion,
            Observaciones = expediente.Observaciones,
            TotalActuaciones = expediente.Actuaciones?.Count ?? 0,
            TotalCitas = expediente.Citas?.Count ?? 0
        };
    }
}
