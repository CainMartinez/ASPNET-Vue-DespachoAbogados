using System.ComponentModel.DataAnnotations;

namespace AbogadosAPI.DTOs;

/// <summary>
/// DTO para crear un nuevo cliente
/// </summary>
/// <remarks>
/// Contiene los datos necesarios para dar de alta un cliente en el sistema
/// </remarks>
public class ClienteCreateDto
{
    /// <summary>
    /// Nombre del cliente
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Apellidos del cliente
    /// </summary>
    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(150)]
    public string Apellidos { get; set; } = string.Empty;

    /// <summary>
    /// DNI o CIF del cliente
    /// </summary>
    [Required(ErrorMessage = "El DNI/CIF es obligatorio")]
    [StringLength(20)]
    public string DniCif { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    [StringLength(15)]
    public string? Telefono { get; set; }

    /// <summary>
    /// Correo electrónico
    /// </summary>
    [StringLength(100)]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    public string? Email { get; set; }

    /// <summary>
    /// Dirección completa
    /// </summary>
    [StringLength(250)]
    public string? Direccion { get; set; }

    /// <summary>
    /// Ciudad de residencia
    /// </summary>
    [StringLength(100)]
    public string? Ciudad { get; set; }

    /// <summary>
    /// Código postal
    /// </summary>
    [StringLength(10)]
    public string? CodigoPostal { get; set; }

    /// <summary>
    /// Observaciones adicionales
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para actualizar un cliente existente
/// </summary>
/// <remarks>
/// Contiene los datos modificables de un cliente
/// </remarks>
public class ClienteUpdateDto
{
    /// <summary>
    /// Nombre del cliente
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Apellidos del cliente
    /// </summary>
    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(150)]
    public string Apellidos { get; set; } = string.Empty;

    /// <summary>
    /// DNI o CIF del cliente
    /// </summary>
    [Required(ErrorMessage = "El DNI/CIF es obligatorio")]
    [StringLength(20)]
    public string DniCif { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    [StringLength(15)]
    public string? Telefono { get; set; }

    /// <summary>
    /// Correo electrónico
    /// </summary>
    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Dirección completa
    /// </summary>
    [StringLength(250)]
    public string? Direccion { get; set; }

    /// <summary>
    /// Ciudad de residencia
    /// </summary>
    [StringLength(100)]
    public string? Ciudad { get; set; }

    /// <summary>
    /// Código postal
    /// </summary>
    [StringLength(10)]
    public string? CodigoPostal { get; set; }

    /// <summary>
    /// Observaciones adicionales
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para leer un cliente
/// </summary>
/// <remarks>
/// Representa la información completa de un cliente incluyendo datos calculados
/// </remarks>
public class ClienteDto
{
    /// <summary>
    /// Identificador único del cliente
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Nombre del cliente
    /// </summary>
    public string Nombre { get; set; } = string.Empty;
    
    /// <summary>
    /// Apellidos del cliente
    /// </summary>
    public string Apellidos { get; set; } = string.Empty;
    
    /// <summary>
    /// Nombre completo calculado (nombre + apellidos)
    /// </summary>
    public string NombreCompleto => $"{Nombre} {Apellidos}";
    
    /// <summary>
    /// DNI o CIF del cliente
    /// </summary>
    public string DniCif { get; set; } = string.Empty;
    
    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    public string? Telefono { get; set; }
    
    /// <summary>
    /// Correo electrónico
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Dirección completa
    /// </summary>
    public string? Direccion { get; set; }
    
    /// <summary>
    /// Ciudad de residencia
    /// </summary>
    public string? Ciudad { get; set; }
    
    /// <summary>
    /// Código postal
    /// </summary>
    public string? CodigoPostal { get; set; }
    
    /// <summary>
    /// Observaciones adicionales
    /// </summary>
    public string? Observaciones { get; set; }
    
    /// <summary>
    /// Fecha de alta en el sistema
    /// </summary>
    public DateTime FechaAlta { get; set; }
    
    /// <summary>
    /// Fecha de última modificación
    /// </summary>
    public DateTime? FechaModificacion { get; set; }
    
    /// <summary>
    /// Número total de expedientes asociados al cliente
    /// </summary>
    public int TotalExpedientes { get; set; }
}
