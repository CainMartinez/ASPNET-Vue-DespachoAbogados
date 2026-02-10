using System.ComponentModel.DataAnnotations;

namespace AbogadosAPI.DTOs;

/// <summary>
/// DTO para crear una nueva actuación
/// </summary>
public class ActuacionCreateDto
{
    /// <summary>
    /// Identificador del expediente al que pertenece la actuación
    /// </summary>
    [Required(ErrorMessage = "El expediente es obligatorio")]
    public int ExpedienteId { get; set; }

    /// <summary>
    /// Fecha en la que se realiza la actuación
    /// </summary>
    public DateTime? FechaActuacion { get; set; }

    /// <summary>
    /// Tipo o categoría de la actuación (audiencia, presentación, notificación, etc.)
    /// </summary>
    [Required(ErrorMessage = "El tipo de actuación es obligatorio")]
    [StringLength(100)]
    public string TipoActuacion { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de la actuación realizada
    /// </summary>
    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(2000)]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Resultado obtenido de la actuación
    /// </summary>
    [StringLength(500)]
    public string? Resultado { get; set; }

    /// <summary>
    /// Nombre del responsable de la actuación
    /// </summary>
    [StringLength(200)]
    public string? Responsable { get; set; }

    /// <summary>
    /// Observaciones o notas adicionales sobre la actuación
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para actualizar una actuación existente
/// </summary>
public class ActuacionUpdateDto
{
    /// <summary>
    /// Fecha en la que se realiza la actuación
    /// </summary>
    [Required(ErrorMessage = "La fecha de actuación es obligatoria")]
    public DateTime FechaActuacion { get; set; }

    /// <summary>
    /// Tipo o categoría de la actuación (audiencia, presentación, notificación, etc.)
    /// </summary>
    [Required(ErrorMessage = "El tipo de actuación es obligatorio")]
    [StringLength(100)]
    public string TipoActuacion { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de la actuación realizada
    /// </summary>
    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(2000)]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Resultado obtenido de la actuación
    /// </summary>
    [StringLength(500)]
    public string? Resultado { get; set; }

    /// <summary>
    /// Nombre del responsable de la actuación
    /// </summary>
    [StringLength(200)]
    public string? Responsable { get; set; }

    /// <summary>
    /// Observaciones o notas adicionales sobre la actuación
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para leer una actuación
/// </summary>
public class ActuacionDto
{
    /// <summary>
    /// Identificador único de la actuación
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Identificador del expediente al que pertenece la actuación
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
    /// Fecha en la que se realizó la actuación
    /// </summary>
    public DateTime FechaActuacion { get; set; }
    
    /// <summary>
    /// Tipo o categoría de la actuación (audiencia, presentación, notificación, etc.)
    /// </summary>
    public string TipoActuacion { get; set; } = string.Empty;
    
    /// <summary>
    /// Descripción detallada de la actuación realizada
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;
    
    /// <summary>
    /// Resultado obtenido de la actuación
    /// </summary>
    public string? Resultado { get; set; }
    
    /// <summary>
    /// Nombre del responsable de la actuación
    /// </summary>
    public string? Responsable { get; set; }
    
    /// <summary>
    /// Observaciones o notas adicionales sobre la actuación
    /// </summary>
    public string? Observaciones { get; set; }
    
    /// <summary>
    /// Fecha en que se registró la actuación en el sistema
    /// </summary>
    public DateTime FechaRegistro { get; set; }
    
    /// <summary>
    /// Fecha de la última modificación de la actuación
    /// </summary>
    public DateTime? FechaModificacion { get; set; }
}
