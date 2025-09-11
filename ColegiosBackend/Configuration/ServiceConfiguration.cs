using ColegiosBackend.Application.Interfaces;
using ColegiosBackend.Application.Services;
using ColegiosBackend.Application.Mappings;
using ColegiosBackend.Core.Interfaces;
using ColegiosBackend.Infrastructure.Repositories;
using FluentValidation;
using System.Reflection;
using ColegiosBackend.Infrastructure.Data;

namespace ColegiosBackend.Configuration;

/// <summary>
/// Configuración centralizada de servicios para Dependency Injection
/// Registra todos los servicios, repositorios, mappers y validadores
/// </summary>
public static class ServiceConfiguration
{
    /// <summary>
    /// Registra todos los servicios de aplicación
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // ==============================================
        // SERVICIOS DE APLICACIÓN
        // ==============================================

        // Servicios principales
        services.AddScoped<IEstudianteService, EstudianteService>();

        // TODO: Agregar otros servicios cuando se implementen
        // services.AddScoped<IProfesorService, ProfesorService>();
        // services.AddScoped<IMateriaService, MateriaService>();
        // services.AddScoped<IMatriculaService, MatriculaService>();
        // services.AddScoped<ICalificacionService, CalificacionService>();
        // services.AddScoped<IAsistenciaService, AsistenciaService>();
        // services.AddScoped<IUsuarioService, UsuarioService>();
        // services.AddScoped<IColegioService, ColegioService>();
        // services.AddScoped<IGradoService, GradoService>();
        // services.AddScoped<IGrupoService, GrupoService>();
        // services.AddScoped<IHorarioService, HorarioService>();
        // services.AddScoped<IReporteService, ReporteService>();

        return services;
    }

    /// <summary>
    /// Registra todos los repositorios de infraestructura
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // ==============================================
        // REPOSITORIOS DE INFRAESTRUCTURA
        // ==============================================

        // Repositorios principales
        services.AddScoped<IEstudianteRepository, EstudianteRepository>();
        services.AddScoped<IMatriculaRepository, MatriculaRepository>();
        services.AddScoped<ICalificacionRepository, CalificacionRepository>();
        services.AddScoped<IAsistenciaRepository, AsistenciaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // TODO: Verificar que estos repositorios existan antes de descomentar
        // services.AddScoped<IProfesorRepository, ProfesorRepository>();
        // services.AddScoped<IMateriaRepository, MateriaRepository>();
        // services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        // services.AddScoped<IColegioRepository, ColegioRepository>();
        // services.AddScoped<IGradoRepository, GradoRepository>();
        // services.AddScoped<IGrupoRepository, GrupoRepository>();
        // services.AddScoped<IHorarioRepository, HorarioRepository>();
        // services.AddScoped<IPeriodoEvaluativoRepository, PeriodoEvaluativoRepository>();
        // services.AddScoped<ITipoEvaluacionRepository, TipoEvaluacionRepository>();
        // services.AddScoped<IAsignacionRepository, AsignacionRepository>();

        return services;
    }

    /// <summary>
    /// Registra AutoMapper con todos los profiles
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        // ==============================================
        // AUTOMAPPER CONFIGURATION
        // ==============================================

        // Registrar AutoMapper con los profiles del assembly de Application
        services.AddAutoMapper(config =>
        {
            // Registrar el profile de estudiantes
            config.AddProfile<EstudianteMappingProfile>();

            // TODO: Agregar otros profiles cuando se implementen
            // config.AddProfile<ProfesorMappingProfile>();
            // config.AddProfile<MateriaMappingProfile>();
            // config.AddProfile<MatriculaMappingProfile>();
            // config.AddProfile<CalificacionMappingProfile>();
            // config.AddProfile<AsistenciaMappingProfile>();
            // config.AddProfile<UsuarioMappingProfile>();
            // config.AddProfile<ColegioMappingProfile>();

            // Configuraciones adicionales de AutoMapper
            config.AllowNullCollections = true;
            config.AllowNullDestinationValues = true;

        }, typeof(EstudianteMappingProfile).Assembly);

        return services;
    }

    /// <summary>
    /// Registra FluentValidation con todos los validadores
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        // ==============================================
        // FLUENTVALIDATION CONFIGURATION
        // ==============================================

        // Registrar todos los validadores del assembly de Application
        services.AddValidatorsFromAssembly(typeof(EstudianteService).Assembly);

        // TODO: Cuando se implementen los validadores, se registrarán automáticamente
        // Los validadores esperados incluyen:
        // - CrearEstudianteDtoValidator
        // - ActualizarEstudianteDtoValidator
        // - CrearProfesorDtoValidator
        // - CrearMateriaDtoValidator
        // - etc.

        return services;
    }

    /// <summary>
    /// Registra MediatR para el patrón CQRS/Mediator
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddMediatRServices(this IServiceCollection services)
    {
        // ==============================================
        // MEDIATR CONFIGURATION
        // ==============================================

        // Registrar MediatR con handlers del assembly de Application
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(EstudianteService).Assembly);
        });

        // TODO: Cuando se implementen los handlers CQRS, se registrarán automáticamente
        // Los handlers esperados incluyen:
        // - GetEstudianteByIdQueryHandler
        // - CreateEstudianteCommandHandler
        // - UpdateEstudianteCommandHandler
        // - etc.

        return services;
    }

    /// <summary>
    /// Registra servicios de logging y observabilidad
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddLoggingServices(this IServiceCollection services)
    {
        // ==============================================
        // LOGGING CONFIGURATION
        // ==============================================

        // El logging está configurado en Program.cs con Serilog
        // Aquí se pueden agregar servicios adicionales de observabilidad

        // TODO: Agregar servicios de métricas y telemetría si es necesario
        // services.AddApplicationInsights();
        // services.AddHealthChecks();

        return services;
    }

    /// <summary>
    /// Registra servicios de seguridad y autenticación
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configuration">Configuración de la aplicación</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddSecurityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ==============================================
        // SECURITY CONFIGURATION
        // ==============================================

        // Configuración JWT (ya debería estar en Program.cs)
        // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //     .AddJwtBearer(options => { ... });

        // TODO: Agregar servicios de autorización específicos
        // services.AddAuthorization(options =>
        // {
        //     options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN"));
        //     options.AddPolicy("ColegioAccess", policy => policy.RequireClaim("ColegioId"));
        // });

        return services;
    }

    /// <summary>
    /// Registra servicios de caché
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configuration">Configuración de la aplicación</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddCachingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ==============================================
        // CACHING CONFIGURATION
        // ==============================================

        // Caché en memoria para desarrollo
        services.AddMemoryCache();

        // TODO: Configurar Redis para producción
        // var redisConnectionString = configuration.GetConnectionString("Redis");
        // if (!string.IsNullOrEmpty(redisConnectionString))
        // {
        //     services.AddStackExchangeRedisCache(options =>
        //     {
        //         options.Configuration = redisConnectionString;
        //     });
        // }

        return services;
    }

    /// <summary>
    /// Registra todos los servicios de la aplicación
    /// Método principal que orquesta el registro de todos los servicios
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configuration">Configuración de la aplicación</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddAllApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registrar todos los servicios en el orden correcto
        services
            .AddRepositories()                              // 1. Repositorios (capa más baja)
            .AddAutoMapperProfiles()                        // 2. Mappers
            .AddApplicationServices()                       // 3. Servicios de aplicación
            .AddFluentValidation()                          // 4. Validadores
            .AddMediatRServices()                          // 5. MediatR para CQRS
            .AddLoggingServices()                          // 6. Logging
            .AddSecurityServices(configuration)            // 7. Seguridad
            .AddCachingServices(configuration);            // 8. Caché

        return services;
    }

    /// <summary>
    /// Valida que todos los servicios requeridos estén registrados
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios validada</returns>
    public static IServiceCollection ValidateServiceRegistration(this IServiceCollection services)
    {
        // ==============================================
        // SERVICE VALIDATION
        // ==============================================

        // Verificar que los servicios críticos estén registrados
        var serviceProvider = services.BuildServiceProvider();

        try
        {
            // Validar servicios principales
            var estudianteService = serviceProvider.GetRequiredService<IEstudianteService>();
            var estudianteRepository = serviceProvider.GetRequiredService<IEstudianteRepository>();

            // Validar AutoMapper
            var mapper = serviceProvider.GetRequiredService<AutoMapper.IMapper>();

            // TODO: Agregar más validaciones cuando se implementen otros servicios

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Error en la configuración de servicios: {ex.Message}", ex);
        }
        finally
        {
            serviceProvider.Dispose();
        }

        return services;
    }

    /// <summary>
    /// Configuración específica para el entorno de desarrollo
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddDevelopmentServices(this IServiceCollection services)
    {
        // ==============================================
        // DEVELOPMENT-SPECIFIC SERVICES
        // ==============================================

        // Servicios específicos para desarrollo
        // services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }

    /// <summary>
    /// Configuración específica para el entorno de producción
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddProductionServices(this IServiceCollection services)
    {
        // ==============================================
        // PRODUCTION-SPECIFIC SERVICES
        // ==============================================

        // Servicios específicos para producción
        // services.AddApplicationInsights();
        // services.AddHealthChecks();

        return services;
    }
}