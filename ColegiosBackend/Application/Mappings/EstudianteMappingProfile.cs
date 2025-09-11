using AutoMapper;
using ColegiosBackend.Application.DTOs.Estudiante;
using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Application.Mappings;

/// <summary>
/// Profile de AutoMapper para las entidades y DTOs relacionados con Estudiantes
/// Configura los mapeos bidireccionales entre entidades de dominio y DTOs de aplicación
/// </summary>
public class EstudianteMappingProfile : Profile
{
    public EstudianteMappingProfile()
    {
        // ==============================================
        // MAPEOS PRINCIPALES DE ESTUDIANTE
        // ==============================================

        #region Mapeos Estudiante -> DTOs

        // Mapeo de Estudiante a EstudianteDto (DTO básico)
        CreateMap<Estudiante, EstudianteDto>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.NombreCompleto : string.Empty))
            .ForMember(dest => dest.Nombres, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.Nombres : string.Empty))
            .ForMember(dest => dest.Apellidos, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.Apellidos : string.Empty))
            .ForMember(dest => dest.NumeroDocumento, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.NumeroDocumento : string.Empty))
            .ForMember(dest => dest.FechaNacimiento, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.FechaNacimiento : null))
            .ForMember(dest => dest.Genero, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.Genero : null))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.Email : null))
            .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.Telefono : null))
            .ForMember(dest => dest.Celular, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.Celular : null))
            .ForMember(dest => dest.FotoUrl, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.FotoUrl : null))
            .ForMember(dest => dest.NombreColegio, opt => opt.MapFrom(src =>
                src.Colegio != null ? src.Colegio.Nombre : string.Empty))
            .ForMember(dest => dest.CodigoColegio, opt => opt.MapFrom(src =>
                src.Colegio != null ? src.Colegio.Codigo : string.Empty))
            .ForMember(dest => dest.GrupoActual, opt => opt.Ignore()) // Se mapea manualmente en el servicio
            .ForMember(dest => dest.GradoActual, opt => opt.Ignore()) // Se mapea manualmente en el servicio
            .ForMember(dest => dest.JornadaActual, opt => opt.Ignore()); // Se mapea manualmente en el servicio

        // Mapeo de Estudiante a EstudianteDetalleDto (DTO detallado)
        CreateMap<Estudiante, EstudianteDetalleDto>()
            .IncludeBase<Estudiante, EstudianteDto>() // Incluye el mapeo base
            .ForMember(dest => dest.SegundoNombre, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.SegundoNombre : null))
            .ForMember(dest => dest.SegundoApellido, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.SegundoApellido : null))
            .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.Direccion : null))
            .ForMember(dest => dest.TipoDocumentoId, opt => opt.MapFrom(src =>
                src.Persona != null ? src.Persona.TipoDocumentoId : 0))
            .ForMember(dest => dest.TipoDocumentoNombre, opt => opt.Ignore()) // Se mapea manualmente en el servicio
            .ForMember(dest => dest.Acudientes, opt => opt.MapFrom(src =>
                src.Acudientes.Where(a => a.EstaActiva)))
            .ForMember(dest => dest.HistorialMatriculas, opt => opt.Ignore()) // Se mapea manualmente en el servicio
            .ForMember(dest => dest.EstadisticasActuales, opt => opt.Ignore()); // Se mapea manualmente en el servicio

        #endregion

        #region Mapeos DTOs -> Estudiante

        // Mapeo de CrearEstudianteDto a Estudiante
        CreateMap<CrearEstudianteDto, Estudiante>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Se genera automáticamente
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => EstadoEstudiante.Activo))
            .ForMember(dest => dest.FechaEgreso, opt => opt.Ignore())
            .ForMember(dest => dest.MotivoEgreso, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Se establece en BaseEntity
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Colegio, opt => opt.Ignore()) // Navegación
            .ForMember(dest => dest.Persona, opt => opt.Ignore()) // Navegación
            .ForMember(dest => dest.Acudientes, opt => opt.Ignore()); // Navegación

        #endregion

        // ==============================================
        // MAPEOS DE ENTIDADES RELACIONADAS
        // ==============================================

        #region Mapeos EstudianteAcudiente

        // Mapeo de EstudianteAcudiente a EstudianteAcudienteDto
        CreateMap<EstudianteAcudiente, EstudianteAcudienteDto>()
            .ForMember(dest => dest.PersonaId, opt => opt.MapFrom(src => src.AcudienteId))
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src =>
                src.Acudiente != null ? src.Acudiente.NombreCompleto : string.Empty))
            .ForMember(dest => dest.NumeroDocumento, opt => opt.MapFrom(src =>
                src.Acudiente != null ? src.Acudiente.NumeroDocumento : string.Empty))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src =>
                src.Acudiente != null ? src.Acudiente.Email : null))
            .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src =>
                src.Acudiente != null ? src.Acudiente.Telefono : null))
            .ForMember(dest => dest.Celular, opt => opt.MapFrom(src =>
                src.Acudiente != null ? src.Acudiente.Celular : null))
            .ForMember(dest => dest.TipoRelacion, opt => opt.MapFrom(src => src.TipoRelacion))
            .ForMember(dest => dest.TipoRelacionNombre, opt => opt.MapFrom(src =>
                GetTipoRelacionNombre(src.TipoRelacion)))
            .ForMember(dest => dest.EsContactoPrincipal, opt => opt.MapFrom(src => src.EsPrincipal))
            .ForMember(dest => dest.EsAutorizadoRecoger, opt => opt.MapFrom(src => src.PuedeRetirar))
            .ForMember(dest => dest.RecibeNotificaciones, opt => opt.MapFrom(src =>
                src.RecibirNotificacionesAcademicas || src.RecibirNotificacionesDisciplinarias || src.RecibirNotificacionesFinancieras))
            .ForMember(dest => dest.Ocupacion, opt => opt.Ignore()) // No existe en la entidad, se mapea manualmente en el servicio
            .ForMember(dest => dest.LugarTrabajo, opt => opt.Ignore()); // No existe en la entidad, se mapea manualmente en el servicio

        #endregion

        #region Mapeos Matricula

        // Mapeo de Matricula a MatriculaEstudianteDto
        CreateMap<Matricula, MatriculaEstudianteDto>()
            .ForMember(dest => dest.AnoAcademicoId, opt => opt.MapFrom(src => src.Grupo != null ? src.Grupo.AnoAcademicoId : Guid.Empty))
            .ForMember(dest => dest.AnoAcademico, opt => opt.MapFrom(src =>
                src.Grupo != null && src.Grupo.AnoAcademico != null ? src.Grupo.AnoAcademico.Nombre : string.Empty))
            .ForMember(dest => dest.GradoId, opt => opt.MapFrom(src => src.Grupo != null ? src.Grupo.GradoId : Guid.Empty))
            .ForMember(dest => dest.GradoNombre, opt => opt.MapFrom(src =>
                src.Grupo != null && src.Grupo.Grado != null ? src.Grupo.Grado.Nombre : string.Empty))
            .ForMember(dest => dest.GrupoNombre, opt => opt.MapFrom(src =>
                src.Grupo != null ? src.Grupo.Nombre : string.Empty))
            .ForMember(dest => dest.EstadoNombre, opt => opt.MapFrom(src =>
                GetEstadoMatriculaNombre(src.Estado)))
            .ForMember(dest => dest.FechaRetiro, opt => opt.MapFrom(src => src.FechaFinalizacion))
            .ForMember(dest => dest.MotivoRetiro, opt => opt.MapFrom(src => src.MotivoFinalizacion))
            .ForMember(dest => dest.ObservacionesMatricula, opt => opt.MapFrom(src => src.Observaciones));

        #endregion

        // ==============================================
        // MAPEOS PARA REPORTES Y ESTADÍSTICAS
        // ==============================================

        #region Mapeos para Reportes

        // Mapeo para reportes de estudiantes por grado
        CreateMap<Grupo, GrupoEstudiantesDto>()
            .ForMember(dest => dest.GrupoId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.GrupoNombre, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.TotalEstudiantes, opt => opt.Ignore()) // Se calcula manualmente
            .ForMember(dest => dest.EstudiantesHombres, opt => opt.Ignore()) // Se calcula manualmente
            .ForMember(dest => dest.EstudiantesMujeres, opt => opt.Ignore()) // Se calcula manualmente
            .ForMember(dest => dest.DirectorGrupo, opt => opt.MapFrom(src =>
                src.DirectorGrupo != null && src.DirectorGrupo.Persona != null
                    ? src.DirectorGrupo.Persona.NombreCompleto
                    : null));

        #endregion

        // ==============================================
        // MAPEOS PARA CALIFICACIONES Y ASISTENCIAS
        // ==============================================

        #region Mapeos relacionados con el rendimiento académico

        // Mapeo de Calificacion para estadísticas
        CreateMap<Calificacion, object>()
            .ForMember("CalificacionValor", opt => opt.MapFrom(src => src.CalificacionValor))
            .ForMember("MateriaId", opt => opt.MapFrom(src =>
                src.Asignacion != null ? src.Asignacion.MateriaId : Guid.Empty))
            .ForMember("MateriaNombre", opt => opt.MapFrom(src =>
                src.Asignacion != null && src.Asignacion.Materia != null
                    ? src.Asignacion.Materia.Nombre
                    : string.Empty))
            .ForMember("PeriodoId", opt => opt.MapFrom(src => src.PeriodoAcademicoId))
            .ForMember("TipoEvaluacion", opt => opt.MapFrom(src =>
                src.TipoEvaluacion != null ? src.TipoEvaluacion.Nombre : string.Empty));

        #endregion
    }

    // ==============================================
    // MÉTODOS AUXILIARES PARA MAPEOS
    // ==============================================

    /// <summary>
    /// Convierte el enum TipoRelacionAcudiente a su representación como string
    /// </summary>
    /// <param name="tipoRelacion">Tipo de relación enum</param>
    /// <returns>Nombre legible del tipo de relación</returns>
    private static string GetTipoRelacionNombre(TipoRelacionAcudiente tipoRelacion)
    {
        return tipoRelacion switch
        {
            TipoRelacionAcudiente.Padre => "Padre",
            TipoRelacionAcudiente.Madre => "Madre",
            TipoRelacionAcudiente.Abuelo => "Abuelo/a",
            TipoRelacionAcudiente.Tio => "Tío/a",
            TipoRelacionAcudiente.Hermano => "Hermano/a",
            TipoRelacionAcudiente.TutorLegal => "Tutor Legal",
            TipoRelacionAcudiente.Padrastro => "Padrastro",
            TipoRelacionAcudiente.Madrastra => "Madrastra",
            TipoRelacionAcudiente.OtroFamiliar => "Otro Familiar",
            TipoRelacionAcudiente.Acudiente => "Acudiente",
            _ => tipoRelacion.ToString()
        };
    }

    /// <summary>
    /// Convierte el enum EstadoMatricula a su representación como string
    /// </summary>
    /// <param name="estado">Estado de matrícula enum</param>
    /// <returns>Nombre legible del estado de matrícula</returns>
    private static string GetEstadoMatriculaNombre(EstadoMatricula estado)
    {
        return estado switch
        {
            EstadoMatricula.Activa => "Activa",
            EstadoMatricula.Suspendida => "Suspendida",
            EstadoMatricula.Retirada => "Retirada",
            EstadoMatricula.Trasladada => "Trasladada",
            EstadoMatricula.Graduada => "Graduada",
            EstadoMatricula.CondicionalAcademica => "Condicional Académica",
            EstadoMatricula.CondicionalDisciplinaria => "Condicional Disciplinaria",
            EstadoMatricula.Cancelada => "Cancelada",
            EstadoMatricula.PendienteDocumentos => "Pendiente Documentos",
            EstadoMatricula.PendientePago => "Pendiente Pago",
            _ => estado.ToString()
        };
    }

    // ==============================================
    // MAPEOS CONDICIONALES ADICIONALES
    // ==============================================

    /// <summary>
    /// Configuración adicional para mapeos condicionales que se pueden aplicar en runtime
    /// </summary>
    public static void ConfigurarMapeosCondicionales(IMapper mapper)
    {
        // Aquí se pueden agregar configuraciones adicionales si son necesarias
        // Por ejemplo, mapeos que dependan del contexto del usuario o configuraciones específicas
    }

    /// <summary>
    /// Mapeo personalizado para calcular estadísticas académicas
    /// Este método se puede usar en el servicio para mapear colecciones de calificaciones
    /// </summary>
    /// <param name="calificaciones">Lista de calificaciones del estudiante</param>
    /// <param name="asistencias">Lista de asistencias del estudiante</param>
    /// <param name="periodoNombre">Nombre del período evaluativo</param>
    /// <returns>DTO con estadísticas académicas calculadas</returns>
    public static EstadisticasAcademicasEstudianteDto MapearEstadisticasAcademicas(
        IEnumerable<Calificacion> calificaciones,
        IEnumerable<object> asistencias, // Cambiar por Asistencia cuando esté disponible
        string periodoNombre)
    {
        var listaCalificaciones = calificaciones.ToList();

        var estadisticas = new EstadisticasAcademicasEstudianteDto
        {
            PeriodoEvaluativo = periodoNombre,
            NumeroMaterias = listaCalificaciones.GroupBy(c => c.AsignacionId).Count(),
            PromedioGeneral = listaCalificaciones.Any()
                ? listaCalificaciones.Average(c => c.CalificacionValor)
                : 0,
            MateriasAprobadas = listaCalificaciones
                .GroupBy(c => c.AsignacionId)
                .Count(g => g.Average(c => c.CalificacionValor) >= 3.0m), // Asumiendo que 3.0 es la nota mínima
            MateriasReprobadas = listaCalificaciones
                .GroupBy(c => c.AsignacionId)
                .Count(g => g.Average(c => c.CalificacionValor) < 3.0m),
            MateriasEnRiesgo = listaCalificaciones
                .GroupBy(c => c.AsignacionId)
                .Count(g => g.Average(c => c.CalificacionValor) >= 3.0m && g.Average(c => c.CalificacionValor) < 3.5m),

            // Datos de asistencia - por implementar cuando esté disponible la entidad Asistencia
            PorcentajeAsistencia = 95.0m, // Valor por defecto
            FaltasInjustificadas = 0,
            FaltasJustificadas = 0,
            Tardanzas = 0
        };

        return estadisticas;
    }
}