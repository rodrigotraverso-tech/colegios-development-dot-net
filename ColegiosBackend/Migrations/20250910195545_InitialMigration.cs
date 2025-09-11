using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColegiosBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "colegios",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    nombre_comercial = table.Column<string>(type: "text", nullable: true),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nit = table.Column<string>(type: "text", nullable: true),
                    esta_activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_fundacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    logo_url = table.Column<string>(type: "text", nullable: true),
                    configuracion_json = table.Column<string>(type: "text", nullable: true),
                    tipo_institucion = table.Column<int>(type: "integer", nullable: false),
                    niveles_educativos = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_colegios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "personas",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_documento_id = table.Column<int>(type: "integer", nullable: false),
                    numero_documento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombres = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    segundo_nombre = table.Column<string>(type: "text", nullable: true),
                    segundo_apellido = table.Column<string>(type: "text", nullable: true),
                    fecha_nacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    genero = table.Column<char>(type: "character(1)", nullable: true),
                    direccion = table.Column<string>(type: "text", nullable: true),
                    telefono = table.Column<string>(type: "text", nullable: true),
                    celular = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    foto_url = table.Column<string>(type: "text", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tipos_evaluacion",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    porcentaje = table.Column<decimal>(type: "numeric", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_evaluacion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "anos_academicos",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    configuracion = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anos_academicos", x => x.id);
                    table.ForeignKey(
                        name: "FK_anos_academicos_colegios_colegio_id",
                        column: x => x.colegio_id,
                        principalSchema: "public",
                        principalTable: "colegios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "materias",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    codigo_materia = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nombre_corto = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    area = table.Column<int>(type: "integer", nullable: false),
                    niveles_permitidos = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    intensidad_horaria_semanal = table.Column<int>(type: "integer", nullable: false),
                    creditos = table.Column<int>(type: "integer", nullable: true),
                    es_obligatoria = table.Column<bool>(type: "boolean", nullable: false),
                    es_practica = table.Column<bool>(type: "boolean", nullable: false),
                    requiere_materiales = table.Column<bool>(type: "boolean", nullable: false),
                    materiales_requeridos = table.Column<string>(type: "text", nullable: true),
                    materias_prerequisito = table.Column<string>(type: "text", nullable: true),
                    competencias = table.Column<string>(type: "text", nullable: true),
                    metodologia = table.Column<string>(type: "text", nullable: true),
                    criterios_evaluacion = table.Column<string>(type: "text", nullable: true),
                    porcentaje_minimo_aprobacion = table.Column<int>(type: "integer", nullable: false),
                    color_identificacion = table.Column<string>(type: "text", nullable: true),
                    icono = table.Column<string>(type: "text", nullable: true),
                    orden_presentacion = table.Column<int>(type: "integer", nullable: false),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_materias", x => x.id);
                    table.ForeignKey(
                        name: "f_k_materias_colegios_colegio_id",
                        column: x => x.colegio_id,
                        principalSchema: "public",
                        principalTable: "colegios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    permisos_json = table.Column<string>(type: "jsonb", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                    table.ForeignKey(
                        name: "f_k_roles_colegios_colegio_id",
                        column: x => x.colegio_id,
                        principalSchema: "public",
                        principalTable: "colegios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "estudiantes",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    persona_id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo_estudiante = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_egreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    motivo_egreso = table.Column<string>(type: "text", nullable: true),
                    numero_matricula = table.Column<string>(type: "text", nullable: true),
                    ano_ingreso = table.Column<int>(type: "integer", nullable: false),
                    informacion_medica = table.Column<string>(type: "text", nullable: true),
                    contacto_emergencia_nombre = table.Column<string>(type: "text", nullable: true),
                    contacto_emergencia_telefono = table.Column<string>(type: "text", nullable: true),
                    contacto_emergencia_relacion = table.Column<string>(type: "text", nullable: true),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estudiantes", x => x.id);
                    table.ForeignKey(
                        name: "FK_estudiantes_colegios_colegio_id",
                        column: x => x.colegio_id,
                        principalSchema: "public",
                        principalTable: "colegios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_estudiantes__personas_persona_id",
                        column: x => x.persona_id,
                        principalSchema: "public",
                        principalTable: "personas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "profesores",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    persona_id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo_profesor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_retiro = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    motivo_retiro = table.Column<string>(type: "text", nullable: true),
                    tipo_contrato = table.Column<int>(type: "integer", nullable: false),
                    cargo = table.Column<int>(type: "integer", nullable: false),
                    especialidades = table.Column<string>(type: "text", nullable: true),
                    titulos_academicos = table.Column<string>(type: "text", nullable: true),
                    anos_experiencia = table.Column<int>(type: "integer", nullable: true),
                    registro_profesional = table.Column<string>(type: "text", nullable: true),
                    salario_base = table.Column<decimal>(type: "numeric", nullable: true),
                    horas_semanales = table.Column<int>(type: "integer", nullable: true),
                    puede_ser_coordinador = table.Column<bool>(type: "boolean", nullable: false),
                    puede_ser_director_grupo = table.Column<bool>(type: "boolean", nullable: false),
                    disponible_reemplazos = table.Column<bool>(type: "boolean", nullable: false),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    contacto_emergencia_nombre = table.Column<string>(type: "text", nullable: true),
                    contacto_emergencia_telefono = table.Column<string>(type: "text", nullable: true),
                    contacto_emergencia_relacion = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profesores", x => x.id);
                    table.ForeignKey(
                        name: "FK_profesores_colegios_colegio_id",
                        column: x => x.colegio_id,
                        principalSchema: "public",
                        principalTable: "colegios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_profesores_personas_persona_id",
                        column: x => x.persona_id,
                        principalSchema: "public",
                        principalTable: "personas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    salt = table.Column<string>(type: "text", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    requiere_cambio_password = table.Column<bool>(type: "boolean", nullable: false),
                    ultimo_acceso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    intentos_fallidos = table.Column<int>(type: "integer", nullable: false),
                    bloqueado_hasta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    persona_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "f_k_usuarios_personas_persona_id",
                        column: x => x.persona_id,
                        principalSchema: "public",
                        principalTable: "personas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "periodos_evaluativos",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ano_academico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    numero = table.Column<int>(type: "integer", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_limite_calificaciones = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    peso_calificacion = table.Column<decimal>(type: "numeric", nullable: false),
                    permite_calificaciones = table.Column<bool>(type: "boolean", nullable: false),
                    configuracion_evaluacion = table.Column<string>(type: "text", nullable: true),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_periodos_evaluativos", x => x.id);
                    table.ForeignKey(
                        name: "f_k_periodos_evaluativos_anos_academicos_ano_academico_id",
                        column: x => x.ano_academico_id,
                        principalSchema: "public",
                        principalTable: "anos_academicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "estudiantes_acudientes",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    estudiante_id = table.Column<Guid>(type: "uuid", nullable: false),
                    acudiente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_relacion = table.Column<int>(type: "integer", nullable: false),
                    es_principal = table.Column<bool>(type: "boolean", nullable: false),
                    puede_retirar = table.Column<bool>(type: "boolean", nullable: false),
                    puede_autorizar = table.Column<bool>(type: "boolean", nullable: false),
                    recibir_notificaciones_academicas = table.Column<bool>(type: "boolean", nullable: false),
                    recibir_notificaciones_disciplinarias = table.Column<bool>(type: "boolean", nullable: false),
                    recibir_notificaciones_financieras = table.Column<bool>(type: "boolean", nullable: false),
                    orden_prioridad = table.Column<int>(type: "integer", nullable: false),
                    fecha_inicio_relacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_fin_relacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    esta_activa = table.Column<bool>(type: "boolean", nullable: false),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    restricciones = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estudiantes_acudientes", x => x.id);
                    table.ForeignKey(
                        name: "f_k_estudiantes_acudientes__personas_acudiente_id",
                        column: x => x.acudiente_id,
                        principalSchema: "public",
                        principalTable: "personas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_estudiantes_acudientes_colegios_colegio_id",
                        column: x => x.colegio_id,
                        principalSchema: "public",
                        principalTable: "colegios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_estudiantes_acudientes_estudiantes_estudiante_id",
                        column: x => x.estudiante_id,
                        principalSchema: "public",
                        principalTable: "estudiantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "grados",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    codigo_grado = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nombre_corto = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nivel = table.Column<int>(type: "integer", nullable: false),
                    numero_grado = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    edad_minima_recomendada = table.Column<int>(type: "integer", nullable: false),
                    edad_maxima_recomendada = table.Column<int>(type: "integer", nullable: false),
                    capacidad_maxima = table.Column<int>(type: "integer", nullable: false),
                    capacidad_maxima_por_seccion = table.Column<int>(type: "integer", nullable: false),
                    minimo_estudiantes_para_abrir = table.Column<int>(type: "integer", nullable: false),
                    coordinador_id = table.Column<Guid>(type: "uuid", nullable: true),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    objetivos_academicos = table.Column<string>(type: "text", nullable: true),
                    competencias = table.Column<string>(type: "text", nullable: true),
                    perfil_egreso = table.Column<string>(type: "text", nullable: true),
                    requisitos_promocion = table.Column<string>(type: "text", nullable: true),
                    duracion_periodos = table.Column<int>(type: "integer", nullable: false),
                    requiere_uniforme_especifico = table.Column<bool>(type: "boolean", nullable: false),
                    descripcion_uniforme = table.Column<string>(type: "text", nullable: true),
                    horario_inicio = table.Column<string>(type: "text", nullable: true),
                    horario_fin = table.Column<string>(type: "text", nullable: true),
                    dias_funcionamiento = table.Column<string>(type: "text", nullable: true),
                    color_identificacion = table.Column<string>(type: "text", nullable: true),
                    orden_presentacion = table.Column<int>(type: "integer", nullable: false),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grados", x => x.id);
                    table.ForeignKey(
                        name: "f_k_grados__profesores_coordinador_id",
                        column: x => x.coordinador_id,
                        principalSchema: "public",
                        principalTable: "profesores",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_grados_colegios_colegio_id",
                        column: x => x.colegio_id,
                        principalSchema: "public",
                        principalTable: "colegios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "profesores_colegios",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profesor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_salida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    persona_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profesores_colegios", x => x.id);
                    table.ForeignKey(
                        name: "f_k_profesores_colegios_colegios_colegio_id",
                        column: x => x.colegio_id,
                        principalSchema: "public",
                        principalTable: "colegios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_profesores_colegios_personas_persona_id",
                        column: x => x.persona_id,
                        principalSchema: "public",
                        principalTable: "personas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_profesores_colegios_profesores_profesor_id",
                        column: x => x.profesor_id,
                        principalSchema: "public",
                        principalTable: "profesores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuario_roles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rol_id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_revocacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario_roles", x => x.id);
                    table.ForeignKey(
                        name: "f_k_usuario_roles_colegios_colegio_id",
                        column: x => x.colegio_id,
                        principalSchema: "public",
                        principalTable: "colegios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_usuario_roles_roles_rol_id",
                        column: x => x.rol_id,
                        principalSchema: "public",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_usuario_roles_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalSchema: "public",
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "grupos",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    grado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ano_academico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    capacidad_maxima = table.Column<int>(type: "integer", nullable: false),
                    director_grupo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    aula = table.Column<string>(type: "text", nullable: true),
                    horario_inicio = table.Column<string>(type: "text", nullable: true),
                    horario_fin = table.Column<string>(type: "text", nullable: true),
                    jornada = table.Column<int>(type: "integer", nullable: true),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    configuracion = table.Column<string>(type: "text", nullable: true),
                    color_identificacion = table.Column<string>(type: "text", nullable: true),
                    orden_presentacion = table.Column<int>(type: "integer", nullable: false),
                    AnoAcademicoId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupos", x => x.id);
                    table.ForeignKey(
                        name: "FK_grupos_anos_academicos_AnoAcademicoId1",
                        column: x => x.AnoAcademicoId1,
                        principalSchema: "public",
                        principalTable: "anos_academicos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_grupos__profesores_director_grupo_id",
                        column: x => x.director_grupo_id,
                        principalSchema: "public",
                        principalTable: "profesores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "f_k_grupos_anos_academicos_ano_academico_id",
                        column: x => x.ano_academico_id,
                        principalSchema: "public",
                        principalTable: "anos_academicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_grupos_colegios_colegio_id",
                        column: x => x.colegio_id,
                        principalSchema: "public",
                        principalTable: "colegios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_grupos_grados_grado_id",
                        column: x => x.grado_id,
                        principalSchema: "public",
                        principalTable: "grados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "asignaciones",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profesor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    grupo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    materia_id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ano_academico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    intensidad_horaria_semanal = table.Column<int>(type: "integer", nullable: false),
                    permite_calificar = table.Column<bool>(type: "boolean", nullable: false),
                    permite_registrar_asistencia = table.Column<bool>(type: "boolean", nullable: false),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    configuracion_asignacion = table.Column<string>(type: "text", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asignaciones", x => x.id);
                    table.ForeignKey(
                        name: "f_k_asignaciones__grupos_grupo_id",
                        column: x => x.grupo_id,
                        principalSchema: "public",
                        principalTable: "grupos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_asignaciones__materias_materia_id",
                        column: x => x.materia_id,
                        principalSchema: "public",
                        principalTable: "materias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_asignaciones__profesores_profesor_id",
                        column: x => x.profesor_id,
                        principalSchema: "public",
                        principalTable: "profesores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_asignaciones_anos_academicos_ano_academico_id",
                        column: x => x.ano_academico_id,
                        principalSchema: "public",
                        principalTable: "anos_academicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "asistencias",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    estudiante_id = table.Column<Guid>(type: "uuid", nullable: false),
                    materia_id = table.Column<Guid>(type: "uuid", nullable: false),
                    grupo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ano_academico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    periodo_evaluativo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha_clase = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    profesor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    es_justificada = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_justificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    motivo_justificacion = table.Column<string>(type: "text", nullable: true),
                    justificado_por_id = table.Column<Guid>(type: "uuid", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asistencias", x => x.id);
                    table.ForeignKey(
                        name: "f_k_asistencias__estudiantes_estudiante_id",
                        column: x => x.estudiante_id,
                        principalSchema: "public",
                        principalTable: "estudiantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_asistencias__grupos_grupo_id",
                        column: x => x.grupo_id,
                        principalSchema: "public",
                        principalTable: "grupos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_asistencias__materias_materia_id",
                        column: x => x.materia_id,
                        principalSchema: "public",
                        principalTable: "materias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_asistencias__periodos_evaluativos_periodo_evaluativo_id",
                        column: x => x.periodo_evaluativo_id,
                        principalSchema: "public",
                        principalTable: "periodos_evaluativos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_asistencias__profesores_profesor_id",
                        column: x => x.profesor_id,
                        principalSchema: "public",
                        principalTable: "profesores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_asistencias_anos_academicos_ano_academico_id",
                        column: x => x.ano_academico_id,
                        principalSchema: "public",
                        principalTable: "anos_academicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "matriculas",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    estudiante_id = table.Column<Guid>(type: "uuid", nullable: false),
                    grupo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    numero_matricula = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    fecha_matricula = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_inicio_clases = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_finalizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    motivo_finalizacion = table.Column<string>(type: "text", nullable: true),
                    tipo_matricula = table.Column<int>(type: "integer", nullable: false),
                    es_estudiante_nuevo = table.Column<bool>(type: "boolean", nullable: false),
                    es_repitente = table.Column<bool>(type: "boolean", nullable: false),
                    colegio_procedencia = table.Column<string>(type: "text", nullable: true),
                    porcentaje_beca = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    tipo_beca = table.Column<int>(type: "integer", nullable: true),
                    motivo_beca = table.Column<string>(type: "text", nullable: true),
                    valor_matricula = table.Column<decimal>(type: "numeric", nullable: true),
                    valor_descuentos = table.Column<decimal>(type: "numeric", nullable: true),
                    valor_final = table.Column<decimal>(type: "numeric", nullable: true),
                    matricula_pagada = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_pago_matricula = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    documentos_presentados = table.Column<string>(type: "text", nullable: true),
                    documentos_pendientes = table.Column<string>(type: "text", nullable: true),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    condiciones_especiales = table.Column<string>(type: "text", nullable: true),
                    procesado_por_id = table.Column<Guid>(type: "uuid", nullable: true),
                    codigo_tipo_beca_configurado = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matriculas", x => x.id);
                    table.ForeignKey(
                        name: "f_k_matriculas__usuarios_procesado_por_id",
                        column: x => x.procesado_por_id,
                        principalSchema: "public",
                        principalTable: "usuarios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_matriculas_colegios_colegio_id",
                        column: x => x.colegio_id,
                        principalSchema: "public",
                        principalTable: "colegios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_matriculas_estudiantes_estudiante_id",
                        column: x => x.estudiante_id,
                        principalSchema: "public",
                        principalTable: "estudiantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_matriculas_grupos_grupo_id",
                        column: x => x.grupo_id,
                        principalSchema: "public",
                        principalTable: "grupos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "calificaciones",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    estudiante_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asignacion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    periodo_academico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_evaluacion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    calificacion_valor = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    calificacion_cualitativa = table.Column<string>(type: "text", nullable: true),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    fecha_calificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    profesor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    colegio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: false),
                    es_recuperacion = table.Column<bool>(type: "boolean", nullable: false),
                    calificacion_original_id = table.Column<Guid>(type: "uuid", nullable: true),
                    peso = table.Column<decimal>(type: "numeric", nullable: false),
                    configuracion_calificacion = table.Column<string>(type: "text", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calificaciones", x => x.id);
                    table.ForeignKey(
                        name: "f_k_calificaciones__estudiantes_estudiante_id",
                        column: x => x.estudiante_id,
                        principalSchema: "public",
                        principalTable: "estudiantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_calificaciones__periodos_evaluativos_periodo_academico_id",
                        column: x => x.periodo_academico_id,
                        principalSchema: "public",
                        principalTable: "periodos_evaluativos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_calificaciones__profesores_profesor_id",
                        column: x => x.profesor_id,
                        principalSchema: "public",
                        principalTable: "profesores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_calificaciones__tipos_evaluacion_tipo_evaluacion_id",
                        column: x => x.tipo_evaluacion_id,
                        principalSchema: "public",
                        principalTable: "tipos_evaluacion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_calificaciones_asignaciones_asignacion_id",
                        column: x => x.asignacion_id,
                        principalSchema: "public",
                        principalTable: "asignaciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_calificaciones_calificaciones_calificacion_original_id",
                        column: x => x.calificacion_original_id,
                        principalSchema: "public",
                        principalTable: "calificaciones",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_anos_academicos_codigo_colegio_id",
                schema: "public",
                table: "anos_academicos",
                columns: new[] { "codigo", "colegio_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_anos_academicos_colegio_id",
                schema: "public",
                table: "anos_academicos",
                column: "colegio_id");

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_ano_academico_id",
                schema: "public",
                table: "asignaciones",
                column: "ano_academico_id");

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_grupo_id",
                schema: "public",
                table: "asignaciones",
                column: "grupo_id");

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_materia_id",
                schema: "public",
                table: "asignaciones",
                column: "materia_id");

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_profesor_id_grupo_id_materia_id_ano_academico_~",
                schema: "public",
                table: "asignaciones",
                columns: new[] { "profesor_id", "grupo_id", "materia_id", "ano_academico_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_asistencia_estudiante_fecha",
                schema: "public",
                table: "asistencias",
                columns: new[] { "estudiante_id", "fecha_clase" });

            migrationBuilder.CreateIndex(
                name: "IX_asistencias_ano_academico_id",
                schema: "public",
                table: "asistencias",
                column: "ano_academico_id");

            migrationBuilder.CreateIndex(
                name: "IX_asistencias_estudiante_id_fecha_clase_materia_id_grupo_id",
                schema: "public",
                table: "asistencias",
                columns: new[] { "estudiante_id", "fecha_clase", "materia_id", "grupo_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_asistencias_grupo_id",
                schema: "public",
                table: "asistencias",
                column: "grupo_id");

            migrationBuilder.CreateIndex(
                name: "IX_asistencias_materia_id",
                schema: "public",
                table: "asistencias",
                column: "materia_id");

            migrationBuilder.CreateIndex(
                name: "IX_asistencias_periodo_evaluativo_id",
                schema: "public",
                table: "asistencias",
                column: "periodo_evaluativo_id");

            migrationBuilder.CreateIndex(
                name: "IX_asistencias_profesor_id",
                schema: "public",
                table: "asistencias",
                column: "profesor_id");

            migrationBuilder.CreateIndex(
                name: "idx_calificacion_estudiante_periodo",
                schema: "public",
                table: "calificaciones",
                columns: new[] { "estudiante_id", "periodo_academico_id" });

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_asignacion_id",
                schema: "public",
                table: "calificaciones",
                column: "asignacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_calificacion_original_id",
                schema: "public",
                table: "calificaciones",
                column: "calificacion_original_id");

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_estudiante_id_asignacion_id_periodo_academic~",
                schema: "public",
                table: "calificaciones",
                columns: new[] { "estudiante_id", "asignacion_id", "periodo_academico_id" });

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_periodo_academico_id",
                schema: "public",
                table: "calificaciones",
                column: "periodo_academico_id");

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_profesor_id",
                schema: "public",
                table: "calificaciones",
                column: "profesor_id");

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_tipo_evaluacion_id",
                schema: "public",
                table: "calificaciones",
                column: "tipo_evaluacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_colegios_codigo",
                schema: "public",
                table: "colegios",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_estudiante_colegio_estado",
                schema: "public",
                table: "estudiantes",
                columns: new[] { "colegio_id", "estado" });

            migrationBuilder.CreateIndex(
                name: "IX_estudiantes_codigo_estudiante_colegio_id",
                schema: "public",
                table: "estudiantes",
                columns: new[] { "codigo_estudiante", "colegio_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estudiantes_persona_id",
                schema: "public",
                table: "estudiantes",
                column: "persona_id");

            migrationBuilder.CreateIndex(
                name: "IX_estudiantes_acudientes_acudiente_id",
                schema: "public",
                table: "estudiantes_acudientes",
                column: "acudiente_id");

            migrationBuilder.CreateIndex(
                name: "IX_estudiantes_acudientes_colegio_id",
                schema: "public",
                table: "estudiantes_acudientes",
                column: "colegio_id");

            migrationBuilder.CreateIndex(
                name: "IX_estudiantes_acudientes_estudiante_id_acudiente_id",
                schema: "public",
                table: "estudiantes_acudientes",
                columns: new[] { "estudiante_id", "acudiente_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_grados_codigo_grado_colegio_id",
                schema: "public",
                table: "grados",
                columns: new[] { "codigo_grado", "colegio_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_grados_colegio_id",
                schema: "public",
                table: "grados",
                column: "colegio_id");

            migrationBuilder.CreateIndex(
                name: "IX_grados_coordinador_id",
                schema: "public",
                table: "grados",
                column: "coordinador_id");

            migrationBuilder.CreateIndex(
                name: "IX_grupos_ano_academico_id",
                schema: "public",
                table: "grupos",
                column: "ano_academico_id");

            migrationBuilder.CreateIndex(
                name: "IX_grupos_AnoAcademicoId1",
                schema: "public",
                table: "grupos",
                column: "AnoAcademicoId1");

            migrationBuilder.CreateIndex(
                name: "IX_grupos_colegio_id",
                schema: "public",
                table: "grupos",
                column: "colegio_id");

            migrationBuilder.CreateIndex(
                name: "IX_grupos_director_grupo_id",
                schema: "public",
                table: "grupos",
                column: "director_grupo_id");

            migrationBuilder.CreateIndex(
                name: "IX_grupos_grado_id_ano_academico_id_codigo",
                schema: "public",
                table: "grupos",
                columns: new[] { "grado_id", "ano_academico_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_materias_codigo_materia_colegio_id",
                schema: "public",
                table: "materias",
                columns: new[] { "codigo_materia", "colegio_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_materias_colegio_id",
                schema: "public",
                table: "materias",
                column: "colegio_id");

            migrationBuilder.CreateIndex(
                name: "idx_matricula_estudiante_grupo",
                schema: "public",
                table: "matriculas",
                columns: new[] { "estudiante_id", "grupo_id" });

            migrationBuilder.CreateIndex(
                name: "IX_matriculas_colegio_id",
                schema: "public",
                table: "matriculas",
                column: "colegio_id");

            migrationBuilder.CreateIndex(
                name: "IX_matriculas_grupo_id",
                schema: "public",
                table: "matriculas",
                column: "grupo_id");

            migrationBuilder.CreateIndex(
                name: "IX_matriculas_procesado_por_id",
                schema: "public",
                table: "matriculas",
                column: "procesado_por_id");

            migrationBuilder.CreateIndex(
                name: "IX_periodos_evaluativos_ano_academico_id",
                schema: "public",
                table: "periodos_evaluativos",
                column: "ano_academico_id");

            migrationBuilder.CreateIndex(
                name: "IX_personas_numero_documento",
                schema: "public",
                table: "personas",
                column: "numero_documento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_profesores_codigo_profesor_colegio_id",
                schema: "public",
                table: "profesores",
                columns: new[] { "codigo_profesor", "colegio_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_profesores_colegio_id",
                schema: "public",
                table: "profesores",
                column: "colegio_id");

            migrationBuilder.CreateIndex(
                name: "IX_profesores_persona_id",
                schema: "public",
                table: "profesores",
                column: "persona_id");

            migrationBuilder.CreateIndex(
                name: "IX_profesores_colegios_colegio_id",
                schema: "public",
                table: "profesores_colegios",
                column: "colegio_id");

            migrationBuilder.CreateIndex(
                name: "IX_profesores_colegios_persona_id",
                schema: "public",
                table: "profesores_colegios",
                column: "persona_id");

            migrationBuilder.CreateIndex(
                name: "IX_profesores_colegios_profesor_id_colegio_id",
                schema: "public",
                table: "profesores_colegios",
                columns: new[] { "profesor_id", "colegio_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_codigo",
                schema: "public",
                table: "roles",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_colegio_id",
                schema: "public",
                table: "roles",
                column: "colegio_id");

            migrationBuilder.CreateIndex(
                name: "IX_tipos_evaluacion_codigo_colegio_id",
                schema: "public",
                table: "tipos_evaluacion",
                columns: new[] { "codigo", "colegio_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_roles_colegio_id",
                schema: "public",
                table: "usuario_roles",
                column: "colegio_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_roles_rol_id",
                schema: "public",
                table: "usuario_roles",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_roles_usuario_id_rol_id_colegio_id",
                schema: "public",
                table: "usuario_roles",
                columns: new[] { "usuario_id", "rol_id", "colegio_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_email",
                schema: "public",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_persona_id",
                schema: "public",
                table: "usuarios",
                column: "persona_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_username",
                schema: "public",
                table: "usuarios",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asistencias",
                schema: "public");

            migrationBuilder.DropTable(
                name: "calificaciones",
                schema: "public");

            migrationBuilder.DropTable(
                name: "estudiantes_acudientes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "matriculas",
                schema: "public");

            migrationBuilder.DropTable(
                name: "profesores_colegios",
                schema: "public");

            migrationBuilder.DropTable(
                name: "usuario_roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "periodos_evaluativos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "tipos_evaluacion",
                schema: "public");

            migrationBuilder.DropTable(
                name: "asignaciones",
                schema: "public");

            migrationBuilder.DropTable(
                name: "estudiantes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "usuarios",
                schema: "public");

            migrationBuilder.DropTable(
                name: "grupos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "materias",
                schema: "public");

            migrationBuilder.DropTable(
                name: "anos_academicos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "grados",
                schema: "public");

            migrationBuilder.DropTable(
                name: "profesores",
                schema: "public");

            migrationBuilder.DropTable(
                name: "colegios",
                schema: "public");

            migrationBuilder.DropTable(
                name: "personas",
                schema: "public");
        }
    }
}
