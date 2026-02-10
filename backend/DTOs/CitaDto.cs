using System.ComponentModel.DataAnnotations;

namespace AbogadosAPI.DTOs;

/// <summary>
/// DTO para crear una nueva cita
/// </summary>
public class CitaCreateDto
{
    /// <summary>
    /// Identificador del expediente al que pertenece la cita
    /// </summary>
    [Required(ErrorMessage = "El expediente es obligatorio")]
    public int ExpedienteId { get; set; }

    /// <summary>
    /// Título o nombre de la cita
    /// </summary>
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de la cita
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
    /// Lugar donde se realizará la cita
    /// </summary>
    [StringLength(200)]
    public string? Lugar { get; set; }

    /// <summary>
    /// Tipo o categoría de la cita (audiencia, reunión, consulta, etc.)
    /// </summary>
    [Required(ErrorMessage = "El tipo de cita es obligatorio")]
    [StringLength(100)]
    public string TipoCita { get; set; } = string.Empty;

    /// <summary>
    /// Participantes que asistirán a la cita
    /// </summary>
    [StringLength(200)]
    public string? Participantes { get; set; }

    /// <summary>
    /// Observaciones o notas adicionales sobre la cita
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para actualizar una cita existente
/// </summary>
public class CitaUpdateDto
{
    /// <summary>
    /// Título o nombre de la cita
    /// </summary>
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de la cita
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
    /// Lugar donde se realizará la cita
    /// </summary>
    [StringLength(200)]
    public string? Lugar { get; set; }

    /// <summary>
    /// Tipo o categoría de la cita (audiencia, reunión, consulta, etc.)
    /// </summary>
    [Required(ErrorMessage = "El tipo de cita es obligatorio")]
    [StringLength(100)]
    public string TipoCita { get; set; } = string.Empty;

    /// <summary>
    /// Participantes que asistirán a la cita
    /// </summary>
    [StringLength(200)]
    public string? Participantes { get; set; }

    /// <summary>
    /// Indica si la cita ha sido completada
    /// </summary>
    public bool Completada { get; set; }

    /// <summary>
    /// Observaciones o notas adicionales sobre la cita
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para leer una cita
/// </summary>
public class CitaDto
{
    /// <summary>
    /// Identificador único de la cita
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Identificador del expediente al que pertenece la cita
    /// </summary>
    public int ExpedienteId { get; set; }
    
    /// <summary>
    /// Número del expediente asociado
    /// </summary>
    public string? ExpedienteNumero { get; set; }
    
    /// <summary>
    /// Asunto del expediente asociado
    /// </summary>
    public string? ExpedienteAsunto { get; set; }
    
    /// <summary>
    /// Título o nombre de la cita
    /// </summary>
    public string Titulo { get; set; } = string.Empty;
    
    /// <summary>
    /// Descripción detallada de la cita
    /// </summary>
    public string? Descripcion { get; set; }
    
    /// <summary>
    /// Fecha y hora de inicio de la cita
    /// </summary>
    public DateTime FechaInicio { get; set; }
    
    /// <summary>
    /// Fecha y hora de finalización de la cita
    /// </summary>
    public DateTime FechaFin { get; set; }
    
    /// <summary>
    /// Lugar donde se realiza la cita
    /// </summary>
    public string? Lugar { get; set; }
    
    /// <summary>
    /// Tipo o categoría de la cita (audiencia, reunión, consulta, etc.)
    /// </summary>
    public string TipoCita { get; set; } = string.Empty;
    
    /// <summary>
    /// Participantes que asisten a la cita
    /// </summary>
    public string? Participantes { get; set; }
    
    /// <summary>
    /// Indica si la cita ha sido completada
    /// </summary>
    public bool Completada { get; set; }
    
    /// <summary>
    /// Observaciones o notas adicionales sobre la cita
    /// </summary>
    public string? Observaciones { get; set; }
    
    /// <summary>
    /// Fecha en que se creó la cita en el sistema
    /// </summary>
    public DateTime FechaCreacion { get; set; }
    
    /// <summary>
    /// Fecha de la última modificación de la cita
    /// </summary>
    public DateTime? FechaModificacion { get; set; }
}
