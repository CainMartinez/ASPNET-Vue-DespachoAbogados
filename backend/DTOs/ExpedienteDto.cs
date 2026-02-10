using System.ComponentModel.DataAnnotations;
using AbogadosAPI.Models;

namespace AbogadosAPI.DTOs;

/// <summary>
/// DTO para crear un nuevo expediente
/// </summary>
public class ExpedienteCreateDto
{
    /// <summary>
    /// Número único del expediente
    /// </summary>
    [Required(ErrorMessage = "El número de expediente es obligatorio")]
    [StringLength(50)]
    public string NumeroExpediente { get; set; } = string.Empty;

    /// <summary>
    /// Asunto principal del expediente
    /// </summary>
    [Required(ErrorMessage = "El asunto es obligatorio")]
    [StringLength(200)]
    public string Asunto { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del expediente
    /// </summary>
    [StringLength(1000)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Tipo o categoría del expediente (civil, penal, laboral, etc.)
    /// </summary>
    [Required(ErrorMessage = "El tipo de expediente es obligatorio")]
    [StringLength(100)]
    public string TipoExpediente { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del cliente asociado al expediente
    /// </summary>
    [Required(ErrorMessage = "El cliente es obligatorio")]
    public int ClienteId { get; set; }

    /// <summary>
    /// Juzgado o tribunal donde se tramita el expediente
    /// </summary>
    [StringLength(100)]
    public string? JuzgadoTribunal { get; set; }

    /// <summary>
    /// Número de procedimiento judicial asignado
    /// </summary>
    [StringLength(50)]
    public string? NumeroProcedimiento { get; set; }

    /// <summary>
    /// Fecha en que se abre el expediente
    /// </summary>
    public DateTime? FechaApertura { get; set; }

    /// <summary>
    /// Observaciones o notas adicionales sobre el expediente
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para actualizar un expediente existente
/// </summary>
public class ExpedienteUpdateDto
{
    /// <summary>
    /// Número único del expediente
    /// </summary>
    [Required(ErrorMessage = "El número de expediente es obligatorio")]
    [StringLength(50)]
    public string NumeroExpediente { get; set; } = string.Empty;

    /// <summary>
    /// Asunto principal del expediente
    /// </summary>
    [Required(ErrorMessage = "El asunto es obligatorio")]
    [StringLength(200)]
    public string Asunto { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del expediente
    /// </summary>
    [StringLength(1000)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Tipo o categoría del expediente (civil, penal, laboral, etc.)
    /// </summary>
    [Required(ErrorMessage = "El tipo de expediente es obligatorio")]
    [StringLength(100)]
    public string TipoExpediente { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del cliente asociado al expediente
    /// </summary>
    [Required(ErrorMessage = "El cliente es obligatorio")]
    public int ClienteId { get; set; }

    /// <summary>
    /// Juzgado o tribunal donde se tramita el expediente
    /// </summary>
    [StringLength(100)]
    public string? JuzgadoTribunal { get; set; }

    /// <summary>
    /// Número de procedimiento judicial asignado
    /// </summary>
    [StringLength(50)]
    public string? NumeroProcedimiento { get; set; }

    /// <summary>
    /// Fecha en que se cierra el expediente
    /// </summary>
    public DateTime? FechaCierre { get; set; }

    /// <summary>
    /// Observaciones o notas adicionales sobre el expediente
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para cambiar el estado de un expediente
/// </summary>
public class ExpedienteCambiarEstadoDto
{
    /// <summary>
    /// Nuevo estado del expediente (Activo, EnProceso, Cerrado, etc.)
    /// </summary>
    [Required(ErrorMessage = "El estado es obligatorio")]
    public Estado Estado { get; set; }

    /// <summary>
    /// Observaciones sobre el cambio de estado
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para leer un expediente
/// </summary>
public class ExpedienteDto
{
    /// <summary>
    /// Identificador único del expediente
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Número único del expediente
    /// </summary>
    public string NumeroExpediente { get; set; } = string.Empty;
    
    /// <summary>
    /// Asunto principal del expediente
    /// </summary>
    public string Asunto { get; set; } = string.Empty;
    
    /// <summary>
    /// Descripción detallada del expediente
    /// </summary>
    public string? Descripcion { get; set; }
    
    /// <summary>
    /// Tipo o categoría del expediente (civil, penal, laboral, etc.)
    /// </summary>
    public string TipoExpediente { get; set; } = string.Empty;
    
    /// <summary>
    /// Estado actual del expediente
    /// </summary>
    public Estado Estado { get; set; }
    
    /// <summary>
    /// Representación en texto del estado del expediente
    /// </summary>
    public string EstadoTexto => Estado.ToString();
    
    /// <summary>
    /// Identificador del cliente asociado al expediente
    /// </summary>
    public int ClienteId { get; set; }
    
    /// <summary>
    /// Nombre del cliente asociado al expediente
    /// </summary>
    public string? ClienteNombre { get; set; }
    
    /// <summary>
    /// Juzgado o tribunal donde se tramita el expediente
    /// </summary>
    public string? JuzgadoTribunal { get; set; }
    
    /// <summary>
    /// Número de procedimiento judicial asignado
    /// </summary>
    public string? NumeroProcedimiento { get; set; }
    
    /// <summary>
    /// Fecha en que se abrió el expediente
    /// </summary>
    public DateTime FechaApertura { get; set; }
    
    /// <summary>
    /// Fecha en que se cerró el expediente
    /// </summary>
    public DateTime? FechaCierre { get; set; }
    
    /// <summary>
    /// Fecha de la última modificación del expediente
    /// </summary>
    public DateTime? FechaModificacion { get; set; }
    
    /// <summary>
    /// Observaciones o notas adicionales sobre el expediente
    /// </summary>
    public string? Observaciones { get; set; }
    
    /// <summary>
    /// Número total de actuaciones registradas en el expediente
    /// </summary>
    public int TotalActuaciones { get; set; }
    
    /// <summary>
    /// Número total de citas asociadas al expediente
    /// </summary>
    public int TotalCitas { get; set; }
}

/// <summary>
/// DTO para listado resumido de expedientes
/// </summary>
public class ExpedienteResumenDto
{
    /// <summary>
    /// Identificador único del expediente
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Número único del expediente
    /// </summary>
    public string NumeroExpediente { get; set; } = string.Empty;
    
    /// <summary>
    /// Asunto principal del expediente
    /// </summary>
    public string Asunto { get; set; } = string.Empty;
    
    /// <summary>
    /// Tipo o categoría del expediente (civil, penal, laboral, etc.)
    /// </summary>
    public string TipoExpediente { get; set; } = string.Empty;
    
    /// <summary>
    /// Estado actual del expediente
    /// </summary>
    public Estado Estado { get; set; }
    
    /// <summary>
    /// Representación en texto del estado del expediente
    /// </summary>
    public string EstadoTexto => Estado.ToString();
    
    /// <summary>
    /// Nombre del cliente asociado al expediente
    /// </summary>
    public string? ClienteNombre { get; set; }
    
    /// <summary>
    /// Fecha en que se abrió el expediente
    /// </summary>
    public DateTime FechaApertura { get; set; }
}
