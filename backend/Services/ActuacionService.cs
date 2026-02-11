using Microsoft.EntityFrameworkCore;
using AbogadosAPI.Data;
using AbogadosAPI.DTOs;
using AbogadosAPI.Models;

namespace AbogadosAPI.Services;

/// <summary>
/// Servicio para gestionar actuaciones
/// </summary>
/// <remarks>
/// Implementa la lógica de negocio para operaciones CRUD de actuaciones procesales
/// </remarks>
public class ActuacionService : IActuacionService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ActuacionService> _logger;

    /// <summary>
    /// Constructor del servicio de actuaciones
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public ActuacionService(ApplicationDbContext context, ILogger<ActuacionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ActuacionDto>> GetAllAsync()
    {
        var actuaciones = await _context.Actuaciones
            .Include(a => a.Expediente)
            .OrderByDescending(a => a.FechaActuacion)
            .ToListAsync();

        return actuaciones.Select(MapToDto);
    }

    /// <inheritdoc/>
    public async Task<ActuacionDto?> GetByIdAsync(int id)
    {
        var actuacion = await _context.Actuaciones
            .Include(a => a.Expediente)
            .FirstOrDefaultAsync(a => a.Id == id);

        return actuacion == null ? null : MapToDto(actuacion);
    }

    /// <inheritdoc/>
    public async Task<ActuacionDto> CreateAsync(ActuacionCreateDto actuacionDto)
    {
        var actuacion = new Actuacion
        {
            ExpedienteId = actuacionDto.ExpedienteId,
            FechaActuacion = actuacionDto.FechaActuacion ?? DateTime.Now,
            TipoActuacion = actuacionDto.TipoActuacion,
            Descripcion = actuacionDto.Descripcion,
            Resultado = actuacionDto.Resultado,
            Responsable = actuacionDto.Responsable,
            Observaciones = actuacionDto.Observaciones,
            FechaRegistro = DateTime.Now
        };

        _context.Actuaciones.Add(actuacion);
        await _context.SaveChangesAsync();

        // Cargar relaciones
        await _context.Entry(actuacion).Reference(a => a.Expediente).LoadAsync();

        return MapToDto(actuacion);
    }

    /// <inheritdoc/>
    public async Task<ActuacionDto?> UpdateAsync(int id, ActuacionUpdateDto actuacionDto)
    {
        var actuacion = await _context.Actuaciones
            .Include(a => a.Expediente)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (actuacion == null) return null;

        actuacion.FechaActuacion = actuacionDto.FechaActuacion;
        actuacion.TipoActuacion = actuacionDto.TipoActuacion;
        actuacion.Descripcion = actuacionDto.Descripcion;
        actuacion.Resultado = actuacionDto.Resultado;
        actuacion.Responsable = actuacionDto.Responsable;
        actuacion.Observaciones = actuacionDto.Observaciones;
        actuacion.FechaModificacion = DateTime.Now;

        await _context.SaveChangesAsync();

        return MapToDto(actuacion);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var actuacion = await _context.Actuaciones.FindAsync(id);
        if (actuacion == null) return false;

        _context.Actuaciones.Remove(actuacion);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ActuacionDto>> GetByExpedienteIdAsync(int expedienteId)
    {
        var actuaciones = await _context.Actuaciones
            .Include(a => a.Expediente)
            .Where(a => a.ExpedienteId == expedienteId)
            .OrderByDescending(a => a.FechaActuacion)
            .ToListAsync();

        return actuaciones.Select(MapToDto);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ActuacionDto>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var actuaciones = await _context.Actuaciones
            .Include(a => a.Expediente)
            .Where(a => a.FechaActuacion >= fechaInicio && a.FechaActuacion <= fechaFin)
            .OrderByDescending(a => a.FechaActuacion)
            .ToListAsync();

        return actuaciones.Select(MapToDto);
    }

    /// <summary>
    /// Mapea una entidad Actuacion a un DTO
    /// </summary>
    /// <param name="actuacion">Entidad a mapear</param>
    /// <returns>DTO mapeado con toda la información de la actuación</returns>
    private static ActuacionDto MapToDto(Actuacion actuacion)
    {
        return new ActuacionDto
        {
            Id = actuacion.Id,
            ExpedienteId = actuacion.ExpedienteId,
            ExpedienteNumero = actuacion.Expediente.NumeroExpediente,
            ExpedienteAsunto = actuacion.Expediente.Asunto,
            FechaActuacion = actuacion.FechaActuacion,
            TipoActuacion = actuacion.TipoActuacion,
            Descripcion = actuacion.Descripcion,
            Resultado = actuacion.Resultado,
            Responsable = actuacion.Responsable,
            Observaciones = actuacion.Observaciones,
            FechaRegistro = actuacion.FechaRegistro,
            FechaModificacion = actuacion.FechaModificacion
        };
    }
}
