using AbogadosAPI.DTOs;

namespace AbogadosAPI.Services;

/// <summary>
/// Interfaz del servicio de documentos
/// </summary>
/// <remarks>
/// Define las operaciones disponibles para gestionar documentos adjuntos a expedientes
/// </remarks>
public interface IDocumentoService
{
    /// <summary>
    /// Obtiene todos los documentos
    /// </summary>
    /// <returns>Colección de documentos</returns>
    Task<IEnumerable<DocumentoDto>> GetAllAsync();

    /// <summary>
    /// Obtiene un documento por su identificador
    /// </summary>
    /// <param name="id">Identificador del documento</param>
    /// <returns>Documento si existe, null en caso contrario</returns>
    Task<DocumentoDto?> GetByIdAsync(int id);

    /// <summary>
    /// Crea un nuevo documento
    /// </summary>
    /// <param name="documentoDto">Datos del documento a crear</param>
    /// <returns>Documento creado</returns>
    Task<DocumentoDto> CreateAsync(DocumentoCreateDto documentoDto);

    /// <summary>
    /// Actualiza un documento existente
    /// </summary>
    /// <param name="id">Identificador del documento</param>
    /// <param name="documentoDto">Nuevos datos del documento</param>
    /// <returns>Documento actualizado si existe, null en caso contrario</returns>
    Task<DocumentoDto?> UpdateAsync(int id, DocumentoUpdateDto documentoDto);

    /// <summary>
    /// Elimina un documento
    /// </summary>
    /// <param name="id">Identificador del documento</param>
    /// <returns>True si se eliminó, false si no existe</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Obtiene todos los documentos de un expediente
    /// </summary>
    /// <param name="expedienteId">Identificador del expediente</param>
    /// <returns>Colección de documentos del expediente</returns>
    Task<IEnumerable<DocumentoDto>> GetByExpedienteIdAsync(int expedienteId);

    /// <summary>
    /// Obtiene documentos por tipo
    /// </summary>
    /// <param name="tipoDocumento">Tipo de documento a buscar</param>
    /// <returns>Colección de documentos del tipo especificado</returns>
    Task<IEnumerable<DocumentoDto>> GetByTipoAsync(string tipoDocumento);
}
