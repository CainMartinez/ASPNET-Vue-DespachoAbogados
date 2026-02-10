using System.ComponentModel.DataAnnotations;

namespace AbogadosAPI.DTOs;

/// <summary>
/// DTO para crear un nuevo documento
/// </summary>
public class DocumentoCreateDto
{
    /// <summary>
    /// Identificador del expediente (opcional para reportes generales)
    /// </summary>
    public int? ExpedienteId { get; set; }

    /// <summary>
    /// Nombre del archivo
    /// </summary>
    [Required(ErrorMessage = "El nombre del archivo es obligatorio")]
    [StringLength(200)]
    public string NombreArchivo { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del documento
    /// </summary>
    [StringLength(500)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Tipo de documento
    /// </summary>
    [Required(ErrorMessage = "El tipo de documento es obligatorio")]
    [StringLength(100)]
    public string TipoDocumento { get; set; } = string.Empty;

    /// <summary>
    /// Ruta del archivo
    /// </summary>
    [Required(ErrorMessage = "La ruta del documento es obligatoria")]
    [StringLength(500)]
    public string RutaArchivo { get; set; } = string.Empty;

    /// <summary>
    /// Tamaño en bytes
    /// </summary>
    [Range(0, long.MaxValue)]
    public long TamanoBytes { get; set; }

    /// <summary>
    /// Extensión del archivo
    /// </summary>
    [StringLength(10)]
    public string? Extension { get; set; }

    /// <summary>
    /// Usuario que carga el documento
    /// </summary>
    [StringLength(100)]
    public string? CargadoPor { get; set; }

    /// <summary>
    /// Observaciones
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para actualizar un documento existente
/// </summary>
public class DocumentoUpdateDto
{
    /// <summary>
    /// Nombre del archivo
    /// </summary>
    [Required(ErrorMessage = "El nombre del archivo es obligatorio")]
    [StringLength(200)]
    public string NombreArchivo { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del documento
    /// </summary>
    [StringLength(500)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Tipo de documento
    /// </summary>
    [Required(ErrorMessage = "El tipo de documento es obligatorio")]
    [StringLength(100)]
    public string TipoDocumento { get; set; } = string.Empty;

    /// <summary>
    /// Observaciones
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para leer un documento
/// </summary>
public class DocumentoDto
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del expediente (null para reportes generales)
    /// </summary>
    public int? ExpedienteId { get; set; }

    /// <summary>
    /// Número del expediente
    /// </summary>
    public string? ExpedienteNumero { get; set; }

    /// <summary>
    /// Asunto del expediente
    /// </summary>
    public string? ExpedienteAsunto { get; set; }

    /// <summary>
    /// Nombre del archivo
    /// </summary>
    public string NombreArchivo { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del documento
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// Tipo de documento
    /// </summary>
    public string TipoDocumento { get; set; } = string.Empty;

    /// <summary>
    /// Ruta del archivo
    /// </summary>
    public string RutaArchivo { get; set; } = string.Empty;

    /// <summary>
    /// Tamaño en bytes
    /// </summary>
    public long TamanoBytes { get; set; }

    /// <summary>
    /// Tamaño formateado (KB, MB)
    /// </summary>
    public string TamanoFormateado => FormatearTamano(TamanoBytes);

    /// <summary>
    /// Extensión del archivo
    /// </summary>
    public string? Extension { get; set; }

    /// <summary>
    /// Fecha de carga
    /// </summary>
    public DateTime FechaCarga { get; set; }

    /// <summary>
    /// Usuario que cargó el documento
    /// </summary>
    public string? CargadoPor { get; set; }

    /// <summary>
    /// Fecha de última modificación
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// Observaciones
    /// </summary>
    public string? Observaciones { get; set; }

    /// <summary>
    /// Formatea el tamaño en bytes a una representación legible
    /// </summary>
    /// <param name="bytes">Tamaño en bytes</param>
    /// <returns>Tamaño formateado (ej: "1.5 MB")</returns>
    private static string FormatearTamano(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
