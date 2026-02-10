using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbogadosAPI.Models;

/// <summary>
/// Representa una cita o evento relacionado con un expediente
/// </summary>
/// <remarks>
/// Las citas gestionan eventos programados como vistas judiciales, reuniones con clientes, etc.
/// </remarks>
public class Cita
{
    /// <summary>
    /// Identificador único de la cita
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Identificador del expediente al que está asociada esta cita
    /// </summary>
    [Required(ErrorMessage = "El expediente es obligatorio")]
    public int ExpedienteId { get; set; }

    /// <summary>
    /// Título o asunto de la cita
    /// </summary>
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del propósito de la cita
    /// </summary>
    [StringLength(1000)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Fecha y hora de inicio de la cita
    /// </summary>
    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    public DateTime FechaInicio { get; set; }

    /// <summary>
    /// Fecha y hora de finalización de la cita
    /// </summary>
    [Required(ErrorMessage = "La fecha de fin es obligatoria")]
    public DateTime FechaFin { get; set; }

    /// <summary>
    /// Lugar físico donde se realizará la cita
    /// </summary>
    [StringLength(200)]
    public string? Lugar { get; set; }

    /// <summary>
    /// Tipo de cita (Vista, Reunión, Consulta, etc.)
    /// </summary>
    [Required(ErrorMessage = "El tipo de cita es obligatorio")]
    [StringLength(100)]
    public string TipoCita { get; set; } = string.Empty;

    /// <summary>
    /// Lista de participantes en la cita
    /// </summary>
    [StringLength(200)]
    public string? Participantes { get; set; }

    /// <summary>
    /// Indica si la cita ha sido completada
    /// </summary>
    public bool Completada { get; set; } = false;

    /// <summary>
    /// Observaciones adicionales sobre la cita
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }

    /// <summary>
    /// Fecha de creación de la cita en el sistema
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    /// <summary>
    /// Fecha de la última modificación de la cita
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// Expediente al que pertenece esta cita
    /// </summary>
    [ForeignKey(nameof(ExpedienteId))]
    public Expediente Expediente { get; set; } = null!;
}
