using Microsoft.EntityFrameworkCore;
using AbogadosAPI.Data;
using AbogadosAPI.DTOs;
using AbogadosAPI.Models;

namespace AbogadosAPI.Services;

/// <summary>
/// Servicio para gestionar documentos
/// </summary>
/// <remarks>
/// Implementa la lógica de negocio para operaciones CRUD de documentos
/// </remarks>
public class DocumentoService : IDocumentoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DocumentoService> _logger;

    /// <summary>
    /// Constructor del servicio de documentos
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public DocumentoService(ApplicationDbContext context, ILogger<DocumentoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DocumentoDto>> GetAllAsync()
    {
        return await _context.Documentos
            .Include(d => d.Expediente)
            .OrderByDescending(d => d.FechaCarga)
            .Select(d => MapToDto(d))
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<DocumentoDto?> GetByIdAsync(int id)
    {
        var documento = await _context.Documentos
            .Include(d => d.Expediente)
            .FirstOrDefaultAsync(d => d.Id == id);

        return documento == null ? null : MapToDto(documento);
    }

    /// <inheritdoc/>
    public async Task<DocumentoDto> CreateAsync(DocumentoCreateDto documentoDto)
    {
        var documento = new Documento
        {
            ExpedienteId = documentoDto.ExpedienteId.HasValue && documentoDto.ExpedienteId.Value > 0
                ? documentoDto.ExpedienteId
                : null,
            NombreArchivo = documentoDto.NombreArchivo,
            Descripcion = documentoDto.Descripcion,
            TipoDocumento = documentoDto.TipoDocumento,
            RutaArchivo = documentoDto.RutaArchivo,
            TamanoBytes = documentoDto.TamanoBytes,
            Extension = documentoDto.Extension,
            CargadoPor = documentoDto.CargadoPor,
            Observaciones = documentoDto.Observaciones,
            FechaCarga = DateTime.Now
        };

        _context.Documentos.Add(documento);
        await _context.SaveChangesAsync();

        // Cargar relaciones si tiene expediente
        if (documento.ExpedienteId.HasValue)
            await _context.Entry(documento).Reference(d => d.Expediente).LoadAsync();

        return MapToDto(documento);
    }

    /// <inheritdoc/>
    public async Task<DocumentoDto?> UpdateAsync(int id, DocumentoUpdateDto documentoDto)
    {
        var documento = await _context.Documentos
            .Include(d => d.Expediente)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (documento == null) return null;

        documento.NombreArchivo = documentoDto.NombreArchivo;
        documento.Descripcion = documentoDto.Descripcion;
        documento.TipoDocumento = documentoDto.TipoDocumento;
        documento.Observaciones = documentoDto.Observaciones;
        documento.FechaModificacion = DateTime.Now;

        await _context.SaveChangesAsync();

        return MapToDto(documento);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var documento = await _context.Documentos.FindAsync(id);
        if (documento == null) return false;

        _context.Documentos.Remove(documento);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DocumentoDto>> GetByExpedienteIdAsync(int expedienteId)
    {
        var docs = await _context.Documentos
            .Include(d => d.Expediente)
            .Where(d => d.ExpedienteId == expedienteId)
            .OrderByDescending(d => d.FechaCarga)
            .ToListAsync();

        return docs.Select(d => MapToDto(d));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DocumentoDto>> GetByTipoAsync(string tipoDocumento)
    {
        var docs = await _context.Documentos
            .Include(d => d.Expediente)
            .Where(d => d.TipoDocumento.ToLower() == tipoDocumento.ToLower())
            .OrderByDescending(d => d.FechaCarga)
            .ToListAsync();

        return docs.Select(d => MapToDto(d));
    }

    /// <summary>
    /// Mapea una entidad Documento a un DTO
    /// </summary>
    /// <param name="documento">Entidad a mapear</param>
    /// <returns>DTO mapeado</returns>
    private static DocumentoDto MapToDto(Documento documento)
    {
        return new DocumentoDto
        {
            Id = documento.Id,
            ExpedienteId = documento.ExpedienteId,
            ExpedienteNumero = documento.Expediente?.NumeroExpediente,
            ExpedienteAsunto = documento.Expediente?.Asunto,
            NombreArchivo = documento.NombreArchivo,
            Descripcion = documento.Descripcion,
            TipoDocumento = documento.TipoDocumento,
            RutaArchivo = documento.RutaArchivo,
            TamanoBytes = documento.TamanoBytes,
            Extension = documento.Extension,
            FechaCarga = documento.FechaCarga,
            CargadoPor = documento.CargadoPor,
            FechaModificacion = documento.FechaModificacion,
            Observaciones = documento.Observaciones
        };
    }
}
