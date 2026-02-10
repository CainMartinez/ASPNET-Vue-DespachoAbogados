using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbogadosAPI.Models;

/// <summary>
/// Representa un cliente del despacho de abogados
/// </summary>
/// <remarks>
/// Un cliente puede ser una persona física o jurídica que contrata los servicios del despacho
/// </remarks>
public class Cliente
{
    /// <summary>
    /// Identificador único del cliente
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Nombre del cliente
    /// </summary>
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Apellidos del cliente (solo para personas físicas)
    /// </summary>
    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(150, ErrorMessage = "Los apellidos no pueden exceder 150 caracteres")]
    public string Apellidos { get; set; } = string.Empty;

    /// <summary>
    /// DNI para personas físicas o CIF para personas jurídicas
    /// </summary>
    [Required(ErrorMessage = "El DNI/CIF es obligatorio")]
    [StringLength(20, ErrorMessage = "El DNI/CIF no puede exceder 20 caracteres")]
    public string DniCif { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono de contacto del cliente
    /// </summary>
    [StringLength(15)]
    public string? Telefono { get; set; }

    /// <summary>
    /// Correo electrónico del cliente
    /// </summary>
    [StringLength(100)]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    public string? Email { get; set; }

    /// <summary>
    /// Dirección completa del cliente
    /// </summary>
    [StringLength(250)]
    public string? Direccion { get; set; }

    /// <summary>
    /// Ciudad de residencia del cliente
    /// </summary>
    [StringLength(100)]
    public string? Ciudad { get; set; }

    /// <summary>
    /// Código postal de la dirección
    /// </summary>
    [StringLength(10)]
    public string? CodigoPostal { get; set; }

    /// <summary>
    /// Observaciones adicionales sobre el cliente
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }

    /// <summary>
    /// Fecha de alta del cliente en el sistema
    /// </summary>
    public DateTime FechaAlta { get; set; } = DateTime.Now;

    /// <summary>
    /// Fecha de la última modificación de los datos del cliente
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// Colección de expedientes asociados al cliente
    /// </summary>
    public ICollection<Expediente> Expedientes { get; set; } = new List<Expediente>();
}
