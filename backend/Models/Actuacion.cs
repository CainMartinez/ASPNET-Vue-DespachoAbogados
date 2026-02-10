using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbogadosAPI.Models;

/// <summary>
/// Representa una actuación o seguimiento dentro de un expediente
/// </summary>
/// <remarks>
/// Las actuaciones registran todas las acciones realizadas durante la tramitación de un expediente
/// </remarks>
public class Actuacion
{
    /// <summary>
    /// Identificador único de la actuación
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Identificador del expediente al que pertenece esta actuación
    /// </summary>
    [Required(ErrorMessage = "El expediente es obligatorio")]
    public int ExpedienteId { get; set; }

    /// <summary>
    /// Fecha en que se realizó la actuación
    /// </summary>
    [Required(ErrorMessage = "La fecha de actuación es obligatoria")]
    public DateTime FechaActuacion { get; set; } = DateTime.Now;

    /// <summary>
    /// Tipo de actuación realizada (Reunión, Escrito, Comparecencia, etc.)
    /// </summary>
    [Required(ErrorMessage = "El tipo de actuación es obligatorio")]
    [StringLength(100)]
    public string TipoActuacion { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de la actuación realizada
    /// </summary>
    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Resultado obtenido de la actuación
    /// </summary>
    [StringLength(500)]
    public string? Resultado { get; set; }

    /// <summary>
    /// Nombre del abogado o responsable de la actuación
    /// </summary>
    [StringLength(200)]
    public string? Responsable { get; set; }

    /// <summary>
    /// Observaciones adicionales sobre la actuación
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }

    /// <summary>
    /// Fecha de registro de la actuación en el sistema
    /// </summary>
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    /// <summary>
    /// Fecha de la última modificación de la actuación
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// Expediente al que pertenece esta actuación
    /// </summary>
    [ForeignKey(nameof(ExpedienteId))]
    public Expediente Expediente { get; set; } = null!;
}
