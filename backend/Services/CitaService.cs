using Microsoft.EntityFrameworkCore;
using AbogadosAPI.Data;
using AbogadosAPI.DTOs;
using AbogadosAPI.Models;

namespace AbogadosAPI.Services;

/// <summary>
/// Servicio para gestionar citas
/// </summary>
/// <remarks>
/// Implementa la lógica de negocio para operaciones CRUD de citas y reuniones
/// </remarks>
public class CitaService : ICitaService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CitaService> _logger;

    /// <summary>
    /// Constructor del servicio de citas
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public CitaService(ApplicationDbContext context, ILogger<CitaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CitaDto>> GetAllAsync()
    {
        return await _context.Citas
            .Include(c => c.Expediente)
            .OrderBy(c => c.FechaInicio)
            .Select(c => MapToDto(c))
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<CitaDto?> GetByIdAsync(int id)
    {
        var cita = await _context.Citas
            .Include(c => c.Expediente)
            .FirstOrDefaultAsync(c => c.Id == id);

        return cita == null ? null : MapToDto(cita);
    }

    /// <inheritdoc/>
    public async Task<CitaDto> CreateAsync(CitaCreateDto citaDto)
    {
        var cita = new Cita
        {
            ExpedienteId = citaDto.ExpedienteId,
            Titulo = citaDto.Titulo,
            Descripcion = citaDto.Descripcion,
            FechaInicio = citaDto.FechaInicio,
            FechaFin = citaDto.FechaFin,
            Lugar = citaDto.Lugar,
            TipoCita = citaDto.TipoCita,
            Participantes = citaDto.Participantes,
            Observaciones = citaDto.Observaciones,
            Completada = false,
            FechaCreacion = DateTime.Now
        };

        _context.Citas.Add(cita);
        await _context.SaveChangesAsync();

        // Cargar relaciones
        await _context.Entry(cita).Reference(c => c.Expediente).LoadAsync();

        return MapToDto(cita);
    }

    /// <inheritdoc/>
    public async Task<CitaDto?> UpdateAsync(int id, CitaUpdateDto citaDto)
    {
        var cita = await _context.Citas
            .Include(c => c.Expediente)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cita == null) return null;

        cita.Titulo = citaDto.Titulo;
        cita.Descripcion = citaDto.Descripcion;
        cita.FechaInicio = citaDto.FechaInicio;
        cita.FechaFin = citaDto.FechaFin;
        cita.Lugar = citaDto.Lugar;
        cita.TipoCita = citaDto.TipoCita;
        cita.Participantes = citaDto.Participantes;
        cita.Completada = citaDto.Completada;
        cita.Observaciones = citaDto.Observaciones;
        cita.FechaModificacion = DateTime.Now;

        await _context.SaveChangesAsync();

        return MapToDto(cita);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var cita = await _context.Citas.FindAsync(id);
        if (cita == null) return false;

        _context.Citas.Remove(cita);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<CitaDto?> MarcarCompletadaAsync(int id, bool completada)
    {
        var cita = await _context.Citas
            .Include(c => c.Expediente)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cita == null) return null;

        cita.Completada = completada;
        cita.FechaModificacion = DateTime.Now;

        await _context.SaveChangesAsync();

        return MapToDto(cita);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CitaDto>> GetByExpedienteIdAsync(int expedienteId)
    {
        var citas = await _context.Citas
            .Include(c => c.Expediente)
            .Where(c => c.ExpedienteId == expedienteId)
            .OrderBy(c => c.FechaInicio)
            .ToListAsync();

        return citas.Select(c => MapToDto(c));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CitaDto>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var citas = await _context.Citas
            .Include(c => c.Expediente)
            .Where(c => c.FechaInicio >= fechaInicio && c.FechaInicio <= fechaFin)
            .OrderBy(c => c.FechaInicio)
            .ToListAsync();

        return citas.Select(c => MapToDto(c));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CitaDto>> GetPendientesAsync()
    {
        var citas = await _context.Citas
            .Include(c => c.Expediente)
            .Where(c => !c.Completada && c.FechaInicio >= DateTime.Now)
            .OrderBy(c => c.FechaInicio)
            .ToListAsync();

        return citas.Select(c => MapToDto(c));
    }

    /// <summary>
    /// Mapea una entidad Cita a un DTO
    /// </summary>
    /// <param name="cita">Entidad a mapear</param>
    /// <returns>DTO mapeado con toda la información de la cita</returns>
    private static CitaDto MapToDto(Cita cita)
    {
        return new CitaDto
        {
            Id = cita.Id,
            ExpedienteId = cita.ExpedienteId,
            ExpedienteNumero = cita.Expediente.NumeroExpediente,
            ExpedienteAsunto = cita.Expediente.Asunto,
            Titulo = cita.Titulo,
            Descripcion = cita.Descripcion,
            FechaInicio = cita.FechaInicio,
            FechaFin = cita.FechaFin,
            Lugar = cita.Lugar,
            TipoCita = cita.TipoCita,
            Participantes = cita.Participantes,
            Completada = cita.Completada,
            Observaciones = cita.Observaciones,
            FechaCreacion = cita.FechaCreacion,
            FechaModificacion = cita.FechaModificacion
        };
    }
}
