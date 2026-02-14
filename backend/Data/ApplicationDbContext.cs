using Microsoft.EntityFrameworkCore;
using AbogadosAPI.Models;

namespace AbogadosAPI.Data;

/// <summary>
/// Contexto de base de datos para el sistema de gestión de expedientes
/// </summary>
/// <remarks>
/// Proporciona acceso a todas las entidades del sistema y configura
/// las relaciones, índices y datos semilla
/// </remarks>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Constructor del contexto de base de datos
    /// </summary>
    /// <param name="options">Opciones de configuración del contexto</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Conjunto de entidades Cliente
    /// </summary>
    public DbSet<Cliente> Clientes { get; set; } = null!;

    /// <summary>
    /// Conjunto de entidades Expediente
    /// </summary>
    public DbSet<Expediente> Expedientes { get; set; } = null!;

    /// <summary>
    /// Conjunto de entidades Actuacion
    /// </summary>
    public DbSet<Actuacion> Actuaciones { get; set; } = null!;

    /// <summary>
    /// Conjunto de entidades Cita
    /// </summary>
    public DbSet<Cita> Citas { get; set; } = null!;

    /// <summary>
    /// Conjunto de entidades Documento
    /// </summary>
    public DbSet<Documento> Documentos { get; set; } = null!;

    /// <summary>
    /// Configura el modelo de datos, relaciones, índices y datos semilla
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasIndex(e => e.DniCif).IsUnique();
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => new { e.Nombre, e.Apellidos });

            // Datos semilla
            entity.HasData(
                new Cliente
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellidos = "García Martínez",
                    DniCif = "12345678A",
                    Telefono = "666111222",
                    Email = "juan.garcia@email.com",
                    Direccion = "Calle Mayor 15",
                    Ciudad = "Madrid",
                    CodigoPostal = "28001",
                    FechaAlta = new DateTime(2024, 1, 15)
                },
                new Cliente
                {
                    Id = 2,
                    Nombre = "María",
                    Apellidos = "López Fernández",
                    DniCif = "87654321B",
                    Telefono = "666333444",
                    Email = "maria.lopez@email.com",
                    Direccion = "Avenida Constitución 42",
                    Ciudad = "Barcelona",
                    CodigoPostal = "08001",
                    FechaAlta = new DateTime(2024, 2, 20)
                },
                new Cliente
                {
                    Id = 3,
                    Nombre = "Empresa Constructora",
                    Apellidos = "SL",
                    DniCif = "B12345678",
                    Telefono = "915555666",
                    Email = "contacto@constructora.com",
                    Direccion = "Polígono Industrial Norte, Nave 8",
                    Ciudad = "Valencia",
                    CodigoPostal = "46001",
                    FechaAlta = new DateTime(2024, 3, 10)
                },
                new Cliente
                {
                    Id = 4,
                    Nombre = "Carlos",
                    Apellidos = "Rodríguez Sánchez",
                    DniCif = "45678912C",
                    Telefono = "677888999",
                    Email = "carlos.rodriguez@email.com",
                    Direccion = "Plaza España 5, 3º A",
                    Ciudad = "Sevilla",
                    CodigoPostal = "41001",
                    FechaAlta = new DateTime(2024, 4, 5)
                }
            );
        });

        // Configuración de Expediente
        modelBuilder.Entity<Expediente>(entity =>
        {
            entity.HasIndex(e => e.NumeroExpediente).IsUnique();
            entity.HasIndex(e => e.Estado);
            entity.HasIndex(e => e.TipoExpediente);
            entity.HasIndex(e => e.ClienteId);

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Expedientes)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Datos semilla
            entity.HasData(
                new Expediente
                {
                    Id = 1,
                    NumeroExpediente = "EXP-2024-001",
                    Asunto = "Reclamación de cantidad por facturas impagadas",
                    Descripcion = "El cliente reclama el pago de servicios prestados a empresa deudora por importe de 15.000€",
                    TipoExpediente = "Civil",
                    Estado = Estado.EnTramite,
                    ClienteId = 1,
                    JuzgadoTribunal = "Juzgado de Primera Instancia nº 3 de Madrid",
                    NumeroProcedimiento = "123/2024",
                    FechaApertura = new DateTime(2024, 5, 1)
                },
                new Expediente
                {
                    Id = 2,
                    NumeroExpediente = "EXP-2024-002",
                    Asunto = "Despido improcedente",
                    Descripcion = "Reclamación por despido sin causa justificada",
                    TipoExpediente = "Laboral",
                    Estado = Estado.Abierto,
                    ClienteId = 2,
                    JuzgadoTribunal = "Juzgado de lo Social nº 1 de Barcelona",
                    NumeroProcedimiento = "456/2024",
                    FechaApertura = new DateTime(2024, 6, 10)
                },
                new Expediente
                {
                    Id = 3,
                    NumeroExpediente = "EXP-2024-003",
                    Asunto = "Incumplimiento contractual en obra",
                    Descripcion = "Defectos en ejecución de obra y retrasos en plazos de entrega",
                    TipoExpediente = "Mercantil",
                    Estado = Estado.EnTramite,
                    ClienteId = 3,
                    FechaApertura = new DateTime(2024, 7, 15)
                },
                new Expediente
                {
                    Id = 4,
                    NumeroExpediente = "EXP-2024-004",
                    Asunto = "Divorcio contencioso",
                    Descripcion = "Procedimiento de divorcio con desacuerdo en custodia y régimen de visitas",
                    TipoExpediente = "Familia",
                    Estado = Estado.Abierto,
                    ClienteId = 4,
                    JuzgadoTribunal = "Juzgado de Familia nº 2 de Sevilla",
                    NumeroProcedimiento = "789/2024",
                    FechaApertura = new DateTime(2024, 8, 20)
                },
                new Expediente
                {
                    Id = 5,
                    NumeroExpediente = "EXP-2023-025",
                    Asunto = "Reclamación de herencia",
                    Descripcion = "Procedimiento finalizado satisfactoriamente",
                    TipoExpediente = "Civil",
                    Estado = Estado.Cerrado,
                    ClienteId = 1,
                    FechaApertura = new DateTime(2023, 11, 5),
                    FechaCierre = new DateTime(2024, 4, 30)
                }
            );
        });

        // Configuración de Actuación
        modelBuilder.Entity<Actuacion>(entity =>
        {
            entity.HasIndex(e => e.ExpedienteId);
            entity.HasIndex(e => e.FechaActuacion);
            entity.HasIndex(e => e.TipoActuacion);

            entity.HasOne(a => a.Expediente)
                .WithMany(e => e.Actuaciones)
                .HasForeignKey(a => a.ExpedienteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Datos semilla
            entity.HasData(
                new Actuacion
                {
                    Id = 1,
                    ExpedienteId = 1,
                    FechaActuacion = new DateTime(2024, 5, 5),
                    TipoActuacion = "Reunión con cliente",
                    Descripcion = "Primera reunión con el cliente para recopilar documentación y analizar viabilidad de la reclamación",
                    Resultado = "Se recoge toda la documentación necesaria: facturas, contratos, emails",
                    Responsable = "Abg. Ana Pérez",
                    FechaRegistro = new DateTime(2024, 5, 5)
                },
                new Actuacion
                {
                    Id = 2,
                    ExpedienteId = 1,
                    FechaActuacion = new DateTime(2024, 5, 15),
                    TipoActuacion = "Escrito procesal",
                    Descripcion = "Redacción y presentación de demanda de juicio verbal",
                    Resultado = "Demanda presentada correctamente. Pendiente de admisión a trámite",
                    Responsable = "Abg. Ana Pérez",
                    FechaRegistro = new DateTime(2024, 5, 15)
                },
                new Actuacion
                {
                    Id = 3,
                    ExpedienteId = 2,
                    FechaActuacion = new DateTime(2024, 6, 12),
                    TipoActuacion = "Consulta telefónica",
                    Descripcion = "Explicación del procedimiento y requisitos para papeleta de conciliación",
                    Resultado = "Cliente informado. Se procede a preparar documentación",
                    Responsable = "Abg. Carlos Méndez",
                    FechaRegistro = new DateTime(2024, 6, 12)
                },
                new Actuacion
                {
                    Id = 4,
                    ExpedienteId = 3,
                    FechaActuacion = new DateTime(2024, 7, 20),
                    TipoActuacion = "Inspección in situ",
                    Descripcion = "Visita a la obra con perito para evaluar defectos constructivos",
                    Resultado = "Perito confirma graves deficiencias. Se elaborará informe técnico",
                    Responsable = "Abg. Isabel Torres",
                    FechaRegistro = new DateTime(2024, 7, 20)
                }
            );
        });

        // Configuración de Cita
        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasIndex(e => e.ExpedienteId);
            entity.HasIndex(e => e.FechaInicio);
            entity.HasIndex(e => e.TipoCita);
            entity.HasIndex(e => e.Completada);

            entity.HasOne(c => c.Expediente)
                .WithMany(e => e.Citas)
                .HasForeignKey(c => c.ExpedienteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Datos semilla
            entity.HasData(
                new Cita
                {
                    Id = 1,
                    ExpedienteId = 1,
                    Titulo = "Vista oral - Juicio verbal",
                    Descripcion = "Comparecencia ante el Juzgado para juicio verbal",
                    FechaInicio = new DateTime(2024, 11, 15, 10, 0, 0),
                    FechaFin = new DateTime(2024, 11, 15, 12, 0, 0),
                    Lugar = "Juzgado de Primera Instancia nº 3 de Madrid - Sala 2",
                    TipoCita = "Vista judicial",
                    Participantes = "Abg. Ana Pérez, Cliente Juan García",
                    Completada = false,
                    FechaCreacion = new DateTime(2024, 9, 1)
                },
                new Cita
                {
                    Id = 2,
                    ExpedienteId = 2,
                    Titulo = "Acto de conciliación",
                    Descripcion = "Intento de conciliación previo a demanda laboral",
                    FechaInicio = new DateTime(2024, 10, 20, 9, 30, 0),
                    FechaFin = new DateTime(2024, 10, 20, 11, 0, 0),
                    Lugar = "SMAC Barcelona - Sala 4",
                    TipoCita = "Conciliación",
                    Participantes = "Abg. Carlos Méndez, Cliente María López, Empresa demandada",
                    Completada = false,
                    FechaCreacion = new DateTime(2024, 8, 15)
                },
                new Cita
                {
                    Id = 3,
                    ExpedienteId = 4,
                    Titulo = "Reunión con cliente",
                    Descripcion = "Revisión de propuesta de convenio regulador",
                    FechaInicio = new DateTime(2024, 10, 5, 16, 0, 0),
                    FechaFin = new DateTime(2024, 10, 5, 17, 30, 0),
                    Lugar = "Despacho - Sala de reuniones",
                    TipoCita = "Reunión",
                    Participantes = "Abg. Luis Martín, Cliente Carlos Rodríguez",
                    Completada = false,
                    FechaCreacion = new DateTime(2024, 9, 20)
                },
                new Cita
                {
                    Id = 4,
                    ExpedienteId = 5,
                    Titulo = "Firma de escrituras",
                    Descripcion = "Firma de escrituras de adjudicación de herencia",
                    FechaInicio = new DateTime(2024, 4, 28, 11, 0, 0),
                    FechaFin = new DateTime(2024, 4, 28, 12, 0, 0),
                    Lugar = "Notaría García y Asociados - Madrid",
                    TipoCita = "Notaría",
                    Participantes = "Todos los herederos, Notario",
                    Completada = true,
                    FechaCreacion = new DateTime(2024, 4, 15)
                }
            );
        });
    }
}
