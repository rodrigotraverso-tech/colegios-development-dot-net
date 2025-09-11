using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.ValueObjects;
using ColegiosBackend.Core.ValueObjects.Permisos;
using Microsoft.EntityFrameworkCore;

namespace ColegiosBackend.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos principal para el sistema de colegios
/// Configura todas las entidades y sus relaciones usando Entity Framework Core
/// VERSIÓN FINAL CORREGIDA - Errores de navegación y auditoría resueltos
/// </summary>
public class ColegiosDbContext : DbContext
{
    public ColegiosDbContext(DbContextOptions<ColegiosDbContext> options) : base(options)
    {
    }

    // ==============================================
    // DBSETS - SOLO ENTIDADES EXISTENTES
    // ==============================================

    #region Entidades Principales

    /// <summary>
    /// Colegios registrados en el sistema
    /// </summary>
    public DbSet<Colegio> Colegios { get; set; } = null!;

    /// <summary>
    /// Personas (entidad global, sin multi-tenancy)
    /// </summary>
    public DbSet<Persona> Personas { get; set; } = null!;

    /// <summary>
    /// Usuarios del sistema
    /// </summary>
    public DbSet<Usuario> Usuarios { get; set; } = null!;

    /// <summary>
    /// Roles disponibles en el sistema
    /// </summary>
    public DbSet<Rol> Roles { get; set; } = null!;

    /// <summary>
    /// Asignación de roles a usuarios
    /// </summary>
    public DbSet<UsuarioRol> UsuarioRoles { get; set; } = null!;

    #endregion

    #region Entidades Académicas

    /// <summary>
    /// Estudiantes registrados
    /// </summary>
    public DbSet<Estudiante> Estudiantes { get; set; } = null!;

    /// <summary>
    /// Profesores registrados
    /// </summary>
    public DbSet<Profesor> Profesores { get; set; } = null!;

    /// <summary>
    /// Relaciones entre profesores y colegios
    /// </summary>
    public DbSet<ProfesorColegio> ProfesoresColegios { get; set; } = null!;

    /// <summary>
    /// Relaciones entre estudiantes y sus acudientes
    /// </summary>
    public DbSet<EstudianteAcudiente> EstudiantesAcudientes { get; set; } = null!;

    /// <summary>
    /// Años académicos
    /// </summary>
    public DbSet<AnoAcademico> AnosAcademicos { get; set; } = null!;

    /// <summary>
    /// Grados académicos
    /// </summary>
    public DbSet<Grado> Grados { get; set; } = null!;

    /// <summary>
    /// Grupos/secciones de estudiantes
    /// </summary>
    public DbSet<Grupo> Grupos { get; set; } = null!;

    /// <summary>
    /// Materias/asignaturas
    /// </summary>
    public DbSet<Materia> Materias { get; set; } = null!;

    /// <summary>
    /// Matrículas de estudiantes
    /// </summary>
    public DbSet<Matricula> Matriculas { get; set; } = null!;

    /// <summary>
    /// Asignaciones de profesores a materias y grupos
    /// </summary>
    public DbSet<Asignacion> Asignaciones { get; set; } = null!;

    #endregion

    #region Entidades de Evaluación

    /// <summary>
    /// Períodos evaluativos
    /// </summary>
    public DbSet<PeriodoEvaluativo> PeriodosEvaluativos { get; set; } = null!;

    /// <summary>
    /// Tipos de evaluación
    /// </summary>
    public DbSet<TipoEvaluacion> TiposEvaluacion { get; set; } = null!;

    /// <summary>
    /// Calificaciones de estudiantes
    /// </summary>
    public DbSet<Calificacion> Calificaciones { get; set; } = null!;

    /// <summary>
    /// Registros de asistencia
    /// </summary>
    public DbSet<Asistencia> Asistencias { get; set; } = null!;

    #endregion

    // ==============================================
    // CONFIGURACIÓN DEL MODELO
    // ==============================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==============================================
        // CONFIGURACIONES GLOBALES
        // ==============================================

        #region Configuraciones Globales

        // Configurar esquema por defecto
        modelBuilder.HasDefaultSchema("public");

        // Configurar convenciones de nombres
        ConfigurarConvencionesNombres(modelBuilder);

        // Configurar soft delete global
        ConfigurarSoftDeleteGlobal(modelBuilder);

        // Configurar auditoría global
        ConfigurarAuditoriaGlobal(modelBuilder);

        #endregion

        // ==============================================
        // APLICAR CONFIGURACIONES DE ENTIDADES
        // ==============================================

        #region Configuraciones de Entidades

        // Configuraciones principales
        ConfigurarEntidadesPrincipales(modelBuilder);
        ConfigurarEntidadesAcademicas(modelBuilder);
        ConfigurarEntidadesEvaluacion(modelBuilder);

        // Configurar relaciones complejas
        ConfigurarRelacionesComplejas(modelBuilder);

        // Configurar índices de rendimiento
        ConfigurarIndices(modelBuilder);

        // Configurar datos semilla (seed data)
        ConfigurarDatosSemilla(modelBuilder);

        #endregion
    }

    // ==============================================
    // MÉTODOS DE CONFIGURACIÓN ESPECÍFICOS
    // ==============================================

    #region Configuraciones Específicas

    /// <summary>
    /// Configura las convenciones de nombres para las tablas y columnas
    /// </summary>
    private static void ConfigurarConvencionesNombres(ModelBuilder modelBuilder)
    {
        // Configurar nombres de tablas en snake_case
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Convertir nombres de tabla a snake_case
            entity.SetTableName(ConvertirASnakeCase(entity.GetTableName() ?? entity.ClrType.Name));

            // Convertir nombres de columnas a snake_case
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ConvertirASnakeCase(property.Name));
            }

            // Convertir nombres de claves foráneas
            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(ConvertirASnakeCase(key.GetConstraintName() ?? ""));
            }
        }
    }

    /// <summary>
    /// Configura el soft delete global para todas las entidades que heredan de BaseEntity
    /// </summary>
    private static void ConfigurarSoftDeleteGlobal(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Agregar filtro global para soft delete
                var method = typeof(ColegiosDbContext)
                    .GetMethod(nameof(GetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    ?.MakeGenericMethod(entityType.ClrType);

                var filter = method?.Invoke(null, Array.Empty<object>());
                entityType.SetQueryFilter((System.Linq.Expressions.LambdaExpression?)filter);
            }
        }
    }

    /// <summary>
    /// Configura la auditoría global para entidades auditables
    /// </summary>
    private static void ConfigurarAuditoriaGlobal(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Configurar campos de auditoría como requeridos
                modelBuilder.Entity(entityType.ClrType)
                    .Property("CreatedAt")
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                modelBuilder.Entity(entityType.ClrType)
                    .Property("UpdatedAt")
                    .IsRequired(false);
            }
        }
    }

    /// <summary>
    /// Configura las entidades principales del sistema
    /// </summary>
    private static void ConfigurarEntidadesPrincipales(ModelBuilder modelBuilder)
    {
        // Configuración de Colegio
        modelBuilder.Entity<Colegio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Codigo).IsUnique();
            // Ignorar ValueObjects que no son entidades
            entity.Ignore(e => e.ContactInfo);
            entity.Ignore(e => e.Direccion);

            // Ignorar propiedades calculadas y navegaciones privadas
            entity.Ignore(e => e.Usuarios);
            entity.Ignore(e => e.Estudiantes);
            entity.Ignore(e => e.Profesores);
            entity.Ignore(e => e.AniosEscolares);
        });

        // Configuración de Persona
        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroDocumento).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.NumeroDocumento).IsUnique();
        });

        // Configuración de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            // Relación con Persona
            entity.HasOne(e => e.Persona)
                .WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(e => e.PersonaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de Rol
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Codigo).IsUnique();
            // Configurar PermisosJson como tipo JSONB
            entity.Property(e => e.PermisosJson)
                .HasColumnType("jsonb")
                .IsRequired();

            // Ignorar la propiedad calculada Permisos
            entity.Ignore(e => e.Permisos);
        });

        // Configuración de UsuarioRol
        modelBuilder.Entity<UsuarioRol>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Relaciones
            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.UsuarioRoles)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Rol)
                .WithMany(r => r.UsuarioRoles)
                .HasForeignKey(e => e.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Colegio)
                .WithMany()
                .HasForeignKey(e => e.ColegioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices únicos
            entity.HasIndex(e => new { e.UsuarioId, e.RolId, e.ColegioId }).IsUnique();
        });
        // Ignorar todos los ValueObjects del sistema
        modelBuilder.Ignore<ContactInfo>();
        modelBuilder.Ignore<Direccion>();
        modelBuilder.Ignore<PermisosRol>();
        modelBuilder.Ignore<ConfiguracionEspecialRol>();
        modelBuilder.Ignore<ModulosPermisos>();
        modelBuilder.Ignore<RestriccionesRol>();
    }

    /// <summary>
    /// Configura las entidades académicas
    /// </summary>
    private static void ConfigurarEntidadesAcademicas(ModelBuilder modelBuilder)
    {
        // Configuración de Estudiante
        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CodigoEstudiante).IsRequired().HasMaxLength(20);

            // Relaciones
            entity.HasOne(e => e.Persona)
                .WithMany(p => p.Estudiantes)
                .HasForeignKey(e => e.PersonaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Colegio)
                .WithMany()
                .HasForeignKey(e => e.ColegioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            entity.HasIndex(e => new { e.CodigoEstudiante, e.ColegioId }).IsUnique();
        });

        // Configuración de Profesor
        modelBuilder.Entity<Profesor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CodigoProfesor).IsRequired().HasMaxLength(20);

            // Relaciones
            entity.HasOne(e => e.Persona)
                .WithMany() // Profesor NO tiene navegación ProfesoresColegios, está en Persona
                .HasForeignKey(e => e.PersonaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Colegio)
                .WithMany()
                .HasForeignKey(e => e.ColegioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            entity.HasIndex(e => new { e.CodigoProfesor, e.ColegioId }).IsUnique();
        });

        // Configuración de ProfesorColegio
        modelBuilder.Entity<ProfesorColegio>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Relaciones - CORREGIDO: ProfesoresColegios está en Persona, no en Profesor
            entity.HasOne(e => e.Profesor)
                .WithMany() // Profesor no tiene esta colección
                .HasForeignKey(e => e.ProfesorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Colegio)
                .WithMany()
                .HasForeignKey(e => e.ColegioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices únicos
            entity.HasIndex(e => new { e.ProfesorId, e.ColegioId }).IsUnique();
        });

        // Configuración de EstudianteAcudiente
        modelBuilder.Entity<EstudianteAcudiente>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Relaciones
            entity.HasOne(e => e.Estudiante)
                .WithMany(est => est.Acudientes)
                .HasForeignKey(e => e.EstudianteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Acudiente)
                .WithMany(p => p.EstudiantesAcudidos)
                .HasForeignKey(e => e.AcudienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices únicos
            entity.HasIndex(e => new { e.EstudianteId, e.AcudienteId }).IsUnique();
        });

        // Configuración de AnoAcademico
        modelBuilder.Entity<AnoAcademico>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);

            // Relaciones
            entity.HasOne(e => e.Colegio)
                .WithMany()
                .HasForeignKey(e => e.ColegioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            entity.HasIndex(e => new { e.Codigo, e.ColegioId }).IsUnique();
        });

        // Configuración de Grado
        modelBuilder.Entity<Grado>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CodigoGrado).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NombreCorto).IsRequired().HasMaxLength(20);

            // Relaciones
            entity.HasOne(e => e.Colegio)
                .WithMany()
                .HasForeignKey(e => e.ColegioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            entity.HasIndex(e => new { e.CodigoGrado, e.ColegioId }).IsUnique();
        });

        // Configuración de Grupo
        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);

            // Relaciones
            entity.HasOne(e => e.Grado)
                .WithMany()
                .HasForeignKey(e => e.GradoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AnoAcademico)
                .WithMany()
                .HasForeignKey(e => e.AnoAcademicoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DirectorGrupo)
                .WithMany()
                .HasForeignKey(e => e.DirectorGrupoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices únicos
            entity.HasIndex(e => new { e.GradoId, e.AnoAcademicoId, e.Codigo }).IsUnique();
        });

        // Configuración de Materia
        modelBuilder.Entity<Materia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CodigoMateria).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NombreCorto).IsRequired().HasMaxLength(20);

            // Índices
            entity.HasIndex(e => new { e.CodigoMateria, e.ColegioId }).IsUnique();
        });

        // Configuración de Matricula
        modelBuilder.Entity<Matricula>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroMatricula).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PorcentajeBeca).HasPrecision(5, 2);

            // Relaciones
            entity.HasOne(e => e.Estudiante)
                .WithMany()
                .HasForeignKey(e => e.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Grupo)
                .WithMany()
                .HasForeignKey(e => e.GrupoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            entity.HasIndex(e => new { e.EstudianteId, e.GrupoId })
                .HasDatabaseName("idx_matricula_estudiante_grupo");
        });

        // Configuración de Asignacion
        modelBuilder.Entity<Asignacion>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Relaciones
            entity.HasOne(e => e.Profesor)
                .WithMany()
                .HasForeignKey(e => e.ProfesorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Grupo)
                .WithMany()
                .HasForeignKey(e => e.GrupoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Materia)
                .WithMany()
                .HasForeignKey(e => e.MateriaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AnoAcademico)
                .WithMany()
                .HasForeignKey(e => e.AnoAcademicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices únicos para evitar asignaciones duplicadas
            entity.HasIndex(e => new { e.ProfesorId, e.GrupoId, e.MateriaId, e.AnoAcademicoId }).IsUnique();
        });
    }

    /// <summary>
    /// Configura las entidades de evaluación
    /// </summary>
    private static void ConfigurarEntidadesEvaluacion(ModelBuilder modelBuilder)
    {
        // Configuración de PeriodoEvaluativo
        modelBuilder.Entity<PeriodoEvaluativo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);

            // Relaciones
            entity.HasOne(e => e.AnoAcademico)
                .WithMany()
                .HasForeignKey(e => e.AnoAcademicoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de TipoEvaluacion
        modelBuilder.Entity<TipoEvaluacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);

            // Índices
            entity.HasIndex(e => new { e.Codigo, e.ColegioId }).IsUnique();
        });

        // Configuración de Calificacion
        modelBuilder.Entity<Calificacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CalificacionValor).HasPrecision(5, 2);

            // Relaciones
            entity.HasOne(e => e.Estudiante)
                .WithMany()
                .HasForeignKey(e => e.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Asignacion)
                .WithMany()
                .HasForeignKey(e => e.AsignacionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PeriodoAcademico)
                .WithMany()
                .HasForeignKey(e => e.PeriodoAcademicoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TipoEvaluacion)
                .WithMany()
                .HasForeignKey(e => e.TipoEvaluacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices de rendimiento
            entity.HasIndex(e => new { e.EstudianteId, e.AsignacionId, e.PeriodoAcademicoId });
        });

        // Configuración de Asistencia
        modelBuilder.Entity<Asistencia>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Relaciones
            entity.HasOne(e => e.Estudiante)
                .WithMany()
                .HasForeignKey(e => e.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Materia)
                .WithMany()
                .HasForeignKey(e => e.MateriaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Grupo)
                .WithMany()
                .HasForeignKey(e => e.GrupoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AnoAcademico)
                .WithMany()
                .HasForeignKey(e => e.AnoAcademicoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PeriodoEvaluativo)
                .WithMany()
                .HasForeignKey(e => e.PeriodoEvaluativoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Profesor)
                .WithMany()
                .HasForeignKey(e => e.ProfesorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices únicos para evitar duplicados
            entity.HasIndex(e => new { e.EstudianteId, e.FechaClase, e.MateriaId, e.GrupoId }).IsUnique();
        });
    }

    /// <summary>
    /// Configura relaciones complejas entre entidades
    /// </summary>
    private static void ConfigurarRelacionesComplejas(ModelBuilder modelBuilder)
    {
        // Configurar relaciones many-to-many adicionales si es necesario
        // Por ejemplo: Estudiante-Materias optativas, etc.
    }

    /// <summary>
    /// Configura índices para mejorar el rendimiento
    /// </summary>
    private static void ConfigurarIndices(ModelBuilder modelBuilder)
    {
        // Índices adicionales para consultas frecuentes
        modelBuilder.Entity<Estudiante>()
            .HasIndex(e => new { e.ColegioId, e.Estado })
            .HasDatabaseName("idx_estudiante_colegio_estado");

        modelBuilder.Entity<Calificacion>()
            .HasIndex(e => new { e.EstudianteId, e.PeriodoAcademicoId })
            .HasDatabaseName("idx_calificacion_estudiante_periodo");

        modelBuilder.Entity<Asistencia>()
            .HasIndex(e => new { e.EstudianteId, e.FechaClase })
            .HasDatabaseName("idx_asistencia_estudiante_fecha");
    }

    /// <summary>
    /// Configura datos semilla para el sistema
    /// </summary>
    private static void ConfigurarDatosSemilla(ModelBuilder modelBuilder)
    {
        // TODO: Agregar datos semilla básicos
        // Por ejemplo: roles por defecto, tipos de documento, etc.
    }

    #endregion

    // ==============================================
    // MÉTODOS AUXILIARES
    // ==============================================

    #region Métodos Auxiliares

    /// <summary>
    /// Filtro para soft delete
    /// </summary>
    private static System.Linq.Expressions.Expression<Func<T, bool>> GetSoftDeleteFilter<T>() where T : BaseEntity
    {
        return entity => !entity.IsDeleted;
    }

    /// <summary>
    /// Convierte un string a snake_case
    /// </summary>
    private static string ConvertirASnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = string.Empty;
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) && i > 0)
                result += "_";
            result += char.ToLower(input[i]);
        }
        return result;
    }

    #endregion

    // ==============================================
    // OVERRIDE DE MÉTODOS BASE
    // ==============================================

    #region Override de Métodos

    /// <summary>
    /// Override de SaveChanges para manejar auditoría automática
    /// </summary>
    public override int SaveChanges()
    {
        ProcesarAuditoriaAntesDeSalvar();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override de SaveChangesAsync para manejar auditoría automática
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ProcesarAuditoriaAntesDeSalvar();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Procesa la auditoría automática antes de guardar cambios
    /// CORREGIDO: No modifica CreatedAt directamente, usa MarkAsUpdated()
    /// </summary>
    private void ProcesarAuditoriaAntesDeSalvar()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                // Solo marcar como actualizado, CreatedAt ya se establece en el constructor de BaseEntity
                entry.Entity.MarkAsUpdated();
            }
            // No necesitamos hacer nada para Added porque BaseEntity ya establece CreatedAt en el constructor
        }
    }
    #endregion
}