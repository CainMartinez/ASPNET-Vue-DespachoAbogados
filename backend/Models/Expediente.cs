using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbogadosAPI.Models;

/// <summary>
/// Representa un expediente jurídico
/// </summary>
/// <remarks>
/// Un expediente es un caso legal que el despacho gestiona para un cliente.
/// Puede ser de diferentes tipos: Civil, Penal, Laboral, Mercantil, Familia, etc.
/// </remarks>
public class Expediente
{
    /// <summary>
    /// Identificador único del expediente
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Número único identificador del expediente
    /// </summary>
    /// <example>EXP-2024-001</example>
    [Required(ErrorMessage = "El número de expediente es obligatorio")]
    [StringLength(50, ErrorMessage = "El número de expediente no puede exceder 50 caracteres")]
    public string NumeroExpediente { get; set; } = string.Empty;

    /// <summary>
    /// Título o asunto principal del expediente
    /// </summary>
    [Required(ErrorMessage = "El asunto es obligatorio")]
    [StringLength(200, ErrorMessage = "El asunto no puede exceder 200 caracteres")]
    public string Asunto { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del caso
    /// </summary>
    [StringLength(1000)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Tipo de expediente jurídico
    /// </summary>
    /// <example>Civil, Penal, Laboral, Mercantil, Familia</example>
    [Required(ErrorMessage = "El tipo de expediente es obligatorio")]
    [StringLength(100)]
    public string TipoExpediente { get; set; } = string.Empty;

    /// <summary>
    /// Estado actual del expediente
    /// </summary>
    [Required(ErrorMessage = "El estado es obligatorio")]
    public Estado Estado { get; set; } = Estado.Abierto;

    /// <summary>
    /// Identificador del cliente propietario del expediente
    /// </summary>
    [Required(ErrorMessage = "El cliente es obligatorio")]
    public int ClienteId { get; set; }

    /// <summary>
    /// Juzgado o tribunal donde se tramita
    /// </summary>
    [StringLength(100)]
    public string? JuzgadoTribunal { get; set; }

    /// <summary>
    /// Número de procedimiento judicial
    /// </summary>
    [StringLength(50)]
    public string? NumeroProcedimiento { get; set; }

    /// <summary>
    /// Fecha de apertura del expediente
    /// </summary>
    public DateTime FechaApertura { get; set; } = DateTime.Now;

    /// <summary>
    /// Fecha de cierre del expediente (si aplica)
    /// </summary>
    public DateTime? FechaCierre { get; set; }

    /// <summary>
    /// Fecha de última modificación del registro
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// Observaciones adicionales sobre el expediente
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }

    // Relaciones

    /// <summary>
    /// Cliente propietario del expediente
    /// </summary>
    [ForeignKey(nameof(ClienteId))]
    public Cliente Cliente { get; set; } = null!;

    /// <summary>
    /// Colección de actuaciones registradas en el expediente
    /// </summary>
    public ICollection<Actuacion> Actuaciones { get; set; } = new List<Actuacion>();

    /// <summary>
    /// Colección de citas asociadas al expediente
    /// </summary>
    public ICollection<Cita> Citas { get; set; } = new List<Cita>();

    /// <summary>
    /// Colección de documentos adjuntos al expediente
    /// </summary>
    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
}

/// <summary>
/// Estados posibles de un expediente
/// </summary>
/// <remarks>
/// Define el ciclo de vida de un expediente desde su apertura hasta su cierre
/// </remarks>
public enum Estado
{
    /// <summary>
    /// Expediente recién creado, pendiente de inicio de trámites
    /// </summary>
    Abierto = 1,

    /// <summary>
    /// Expediente con trámites activos en curso
    /// </summary>
    EnTramite = 2,

    /// <summary>
    /// Expediente temporalmente pausado
    /// </summary>
    Suspendido = 3,

    /// <summary>
    /// Expediente archivado sin resolución
    /// </summary>
    Archivado = 4,

    /// <summary>
    /// Expediente finalizado y cerrado
    /// </summary>
    Cerrado = 5
}
