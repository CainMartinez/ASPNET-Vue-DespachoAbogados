using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbogadosAPI.Models;

/// <summary>
/// Representa un documento adjunto a un expediente
/// </summary>
/// <remarks>
/// Los documentos pueden ser contratos, escritos judiciales, sentencias, 
/// pruebas documentales, informes periciales, etc.
/// </remarks>
public class Documento
{
    /// <summary>
    /// Identificador único del documento
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Identificador del expediente al que pertenece el documento (null para reportes generales)
    /// </summary>
    public int? ExpedienteId { get; set; }

    /// <summary>
    /// Nombre del archivo del documento
    /// </summary>
    [Required(ErrorMessage = "El nombre del archivo es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre del archivo no puede exceder 200 caracteres")]
    public string NombreArchivo { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del contenido del documento
    /// </summary>
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Tipo de documento
    /// </summary>
    /// <example>Contrato, Escrito, Sentencia, Prueba, Informe, etc.</example>
    [Required(ErrorMessage = "El tipo de documento es obligatorio")]
    [StringLength(100)]
    public string TipoDocumento { get; set; } = string.Empty;

    /// <summary>
    /// Ruta o URL donde se almacena el documento
    /// </summary>
    [Required(ErrorMessage = "La ruta del documento es obligatoria")]
    [StringLength(500)]
    public string RutaArchivo { get; set; } = string.Empty;

    /// <summary>
    /// Tamaño del archivo en bytes
    /// </summary>
    [Range(0, long.MaxValue)]
    public long TamanoBytes { get; set; }

    /// <summary>
    /// Extensión del archivo
    /// </summary>
    /// <example>.pdf, .doc, .docx, .jpg, etc.</example>
    [StringLength(10)]
    public string? Extension { get; set; }

    /// <summary>
    /// Fecha de carga del documento al sistema
    /// </summary>
    public DateTime FechaCarga { get; set; } = DateTime.Now;

    /// <summary>
    /// Usuario o persona que cargó el documento
    /// </summary>
    [StringLength(100)]
    public string? CargadoPor { get; set; }

    /// <summary>
    /// Fecha de última modificación del registro
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// Observaciones adicionales sobre el documento
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }

    // Relaciones

    /// <summary>
    /// Expediente al que pertenece este documento
    /// </summary>
    [ForeignKey(nameof(ExpedienteId))]
    public Expediente Expediente { get; set; } = null!;
}
