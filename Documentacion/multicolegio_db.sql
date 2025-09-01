-- ===============================================
-- SISTEMA DE GESTIÓN ACADÉMICA MULTI-COLEGIO
-- Base de Datos PostgreSQL
-- ===============================================

-- Extensiones necesarias
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ===============================================
-- TABLAS DE CONFIGURACIÓN GLOBAL
-- ===============================================

-- Tabla principal de colegios (Multi-tenant)
CREATE TABLE colegios (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    codigo VARCHAR(10) UNIQUE NOT NULL,
    nombre VARCHAR(200) NOT NULL,
    direccion TEXT,
    telefono VARCHAR(20),
    email VARCHAR(100),
    logo_url VARCHAR(500),
    colores_tema JSONB, -- Para branding personalizado
    configuracion JSONB, -- Configuraciones específicas del colegio
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tipos de documentos de identidad
CREATE TABLE tipos_documento (
    id SERIAL PRIMARY KEY,
    codigo VARCHAR(10) UNIQUE NOT NULL,
    nombre VARCHAR(50) NOT NULL,
    activo BOOLEAN DEFAULT true
);

-- Tipos de usuarios del sistema
CREATE TABLE tipos_usuario (
    id SERIAL PRIMARY KEY,
    codigo VARCHAR(20) UNIQUE NOT NULL,
    nombre VARCHAR(50) NOT NULL,
    descripcion TEXT,
    activo BOOLEAN DEFAULT true
);

-- ===============================================
-- GESTIÓN DE USUARIOS E IDENTIDADES
-- ===============================================

-- Usuarios del sistema (Unificado para SSO)
CREATE TABLE usuarios (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    salt VARCHAR(100) NOT NULL,
    activo BOOLEAN DEFAULT true,
    requiere_cambio_password BOOLEAN DEFAULT false,
    ultimo_acceso TIMESTAMP,
    intentos_fallidos INTEGER DEFAULT 0,
    bloqueado_hasta TIMESTAMP,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Roles por colegio
CREATE TABLE roles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    codigo VARCHAR(20) NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    permisos JSONB, -- Array de permisos específicos
    activo BOOLEAN DEFAULT true,
    UNIQUE(colegio_id, codigo)
);

-- Asignación de roles a usuarios por colegio
CREATE TABLE usuario_roles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    usuario_id UUID NOT NULL REFERENCES usuarios(id),
    rol_id UUID NOT NULL REFERENCES roles(id),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    activo BOOLEAN DEFAULT true,
    fecha_asignacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(usuario_id, rol_id, colegio_id)
);

-- ===============================================
-- ESTRUCTURA ACADÉMICA
-- ===============================================

-- Años académicos por colegio
CREATE TABLE anos_academicos (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    codigo VARCHAR(10) NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    activo BOOLEAN DEFAULT true,
    configuracion JSONB, -- Períodos, escalas de calificación, etc.
    UNIQUE(colegio_id, codigo)
);

-- Niveles educativos (Preescolar, Primaria, Secundaria, etc.)
CREATE TABLE niveles_educativos (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    codigo VARCHAR(10) NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    orden INTEGER NOT NULL,
    activo BOOLEAN DEFAULT true,
    UNIQUE(colegio_id, codigo)
);

-- Grados por nivel
CREATE TABLE grados (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    nivel_educativo_id UUID NOT NULL REFERENCES niveles_educativos(id),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    codigo VARCHAR(10) NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    orden INTEGER NOT NULL,
    activo BOOLEAN DEFAULT true,
    UNIQUE(colegio_id, codigo)
);

-- Grupos/Secciones por grado
CREATE TABLE grupos (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    grado_id UUID NOT NULL REFERENCES grados(id),
    ano_academico_id UUID NOT NULL REFERENCES anos_academicos(id),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    codigo VARCHAR(10) NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    capacidad_maxima INTEGER DEFAULT 30,
    director_grupo_id UUID, -- Referencia a profesores
    activo BOOLEAN DEFAULT true,
    UNIQUE(colegio_id, ano_academico_id, codigo)
);

-- Materias/Asignaturas
CREATE TABLE materias (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    codigo VARCHAR(20) NOT NULL,
    nombre VARCHAR(200) NOT NULL,
    descripcion TEXT,
    area VARCHAR(100), -- Matemáticas, Ciencias, Humanidades, etc.
    activo BOOLEAN DEFAULT true,
    UNIQUE(colegio_id, codigo)
);

-- Pensum académico (Materias por grado)
CREATE TABLE pensum (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    grado_id UUID NOT NULL REFERENCES grados(id),
    materia_id UUID NOT NULL REFERENCES materias(id),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    ano_academico_id UUID NOT NULL REFERENCES anos_academicos(id),
    intensidad_horaria INTEGER DEFAULT 1,
    es_obligatoria BOOLEAN DEFAULT true,
    activo BOOLEAN DEFAULT true,
    UNIQUE(grado_id, materia_id, ano_academico_id)
);

-- ===============================================
-- GESTIÓN DE PERSONAS
-- ===============================================

-- Personas (Base para estudiantes, profesores, padres)
CREATE TABLE personas (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tipo_documento_id INTEGER NOT NULL REFERENCES tipos_documento(id),
    numero_documento VARCHAR(20) NOT NULL,
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    fecha_nacimiento DATE,
    genero CHAR(1) CHECK (genero IN ('M', 'F', 'O')),
    direccion TEXT,
    telefono VARCHAR(20),
    celular VARCHAR(20),
    email VARCHAR(100),
    foto_url VARCHAR(500),
    activo BOOLEAN DEFAULT true,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(tipo_documento_id, numero_documento)
);

-- Profesores
CREATE TABLE profesores (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    persona_id UUID NOT NULL REFERENCES personas(id),
    usuario_id UUID REFERENCES usuarios(id),
    codigo_empleado VARCHAR(20),
    especialidades TEXT[],
    fecha_ingreso DATE,
    activo BOOLEAN DEFAULT true,
    UNIQUE(codigo_empleado)
);

-- Relación profesores-colegios (Un profesor puede trabajar en varios colegios)
CREATE TABLE profesor_colegios (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    profesor_id UUID NOT NULL REFERENCES profesores(id),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    activo BOOLEAN DEFAULT true,
    fecha_ingreso DATE DEFAULT CURRENT_DATE,
    UNIQUE(profesor_id, colegio_id)
);

-- Estudiantes
CREATE TABLE estudiantes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    persona_id UUID NOT NULL REFERENCES personas(id),
    codigo_estudiante VARCHAR(20) NOT NULL,
    activo BOOLEAN DEFAULT true,
    UNIQUE(codigo_estudiante)
);

-- Matrículas (Relación estudiante-colegio-año académico)
CREATE TABLE matriculas (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    estudiante_id UUID NOT NULL REFERENCES estudiantes(id),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    ano_academico_id UUID NOT NULL REFERENCES anos_academicos(id),
    grupo_id UUID NOT NULL REFERENCES grupos(id),
    fecha_matricula DATE DEFAULT CURRENT_DATE,
    estado VARCHAR(20) DEFAULT 'ACTIVA', -- ACTIVA, RETIRADO, GRADUADO
    observaciones TEXT,
    UNIQUE(estudiante_id, colegio_id, ano_academico_id)
);

-- Padres/Acudientes
CREATE TABLE acudientes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    persona_id UUID NOT NULL REFERENCES personas(id),
    usuario_id UUID REFERENCES usuarios(id),
    profesion VARCHAR(100),
    empresa VARCHAR(200),
    activo BOOLEAN DEFAULT true
);

-- Relación estudiante-acudientes
CREATE TABLE estudiante_acudientes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    estudiante_id UUID NOT NULL REFERENCES estudiantes(id),
    acudiente_id UUID NOT NULL REFERENCES acudientes(id),
    parentesco VARCHAR(50) NOT NULL, -- Padre, Madre, Abuelo, Tío, etc.
    es_responsable_financiero BOOLEAN DEFAULT false,
    es_contacto_emergencia BOOLEAN DEFAULT false,
    autorizado_recoger BOOLEAN DEFAULT false,
    activo BOOLEAN DEFAULT true,
    UNIQUE(estudiante_id, acudiente_id)
);

-- ===============================================
-- GESTIÓN ACADÉMICA Y EVALUACIONES
-- ===============================================

-- Períodos académicos
CREATE TABLE periodos_academicos (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    ano_academico_id UUID NOT NULL REFERENCES anos_academicos(id),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    numero INTEGER NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    activo BOOLEAN DEFAULT true,
    UNIQUE(ano_academico_id, numero)
);

-- Asignación de materias a profesores y grupos
CREATE TABLE asignaciones (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    profesor_id UUID NOT NULL REFERENCES profesores(id),
    grupo_id UUID NOT NULL REFERENCES grupos(id),
    materia_id UUID NOT NULL REFERENCES materias(id),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    ano_academico_id UUID NOT NULL REFERENCES anos_academicos(id),
    activo BOOLEAN DEFAULT true,
    UNIQUE(grupo_id, materia_id, ano_academico_id)
);

-- Tipos de evaluación por colegio
CREATE TABLE tipos_evaluacion (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    codigo VARCHAR(20) NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    porcentaje DECIMAL(5,2),
    activo BOOLEAN DEFAULT true,
    UNIQUE(colegio_id, codigo)
);

-- Calificaciones
CREATE TABLE calificaciones (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    estudiante_id UUID NOT NULL REFERENCES estudiantes(id),
    asignacion_id UUID NOT NULL REFERENCES asignaciones(id),
    periodo_academico_id UUID NOT NULL REFERENCES periodos_academicos(id),
    tipo_evaluacion_id UUID NOT NULL REFERENCES tipos_evaluacion(id),
    calificacion DECIMAL(4,2) NOT NULL,
    observaciones TEXT,
    fecha_calificacion DATE DEFAULT CURRENT_DATE,
    profesor_id UUID NOT NULL REFERENCES profesores(id),
    colegio_id UUID NOT NULL REFERENCES colegios(id)
);

-- Asistencia
CREATE TABLE asistencia (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    estudiante_id UUID NOT NULL REFERENCES estudiantes(id),
    grupo_id UUID NOT NULL REFERENCES grupos(id),
    fecha DATE NOT NULL,
    estado VARCHAR(20) NOT NULL, -- PRESENTE, AUSENTE, TARDE, EXCUSADO
    observaciones TEXT,
    registrado_por UUID NOT NULL REFERENCES usuarios(id),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(estudiante_id, grupo_id, fecha)
);

-- ===============================================
-- GESTIÓN FINANCIERA
-- ===============================================

-- Conceptos de facturación por colegio
CREATE TABLE conceptos_facturacion (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    codigo VARCHAR(20) NOT NULL,
    nombre VARCHAR(200) NOT NULL,
    descripcion TEXT,
    valor_base DECIMAL(12,2),
    es_obligatorio BOOLEAN DEFAULT true,
    periodicidad VARCHAR(20), -- MENSUAL, ANUAL, UNICO
    activo BOOLEAN DEFAULT true,
    UNIQUE(colegio_id, codigo)
);

-- Facturas
CREATE TABLE facturas (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    numero_factura VARCHAR(50) NOT NULL,
    acudiente_id UUID NOT NULL REFERENCES acudientes(id),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    fecha_factura DATE NOT NULL,
    fecha_vencimiento DATE NOT NULL,
    subtotal DECIMAL(12,2) NOT NULL,
    descuentos DECIMAL(12,2) DEFAULT 0,
    total DECIMAL(12,2) NOT NULL,
    estado VARCHAR(20) DEFAULT 'PENDIENTE', -- PENDIENTE, PAGADA, VENCIDA, ANULADA
    observaciones TEXT,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(colegio_id, numero_factura)
);

-- Detalle de facturas
CREATE TABLE factura_detalles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    factura_id UUID NOT NULL REFERENCES facturas(id),
    concepto_facturacion_id UUID NOT NULL REFERENCES conceptos_facturacion(id),
    estudiante_id UUID REFERENCES estudiantes(id),
    cantidad INTEGER DEFAULT 1,
    valor_unitario DECIMAL(12,2) NOT NULL,
    descuento DECIMAL(12,2) DEFAULT 0,
    valor_total DECIMAL(12,2) NOT NULL
);

-- Pagos
CREATE TABLE pagos (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    factura_id UUID NOT NULL REFERENCES facturas(id),
    fecha_pago DATE NOT NULL,
    valor_pago DECIMAL(12,2) NOT NULL,
    metodo_pago VARCHAR(50), -- EFECTIVO, TARJETA, TRANSFERENCIA, etc.
    referencia VARCHAR(100),
    observaciones TEXT,
    registrado_por UUID NOT NULL REFERENCES usuarios(id),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ===============================================
-- COMUNICACIÓN Y NOTIFICACIONES
-- ===============================================

-- Tipos de notificación
CREATE TABLE tipos_notificacion (
    id SERIAL PRIMARY KEY,
    codigo VARCHAR(20) UNIQUE NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    activo BOOLEAN DEFAULT true
);

-- Notificaciones
CREATE TABLE notificaciones (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    tipo_notificacion_id INTEGER NOT NULL REFERENCES tipos_notificacion(id),
    titulo VARCHAR(200) NOT NULL,
    mensaje TEXT NOT NULL,
    usuario_emisor_id UUID NOT NULL REFERENCES usuarios(id),
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_programada TIMESTAMP,
    activo BOOLEAN DEFAULT true
);

-- Destinatarios de notificaciones
CREATE TABLE notificacion_destinatarios (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    notificacion_id UUID NOT NULL REFERENCES notificaciones(id),
    usuario_id UUID NOT NULL REFERENCES usuarios(id),
    leida BOOLEAN DEFAULT false,
    fecha_lectura TIMESTAMP,
    UNIQUE(notificacion_id, usuario_id)
);

-- Mensajes entre usuarios
CREATE TABLE mensajes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    colegio_id UUID NOT NULL REFERENCES colegios(id),
    usuario_emisor_id UUID NOT NULL REFERENCES usuarios(id),
    usuario_receptor_id UUID NOT NULL REFERENCES usuarios(id),
    asunto VARCHAR(200),
    mensaje TEXT NOT NULL,
    leido BOOLEAN DEFAULT false,
    fecha_envio TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_lectura TIMESTAMP
);

-- ===============================================
-- AUDITORÍA Y LOGS
-- ===============================================

-- Logs de auditoría
CREATE TABLE logs_auditoria (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    colegio_id UUID REFERENCES colegios(id),
    usuario_id UUID REFERENCES usuarios(id),
    tabla VARCHAR(100) NOT NULL,
    registro_id UUID,
    accion VARCHAR(20) NOT NULL, -- INSERT, UPDATE, DELETE
    datos_anteriores JSONB,
    datos_nuevos JSONB,
    ip_address INET,
    user_agent TEXT,
    fecha_accion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ===============================================
-- ÍNDICES PARA RENDIMIENTO
-- ===============================================

-- Índices principales
CREATE INDEX idx_usuarios_email ON usuarios(email);
CREATE INDEX idx_usuarios_username ON usuarios(username);
CREATE INDEX idx_personas_documento ON personas(tipo_documento_id, numero_documento);
CREATE INDEX idx_matriculas_estudiante_colegio ON matriculas(estudiante_id, colegio_id);
CREATE INDEX idx_calificaciones_estudiante_periodo ON calificaciones(estudiante_id, periodo_academico_id);
CREATE INDEX idx_asistencia_estudiante_fecha ON asistencia(estudiante_id, fecha);
CREATE INDEX idx_notificaciones_colegio_fecha ON notificaciones(colegio_id, fecha_creacion);
CREATE INDEX idx_facturas_acudiente_estado ON facturas(acudiente_id, estado);

-- ===============================================
-- TRIGGERS DE AUDITORÍA
-- ===============================================

-- Función para triggers de auditoría
CREATE OR REPLACE FUNCTION trigger_auditoria()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'DELETE' THEN
        INSERT INTO logs_auditoria(tabla, registro_id, accion, datos_anteriores)
        VALUES (TG_TABLE_NAME, OLD.id, TG_OP, row_to_json(OLD));
        RETURN OLD;
    ELSIF TG_OP = 'UPDATE' THEN
        INSERT INTO logs_auditoria(tabla, registro_id, accion, datos_anteriores, datos_nuevos)
        VALUES (TG_TABLE_NAME, NEW.id, TG_OP, row_to_json(OLD), row_to_json(NEW));
        RETURN NEW;
    ELSIF TG_OP = 'INSERT' THEN
        INSERT INTO logs_auditoria(tabla, registro_id, accion, datos_nuevos)
        VALUES (TG_TABLE_NAME, NEW.id, TG_OP, row_to_json(NEW));
        RETURN NEW;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- ===============================================
-- FUNCIONES DE UTILIDAD
-- ===============================================

-- Función para actualizar fecha_actualizacion
CREATE OR REPLACE FUNCTION actualizar_fecha_modificacion()
RETURNS TRIGGER AS $$
BEGIN
    NEW.fecha_actualizacion = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Triggers para fecha_actualizacion
CREATE TRIGGER trigger_actualizar_colegios 
    BEFORE UPDATE ON colegios 
    FOR EACH ROW EXECUTE FUNCTION actualizar_fecha_modificacion();

CREATE TRIGGER trigger_actualizar_usuarios 
    BEFORE UPDATE ON usuarios 
    FOR EACH ROW EXECUTE FUNCTION actualizar_fecha_modificacion();

CREATE TRIGGER trigger_actualizar_personas 
    BEFORE UPDATE ON personas 
    FOR EACH ROW EXECUTE FUNCTION actualizar_fecha_modificacion();

-- ===============================================
-- DATOS INICIALES BÁSICOS
-- ===============================================

-- Tipos de documento
INSERT INTO tipos_documento (codigo, nombre) VALUES 
('CC', 'Cédula de Ciudadanía'),
('TI', 'Tarjeta de Identidad'),
('CE', 'Cédula de Extranjería'),
('PP', 'Pasaporte'),
('RC', 'Registro Civil');

-- Tipos de usuario
INSERT INTO tipos_usuario (codigo, nombre, descripcion) VALUES 
('ADMIN_GLOBAL', 'Administrador Global', 'Administrador de todo el sistema'),
('ADMIN_COLEGIO', 'Administrador de Colegio', 'Administrador de un colegio específico'),
('PROFESOR', 'Profesor', 'Profesor de la institución'),
('ACUDIENTE', 'Acudiente', 'Padre de familia o acudiente'),
('ESTUDIANTE', 'Estudiante', 'Estudiante de la institución');

-- Tipos de notificación
INSERT INTO tipos_notificacion (codigo, nombre, descripcion) VALUES 
('ACADEMICA', 'Académica', 'Notificaciones relacionadas con calificaciones y rendimiento académico'),
('ADMINISTRATIVA', 'Administrativa', 'Notificaciones administrativas generales'),
('FINANCIERA', 'Financiera', 'Notificaciones sobre pagos y facturación'),
('EMERGENCIA', 'Emergencia', 'Notificaciones de emergencia'),
('EVENTO', 'Evento', 'Notificaciones sobre eventos y actividades');

-- ===============================================
-- COMENTARIOS DE DOCUMENTACIÓN
-- ===============================================

COMMENT ON TABLE colegios IS 'Tabla principal para manejo multi-tenant. Cada colegio es un tenant independiente.';
COMMENT ON TABLE usuarios IS 'Gestión unificada de usuarios con SSO. Un usuario puede tener acceso a múltiples colegios.';
COMMENT ON TABLE personas IS 'Tabla base para todas las personas del sistema (estudiantes, profesores, acudientes).';
COMMENT ON TABLE matriculas IS 'Relación estudiante-colegio-año académico. Permite historial completo de matrículas.';
COMMENT ON TABLE calificaciones IS 'Almacena todas las calificaciones con flexibilidad para diferentes sistemas de evaluación.';
COMMENT ON TABLE facturas IS 'Facturación que puede consolidar múltiples estudiantes de una familia en diferentes colegios.';

-- ===============================================
-- FIN DEL SCRIPT
-- ===============================================