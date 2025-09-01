--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5
-- Dumped by pg_dump version 17.5

-- Started on 2025-09-01 10:31:25

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 5422 (class 1262 OID 20045)
-- Name: colegios-claude; Type: DATABASE; Schema: -; Owner: -
--

CREATE DATABASE "colegios-claude" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Spanish_Bolivia.1252';


\encoding SQL_ASCII
\connect -reuse-previous=on "dbname='colegios-claude'"

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 3 (class 3079 OID 20057)
-- Name: pgcrypto; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS pgcrypto WITH SCHEMA public;


--
-- TOC entry 5423 (class 0 OID 0)
-- Dependencies: 3
-- Name: EXTENSION pgcrypto; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON EXTENSION pgcrypto IS 'cryptographic functions';


--
-- TOC entry 2 (class 3079 OID 20046)
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- TOC entry 5424 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


--
-- TOC entry 305 (class 1255 OID 20783)
-- Name: actualizar_fecha_modificacion(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.actualizar_fecha_modificacion() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW.fecha_actualizacion = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$;


--
-- TOC entry 306 (class 1255 OID 20811)
-- Name: es_admin_global(uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.es_admin_global(usuario_uuid uuid) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1 
        FROM usuario_roles ur
        JOIN roles r ON ur.rol_id = r.id
        WHERE ur.usuario_id = usuario_uuid 
        AND r.codigo IN ('ADMIN_GLOBAL', 'SUPER_ADMIN')
        AND r.colegio_id IS NULL
        AND ur.activo = true
        AND r.activo = true
    );
END;
$$;


--
-- TOC entry 5425 (class 0 OID 0)
-- Dependencies: 306
-- Name: FUNCTION es_admin_global(usuario_uuid uuid); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.es_admin_global(usuario_uuid uuid) IS 'Verifica si un usuario tiene rol de administrador global (ADMIN_GLOBAL o SUPER_ADMIN).';


--
-- TOC entry 318 (class 1255 OID 20812)
-- Name: obtener_colegios_usuario(uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.obtener_colegios_usuario(usuario_uuid uuid) RETURNS TABLE(colegio_id uuid, nombre_colegio character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Si es admin global, devolver todos los colegios
    IF es_admin_global(usuario_uuid) THEN
        RETURN QUERY
        SELECT c.id, c.nombre
        FROM colegios c
        WHERE c.activo = true
        ORDER BY c.nombre;
    ELSE
        -- Si no es admin global, devolver solo colegios asignados
        RETURN QUERY
        SELECT DISTINCT c.id, c.nombre
        FROM colegios c
        JOIN usuario_roles ur ON c.id = ur.colegio_id
        WHERE ur.usuario_id = usuario_uuid
        AND ur.activo = true
        AND c.activo = true
        ORDER BY c.nombre;
    END IF;
END;
$$;


--
-- TOC entry 5426 (class 0 OID 0)
-- Dependencies: 318
-- Name: FUNCTION obtener_colegios_usuario(usuario_uuid uuid); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.obtener_colegios_usuario(usuario_uuid uuid) IS 'Obtiene todos los colegios a los que un usuario tiene acceso. Admin globales ven todos los colegios.';


--
-- TOC entry 319 (class 1255 OID 20813)
-- Name: tiene_permiso_usuario(uuid, uuid, character varying, character varying); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.tiene_permiso_usuario(usuario_uuid uuid, colegio_uuid uuid, modulo_nombre character varying, accion_nombre character varying) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
DECLARE
    permisos_json JSONB;
    tiene_permiso BOOLEAN := false;
BEGIN
    -- Verificar si es admin global (acceso a todo)
    IF es_admin_global(usuario_uuid) THEN
        RETURN true;
    END IF;
    
    -- Buscar permisos específicos para el colegio
    SELECT r.permisos INTO permisos_json
    FROM usuario_roles ur
    JOIN roles r ON ur.rol_id = r.id
    WHERE ur.usuario_id = usuario_uuid
    AND ur.colegio_id = colegio_uuid
    AND ur.activo = true
    AND r.activo = true
    LIMIT 1;
    
    -- Verificar permiso específico en el JSON
    IF permisos_json IS NOT NULL THEN
        SELECT COALESCE(
            (permisos_json->'modulos'->modulo_nombre->>accion_nombre)::boolean,
            false
        ) INTO tiene_permiso;
    END IF;
    
    RETURN tiene_permiso;
END;
$$;


--
-- TOC entry 5427 (class 0 OID 0)
-- Dependencies: 319
-- Name: FUNCTION tiene_permiso_usuario(usuario_uuid uuid, colegio_uuid uuid, modulo_nombre character varying, accion_nombre character varying); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.tiene_permiso_usuario(usuario_uuid uuid, colegio_uuid uuid, modulo_nombre character varying, accion_nombre character varying) IS 'Verifica si un usuario tiene un permiso específico en un colegio. Admin globales tienen todos los permisos.';


--
-- TOC entry 304 (class 1255 OID 20782)
-- Name: trigger_auditoria(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.trigger_auditoria() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
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
$$;


--
-- TOC entry 320 (class 1255 OID 20814)
-- Name: validar_consistencia_usuario_rol(uuid, uuid, uuid); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.validar_consistencia_usuario_rol(p_usuario_id uuid, p_rol_id uuid, p_colegio_id uuid) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
DECLARE
    rol_es_global BOOLEAN;
    rol_colegio_id UUID;
BEGIN
    -- Obtener información del rol
    SELECT 
        (colegio_id IS NULL) as es_global,
        colegio_id
    INTO rol_es_global, rol_colegio_id
    FROM roles 
    WHERE id = p_rol_id AND activo = true;
    
    -- Si no se encuentra el rol, es inválido
    IF NOT FOUND THEN
        RETURN false;
    END IF;
    
    -- Si es rol global, p_colegio_id debe ser NULL
    IF rol_es_global AND p_colegio_id IS NOT NULL THEN
        RETURN false;
    END IF;
    
    -- Si es rol de colegio, p_colegio_id debe coincidir con rol_colegio_id
    IF NOT rol_es_global AND p_colegio_id != rol_colegio_id THEN
        RETURN false;
    END IF;
    
    RETURN true;
END;
$$;


--
-- TOC entry 5428 (class 0 OID 0)
-- Dependencies: 320
-- Name: FUNCTION validar_consistencia_usuario_rol(p_usuario_id uuid, p_rol_id uuid, p_colegio_id uuid); Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON FUNCTION public.validar_consistencia_usuario_rol(p_usuario_id uuid, p_rol_id uuid, p_colegio_id uuid) IS 'Valida que la asignación de rol y colegio sea consistente.';


--
-- TOC entry 321 (class 1255 OID 20829)
-- Name: validar_rol_contexto(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.validar_rol_contexto() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Validar que roles globales solo tengan códigos permitidos
    IF NEW.colegio_id IS NULL AND NEW.codigo NOT IN ('ADMIN_GLOBAL', 'SUPER_ADMIN') THEN
        RAISE EXCEPTION 'Solo se permiten códigos ADMIN_GLOBAL y SUPER_ADMIN para roles globales. Código recibido: %', NEW.codigo;
    END IF;
    
    -- Validar que roles de colegio no usen códigos globales
    IF NEW.colegio_id IS NOT NULL AND NEW.codigo IN ('ADMIN_GLOBAL', 'SUPER_ADMIN') THEN
        RAISE EXCEPTION 'Los códigos ADMIN_GLOBAL y SUPER_ADMIN están reservados para roles globales';
    END IF;
    
    RETURN NEW;
END;
$$;


--
-- TOC entry 322 (class 1255 OID 20830)
-- Name: validar_usuario_rol_asignacion(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.validar_usuario_rol_asignacion() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Usar la función de validación
    IF NOT validar_consistencia_usuario_rol(NEW.usuario_id, NEW.rol_id, NEW.colegio_id) THEN
        RAISE EXCEPTION 'Inconsistencia en asignación de rol: el rol y colegio no son compatibles';
    END IF;
    
    RETURN NEW;
END;
$$;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 238 (class 1259 OID 20414)
-- Name: acudientes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.acudientes (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    persona_id uuid NOT NULL,
    usuario_id uuid,
    profesion character varying(100),
    empresa character varying(200),
    activo boolean DEFAULT true
);


--
-- TOC entry 227 (class 1259 OID 20187)
-- Name: anos_academicos; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.anos_academicos (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    colegio_id uuid NOT NULL,
    codigo character varying(10) NOT NULL,
    nombre character varying(100) NOT NULL,
    fecha_inicio date NOT NULL,
    fecha_fin date NOT NULL,
    activo boolean DEFAULT true,
    configuracion jsonb
);


--
-- TOC entry 241 (class 1259 OID 20472)
-- Name: asignaciones; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.asignaciones (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    profesor_id uuid NOT NULL,
    grupo_id uuid NOT NULL,
    materia_id uuid NOT NULL,
    colegio_id uuid NOT NULL,
    ano_academico_id uuid NOT NULL,
    activo boolean DEFAULT true
);


--
-- TOC entry 244 (class 1259 OID 20561)
-- Name: asistencia; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.asistencia (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    estudiante_id uuid NOT NULL,
    grupo_id uuid NOT NULL,
    fecha date NOT NULL,
    estado character varying(20) NOT NULL,
    observaciones text,
    registrado_por uuid NOT NULL,
    colegio_id uuid NOT NULL,
    fecha_registro timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 243 (class 1259 OID 20522)
-- Name: calificaciones; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.calificaciones (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    estudiante_id uuid NOT NULL,
    asignacion_id uuid NOT NULL,
    periodo_academico_id uuid NOT NULL,
    tipo_evaluacion_id uuid NOT NULL,
    calificacion numeric(4,2) NOT NULL,
    observaciones text,
    fecha_calificacion date DEFAULT CURRENT_DATE,
    profesor_id uuid NOT NULL,
    colegio_id uuid NOT NULL
);


--
-- TOC entry 5429 (class 0 OID 0)
-- Dependencies: 243
-- Name: TABLE calificaciones; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.calificaciones IS 'Almacena todas las calificaciones con flexibilidad para diferentes sistemas de evaluación.';


--
-- TOC entry 219 (class 1259 OID 20094)
-- Name: colegios; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.colegios (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    codigo character varying(10) NOT NULL,
    nombre character varying(200) NOT NULL,
    direccion text,
    telefono character varying(20),
    email character varying(100),
    logo_url character varying(500),
    colores_tema jsonb,
    configuracion jsonb,
    activo boolean DEFAULT true,
    fecha_creacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 5430 (class 0 OID 0)
-- Dependencies: 219
-- Name: TABLE colegios; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.colegios IS 'Tabla principal para manejo multi-tenant. Cada colegio es un tenant independiente.';


--
-- TOC entry 245 (class 1259 OID 20592)
-- Name: conceptos_facturacion; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.conceptos_facturacion (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    colegio_id uuid NOT NULL,
    codigo character varying(20) NOT NULL,
    nombre character varying(200) NOT NULL,
    descripcion text,
    valor_base numeric(12,2),
    es_obligatorio boolean DEFAULT true,
    periodicidad character varying(20),
    activo boolean DEFAULT true
);


--
-- TOC entry 239 (class 1259 OID 20431)
-- Name: estudiante_acudientes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.estudiante_acudientes (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    estudiante_id uuid NOT NULL,
    acudiente_id uuid NOT NULL,
    parentesco character varying(50) NOT NULL,
    es_responsable_financiero boolean DEFAULT false,
    es_contacto_emergencia boolean DEFAULT false,
    autorizado_recoger boolean DEFAULT false,
    activo boolean DEFAULT true
);


--
-- TOC entry 236 (class 1259 OID 20368)
-- Name: estudiantes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.estudiantes (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    persona_id uuid NOT NULL,
    codigo_estudiante character varying(20) NOT NULL,
    activo boolean DEFAULT true
);


--
-- TOC entry 247 (class 1259 OID 20632)
-- Name: factura_detalles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.factura_detalles (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    factura_id uuid NOT NULL,
    concepto_facturacion_id uuid NOT NULL,
    estudiante_id uuid,
    cantidad integer DEFAULT 1,
    valor_unitario numeric(12,2) NOT NULL,
    descuento numeric(12,2) DEFAULT 0,
    valor_total numeric(12,2) NOT NULL
);


--
-- TOC entry 246 (class 1259 OID 20609)
-- Name: facturas; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.facturas (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    numero_factura character varying(50) NOT NULL,
    acudiente_id uuid NOT NULL,
    colegio_id uuid NOT NULL,
    fecha_factura date NOT NULL,
    fecha_vencimiento date NOT NULL,
    subtotal numeric(12,2) NOT NULL,
    descuentos numeric(12,2) DEFAULT 0,
    total numeric(12,2) NOT NULL,
    estado character varying(20) DEFAULT 'PENDIENTE'::character varying,
    observaciones text,
    fecha_creacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 5431 (class 0 OID 0)
-- Dependencies: 246
-- Name: TABLE facturas; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.facturas IS 'Facturación que puede consolidar múltiples estudiantes de una familia en diferentes colegios.';


--
-- TOC entry 229 (class 1259 OID 20217)
-- Name: grados; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.grados (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    nivel_educativo_id uuid NOT NULL,
    colegio_id uuid NOT NULL,
    codigo character varying(10) NOT NULL,
    nombre character varying(100) NOT NULL,
    orden integer NOT NULL,
    activo boolean DEFAULT true
);


--
-- TOC entry 230 (class 1259 OID 20236)
-- Name: grupos; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.grupos (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    grado_id uuid NOT NULL,
    ano_academico_id uuid NOT NULL,
    colegio_id uuid NOT NULL,
    codigo character varying(10) NOT NULL,
    nombre character varying(100) NOT NULL,
    capacidad_maxima integer DEFAULT 30,
    director_grupo_id uuid,
    activo boolean DEFAULT true
);


--
-- TOC entry 254 (class 1259 OID 20755)
-- Name: logs_auditoria; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.logs_auditoria (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    colegio_id uuid,
    usuario_id uuid,
    tabla character varying(100) NOT NULL,
    registro_id uuid,
    accion character varying(20) NOT NULL,
    datos_anteriores jsonb,
    datos_nuevos jsonb,
    ip_address inet,
    user_agent text,
    fecha_accion timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 231 (class 1259 OID 20261)
-- Name: materias; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.materias (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    colegio_id uuid NOT NULL,
    codigo character varying(20) NOT NULL,
    nombre character varying(200) NOT NULL,
    descripcion text,
    area character varying(100),
    activo boolean DEFAULT true
);


--
-- TOC entry 237 (class 1259 OID 20382)
-- Name: matriculas; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.matriculas (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    estudiante_id uuid NOT NULL,
    colegio_id uuid NOT NULL,
    ano_academico_id uuid NOT NULL,
    grupo_id uuid NOT NULL,
    fecha_matricula date DEFAULT CURRENT_DATE,
    estado character varying(20) DEFAULT 'ACTIVA'::character varying,
    observaciones text
);


--
-- TOC entry 5432 (class 0 OID 0)
-- Dependencies: 237
-- Name: TABLE matriculas; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.matriculas IS 'Relación estudiante-colegio-año académico. Permite historial completo de matrículas.';


--
-- TOC entry 253 (class 1259 OID 20730)
-- Name: mensajes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.mensajes (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    colegio_id uuid NOT NULL,
    usuario_emisor_id uuid NOT NULL,
    usuario_receptor_id uuid NOT NULL,
    asunto character varying(200),
    mensaje text NOT NULL,
    leido boolean DEFAULT false,
    fecha_envio timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    fecha_lectura timestamp without time zone
);


--
-- TOC entry 228 (class 1259 OID 20203)
-- Name: niveles_educativos; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.niveles_educativos (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    colegio_id uuid NOT NULL,
    codigo character varying(10) NOT NULL,
    nombre character varying(100) NOT NULL,
    orden integer NOT NULL,
    activo boolean DEFAULT true
);


--
-- TOC entry 252 (class 1259 OID 20711)
-- Name: notificacion_destinatarios; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.notificacion_destinatarios (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    notificacion_id uuid NOT NULL,
    usuario_id uuid NOT NULL,
    leida boolean DEFAULT false,
    fecha_lectura timestamp without time zone
);


--
-- TOC entry 251 (class 1259 OID 20686)
-- Name: notificaciones; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.notificaciones (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    colegio_id uuid NOT NULL,
    tipo_notificacion_id integer NOT NULL,
    titulo character varying(200) NOT NULL,
    mensaje text NOT NULL,
    usuario_emisor_id uuid NOT NULL,
    fecha_creacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    fecha_programada timestamp without time zone,
    activo boolean DEFAULT true
);


--
-- TOC entry 248 (class 1259 OID 20655)
-- Name: pagos; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.pagos (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    factura_id uuid NOT NULL,
    fecha_pago date NOT NULL,
    valor_pago numeric(12,2) NOT NULL,
    metodo_pago character varying(50),
    referencia character varying(100),
    observaciones text,
    registrado_por uuid NOT NULL,
    fecha_registro timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 232 (class 1259 OID 20277)
-- Name: pensum; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.pensum (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    grado_id uuid NOT NULL,
    materia_id uuid NOT NULL,
    colegio_id uuid NOT NULL,
    ano_academico_id uuid NOT NULL,
    intensidad_horaria integer DEFAULT 1,
    es_obligatoria boolean DEFAULT true,
    activo boolean DEFAULT true
);


--
-- TOC entry 240 (class 1259 OID 20453)
-- Name: periodos_academicos; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.periodos_academicos (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    ano_academico_id uuid NOT NULL,
    colegio_id uuid NOT NULL,
    numero integer NOT NULL,
    nombre character varying(100) NOT NULL,
    fecha_inicio date NOT NULL,
    fecha_fin date NOT NULL,
    activo boolean DEFAULT true
);


--
-- TOC entry 233 (class 1259 OID 20308)
-- Name: personas; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.personas (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    tipo_documento_id integer NOT NULL,
    numero_documento character varying(20) NOT NULL,
    nombres character varying(100) NOT NULL,
    apellidos character varying(100) NOT NULL,
    fecha_nacimiento date,
    genero character(1),
    direccion text,
    telefono character varying(20),
    celular character varying(20),
    email character varying(100),
    foto_url character varying(500),
    activo boolean DEFAULT true,
    fecha_creacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    segundo_nombre character varying(100),
    segundo_apellido character varying(100),
    CONSTRAINT personas_genero_check CHECK ((genero = ANY (ARRAY['M'::bpchar, 'F'::bpchar, 'O'::bpchar])))
);


--
-- TOC entry 5433 (class 0 OID 0)
-- Dependencies: 233
-- Name: TABLE personas; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.personas IS 'Tabla base para todas las personas del sistema. Incluye nombres completos (primer y segundo nombre/apellido) y datos básicos.';


--
-- TOC entry 5434 (class 0 OID 0)
-- Dependencies: 233
-- Name: COLUMN personas.segundo_nombre; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.personas.segundo_nombre IS 'Segundo nombre de la persona (opcional)';


--
-- TOC entry 5435 (class 0 OID 0)
-- Dependencies: 233
-- Name: COLUMN personas.segundo_apellido; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.personas.segundo_apellido IS 'Segundo apellido de la persona (opcional)';


--
-- TOC entry 235 (class 1259 OID 20348)
-- Name: profesor_colegios; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.profesor_colegios (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    profesor_id uuid NOT NULL,
    colegio_id uuid NOT NULL,
    activo boolean DEFAULT true,
    fecha_ingreso date DEFAULT CURRENT_DATE
);


--
-- TOC entry 234 (class 1259 OID 20327)
-- Name: profesores; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.profesores (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    persona_id uuid NOT NULL,
    usuario_id uuid,
    codigo_empleado character varying(20),
    especialidades text[],
    fecha_ingreso date,
    activo boolean DEFAULT true
);


--
-- TOC entry 225 (class 1259 OID 20146)
-- Name: roles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.roles (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    colegio_id uuid,
    codigo character varying(20) NOT NULL,
    nombre character varying(100) NOT NULL,
    descripcion text,
    permisos jsonb,
    activo boolean DEFAULT true,
    CONSTRAINT check_roles_globales CHECK ((((colegio_id IS NULL) AND ((codigo)::text = ANY ((ARRAY['ADMIN_GLOBAL'::character varying, 'SUPER_ADMIN'::character varying])::text[]))) OR ((colegio_id IS NOT NULL) AND ((codigo)::text <> ALL ((ARRAY['ADMIN_GLOBAL'::character varying, 'SUPER_ADMIN'::character varying])::text[])))))
);


--
-- TOC entry 5436 (class 0 OID 0)
-- Dependencies: 225
-- Name: TABLE roles; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.roles IS 'Roles del sistema. Puede ser global (colegio_id IS NULL) para ADMIN_GLOBAL y SUPER_ADMIN, o específico por colegio.';


--
-- TOC entry 5437 (class 0 OID 0)
-- Dependencies: 225
-- Name: COLUMN roles.colegio_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.roles.colegio_id IS 'ID del colegio. NULL para roles globales (ADMIN_GLOBAL, SUPER_ADMIN).';


--
-- TOC entry 221 (class 1259 OID 20108)
-- Name: tipos_documento; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.tipos_documento (
    id integer NOT NULL,
    codigo character varying(10) NOT NULL,
    nombre character varying(50) NOT NULL,
    activo boolean DEFAULT true
);


--
-- TOC entry 220 (class 1259 OID 20107)
-- Name: tipos_documento_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.tipos_documento_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 5438 (class 0 OID 0)
-- Dependencies: 220
-- Name: tipos_documento_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.tipos_documento_id_seq OWNED BY public.tipos_documento.id;


--
-- TOC entry 242 (class 1259 OID 20506)
-- Name: tipos_evaluacion; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.tipos_evaluacion (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    colegio_id uuid NOT NULL,
    codigo character varying(20) NOT NULL,
    nombre character varying(100) NOT NULL,
    descripcion text,
    porcentaje numeric(5,2),
    activo boolean DEFAULT true
);


--
-- TOC entry 250 (class 1259 OID 20675)
-- Name: tipos_notificacion; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.tipos_notificacion (
    id integer NOT NULL,
    codigo character varying(20) NOT NULL,
    nombre character varying(100) NOT NULL,
    descripcion text,
    activo boolean DEFAULT true
);


--
-- TOC entry 249 (class 1259 OID 20674)
-- Name: tipos_notificacion_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.tipos_notificacion_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 5439 (class 0 OID 0)
-- Dependencies: 249
-- Name: tipos_notificacion_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.tipos_notificacion_id_seq OWNED BY public.tipos_notificacion.id;


--
-- TOC entry 223 (class 1259 OID 20118)
-- Name: tipos_usuario; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.tipos_usuario (
    id integer NOT NULL,
    codigo character varying(20) NOT NULL,
    nombre character varying(50) NOT NULL,
    descripcion text,
    activo boolean DEFAULT true
);


--
-- TOC entry 222 (class 1259 OID 20117)
-- Name: tipos_usuario_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.tipos_usuario_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 5440 (class 0 OID 0)
-- Dependencies: 222
-- Name: tipos_usuario_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.tipos_usuario_id_seq OWNED BY public.tipos_usuario.id;


--
-- TOC entry 226 (class 1259 OID 20162)
-- Name: usuario_roles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.usuario_roles (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    usuario_id uuid NOT NULL,
    rol_id uuid NOT NULL,
    colegio_id uuid,
    activo boolean DEFAULT true,
    fecha_asignacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 5441 (class 0 OID 0)
-- Dependencies: 226
-- Name: TABLE usuario_roles; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.usuario_roles IS 'Asignación de roles a usuarios. Para roles globales, colegio_id es NULL. La consistencia se valida mediante triggers.';


--
-- TOC entry 5442 (class 0 OID 0)
-- Dependencies: 226
-- Name: COLUMN usuario_roles.colegio_id; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON COLUMN public.usuario_roles.colegio_id IS 'ID del colegio. NULL para asignaciones de roles globales.';


--
-- TOC entry 224 (class 1259 OID 20129)
-- Name: usuarios; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.usuarios (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    username character varying(50) NOT NULL,
    email character varying(100) NOT NULL,
    password_hash character varying(255) NOT NULL,
    salt character varying(100) NOT NULL,
    activo boolean DEFAULT true,
    requiere_cambio_password boolean DEFAULT false,
    ultimo_acceso timestamp without time zone,
    intentos_fallidos integer DEFAULT 0,
    bloqueado_hasta timestamp without time zone,
    fecha_creacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- TOC entry 5443 (class 0 OID 0)
-- Dependencies: 224
-- Name: TABLE usuarios; Type: COMMENT; Schema: public; Owner: -
--

COMMENT ON TABLE public.usuarios IS 'Gestión unificada de usuarios con SSO. Un usuario puede tener acceso a múltiples colegios.';


--
-- TOC entry 257 (class 1259 OID 20824)
-- Name: vista_roles_colegio; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vista_roles_colegio AS
 SELECT r.id,
    r.codigo,
    r.nombre,
    r.descripcion,
    r.permisos,
    r.activo,
    r.colegio_id,
    c.nombre AS colegio_nombre,
    'COLEGIO'::text AS ambito
   FROM (public.roles r
     JOIN public.colegios c ON ((r.colegio_id = c.id)))
  WHERE (r.colegio_id IS NOT NULL);


--
-- TOC entry 256 (class 1259 OID 20820)
-- Name: vista_roles_globales; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vista_roles_globales AS
 SELECT id,
    codigo,
    nombre,
    descripcion,
    permisos,
    activo,
    'GLOBAL'::text AS ambito
   FROM public.roles
  WHERE (colegio_id IS NULL);


--
-- TOC entry 255 (class 1259 OID 20815)
-- Name: vista_usuarios_roles; Type: VIEW; Schema: public; Owner: -
--

CREATE VIEW public.vista_usuarios_roles AS
 SELECT u.id AS usuario_id,
    u.username,
    u.email,
    ur.id AS usuario_rol_id,
    r.id AS rol_id,
    r.codigo AS rol_codigo,
    r.nombre AS rol_nombre,
    ur.colegio_id,
    c.nombre AS colegio_nombre,
        CASE
            WHEN (ur.colegio_id IS NULL) THEN 'GLOBAL'::text
            ELSE 'COLEGIO'::text
        END AS tipo_rol,
    ur.activo AS rol_activo,
    ur.fecha_asignacion
   FROM (((public.usuarios u
     JOIN public.usuario_roles ur ON ((u.id = ur.usuario_id)))
     JOIN public.roles r ON ((ur.rol_id = r.id)))
     LEFT JOIN public.colegios c ON ((ur.colegio_id = c.id)))
  WHERE (u.activo = true);


--
-- TOC entry 4944 (class 2604 OID 20111)
-- Name: tipos_documento id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_documento ALTER COLUMN id SET DEFAULT nextval('public.tipos_documento_id_seq'::regclass);


--
-- TOC entry 5017 (class 2604 OID 20678)
-- Name: tipos_notificacion id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_notificacion ALTER COLUMN id SET DEFAULT nextval('public.tipos_notificacion_id_seq'::regclass);


--
-- TOC entry 4946 (class 2604 OID 20121)
-- Name: tipos_usuario id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_usuario ALTER COLUMN id SET DEFAULT nextval('public.tipos_usuario_id_seq'::regclass);


--
-- TOC entry 5400 (class 0 OID 20414)
-- Dependencies: 238
-- Data for Name: acudientes; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5389 (class 0 OID 20187)
-- Dependencies: 227
-- Data for Name: anos_academicos; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5403 (class 0 OID 20472)
-- Dependencies: 241
-- Data for Name: asignaciones; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5406 (class 0 OID 20561)
-- Dependencies: 244
-- Data for Name: asistencia; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5405 (class 0 OID 20522)
-- Dependencies: 243
-- Data for Name: calificaciones; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5381 (class 0 OID 20094)
-- Dependencies: 219
-- Data for Name: colegios; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5407 (class 0 OID 20592)
-- Dependencies: 245
-- Data for Name: conceptos_facturacion; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5401 (class 0 OID 20431)
-- Dependencies: 239
-- Data for Name: estudiante_acudientes; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5398 (class 0 OID 20368)
-- Dependencies: 236
-- Data for Name: estudiantes; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5409 (class 0 OID 20632)
-- Dependencies: 247
-- Data for Name: factura_detalles; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5408 (class 0 OID 20609)
-- Dependencies: 246
-- Data for Name: facturas; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5391 (class 0 OID 20217)
-- Dependencies: 229
-- Data for Name: grados; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5392 (class 0 OID 20236)
-- Dependencies: 230
-- Data for Name: grupos; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5416 (class 0 OID 20755)
-- Dependencies: 254
-- Data for Name: logs_auditoria; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5393 (class 0 OID 20261)
-- Dependencies: 231
-- Data for Name: materias; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5399 (class 0 OID 20382)
-- Dependencies: 237
-- Data for Name: matriculas; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5415 (class 0 OID 20730)
-- Dependencies: 253
-- Data for Name: mensajes; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5390 (class 0 OID 20203)
-- Dependencies: 228
-- Data for Name: niveles_educativos; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5414 (class 0 OID 20711)
-- Dependencies: 252
-- Data for Name: notificacion_destinatarios; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5413 (class 0 OID 20686)
-- Dependencies: 251
-- Data for Name: notificaciones; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5410 (class 0 OID 20655)
-- Dependencies: 248
-- Data for Name: pagos; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5394 (class 0 OID 20277)
-- Dependencies: 232
-- Data for Name: pensum; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5402 (class 0 OID 20453)
-- Dependencies: 240
-- Data for Name: periodos_academicos; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5395 (class 0 OID 20308)
-- Dependencies: 233
-- Data for Name: personas; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5397 (class 0 OID 20348)
-- Dependencies: 235
-- Data for Name: profesor_colegios; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5396 (class 0 OID 20327)
-- Dependencies: 234
-- Data for Name: profesores; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5387 (class 0 OID 20146)
-- Dependencies: 225
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.roles VALUES ('7573c26c-64e2-4e6a-a550-75d48ec41c76', NULL, 'ADMIN_GLOBAL', 'Administrador Global del Sistema', 'Acceso completo a todos los colegios y funcionalidades del sistema', '{"modulos": {"reportes": {"boletines": true, "certificados": true, "listas_estudiantes": true, "reportes_academicos": true, "reportes_financieros": true, "reportes_disciplinarios": true}, "asistencia": {"ver": true, "editar": true, "reportes": true, "registrar": true, "justificar": true}, "financiero": {"descuentos": true, "ver_facturas": true, "recibir_pagos": true, "crear_facturas": true, "anular_facturas": true, "reportes_financieros": true}, "profesores": {"ver": true, "crear": true, "editar": true, "eliminar": true, "asignar_materias": true}, "estudiantes": {"ver": true, "crear": true, "editar": true, "eliminar": true, "exportar": true}, "administracion": {"logs_auditoria": true, "gestionar_roles": true, "backup_restaurar": true, "configurar_colegio": true, "gestionar_usuarios": true}, "calificaciones": {"ver": true, "crear": true, "editar": true, "eliminar": true, "publicar": true, "ver_otras_materias": true}, "comunicaciones": {"crear_circulares": true, "mensajes_masivos": true, "mensajes_acudientes": true, "enviar_notificaciones": true}}, "restricciones": {"ip_permitidas": [], "solo_su_grupo": false, "horario_acceso": {"activo": false}, "solo_sus_materias": false, "solo_horario_laboral": false, "solo_sus_estudiantes": false}, "configuracion_especial": {"gestiona_licencias": true, "puede_crear_colegios": true, "acceso_todos_colegios": true, "puede_eliminar_colegios": true, "puede_ver_notas_finales": true, "limite_estudiantes_consulta": 0, "notificar_cambios_importantes": false, "requiere_autorizacion_cambios": false, "puede_editar_despues_publicacion": true}}', true);
INSERT INTO public.roles VALUES ('09fdcdd4-5eaf-4550-8761-b5bd787de9d0', NULL, 'SUPER_ADMIN', 'Super Administrador', 'Acceso de desarrollador con permisos de sistema y base de datos', '{"modulos": {"sistema": {"ejecutar_scripts": true, "ver_logs_sistema": true, "acceso_base_datos": true, "gestionar_backups": true, "configurar_servidor": true}, "reportes": {"boletines": true, "certificados": true, "listas_estudiantes": true, "reportes_academicos": true, "reportes_financieros": true, "reportes_disciplinarios": true}, "asistencia": {"ver": true, "editar": true, "reportes": true, "registrar": true, "justificar": true}, "financiero": {"descuentos": true, "ver_facturas": true, "recibir_pagos": true, "crear_facturas": true, "anular_facturas": true, "reportes_financieros": true}, "profesores": {"ver": true, "crear": true, "editar": true, "eliminar": true, "asignar_materias": true}, "estudiantes": {"ver": true, "crear": true, "editar": true, "eliminar": true, "exportar": true}, "administracion": {"logs_auditoria": true, "gestionar_roles": true, "backup_restaurar": true, "configurar_colegio": true, "gestionar_usuarios": true}, "calificaciones": {"ver": true, "crear": true, "editar": true, "eliminar": true, "publicar": true, "ver_otras_materias": true}, "comunicaciones": {"crear_circulares": true, "mensajes_masivos": true, "mensajes_acudientes": true, "enviar_notificaciones": true}}, "restricciones": {"ip_permitidas": [], "solo_su_grupo": false, "horario_acceso": {"activo": false}, "solo_sus_materias": false, "solo_horario_laboral": false, "solo_sus_estudiantes": false}, "configuracion_especial": {"gestiona_licencias": true, "puede_ejecutar_sql": true, "puede_crear_colegios": true, "acceso_todos_colegios": true, "acceso_sistema_completo": true, "puede_eliminar_colegios": true, "puede_ver_notas_finales": true, "limite_estudiantes_consulta": 0, "notificar_cambios_importantes": false, "requiere_autorizacion_cambios": false, "puede_editar_despues_publicacion": true}}', true);


--
-- TOC entry 5383 (class 0 OID 20108)
-- Dependencies: 221
-- Data for Name: tipos_documento; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.tipos_documento VALUES (1, 'CC', 'Cédula de Ciudadanía', true);
INSERT INTO public.tipos_documento VALUES (2, 'TI', 'Tarjeta de Identidad', true);
INSERT INTO public.tipos_documento VALUES (3, 'CE', 'Cédula de Extranjería', true);
INSERT INTO public.tipos_documento VALUES (4, 'PP', 'Pasaporte', true);
INSERT INTO public.tipos_documento VALUES (5, 'RC', 'Registro Civil', true);


--
-- TOC entry 5404 (class 0 OID 20506)
-- Dependencies: 242
-- Data for Name: tipos_evaluacion; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5412 (class 0 OID 20675)
-- Dependencies: 250
-- Data for Name: tipos_notificacion; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.tipos_notificacion VALUES (1, 'ACADEMICA', 'Académica', 'Notificaciones relacionadas con calificaciones y rendimiento académico', true);
INSERT INTO public.tipos_notificacion VALUES (2, 'ADMINISTRATIVA', 'Administrativa', 'Notificaciones administrativas generales', true);
INSERT INTO public.tipos_notificacion VALUES (3, 'FINANCIERA', 'Financiera', 'Notificaciones sobre pagos y facturación', true);
INSERT INTO public.tipos_notificacion VALUES (4, 'EMERGENCIA', 'Emergencia', 'Notificaciones de emergencia', true);
INSERT INTO public.tipos_notificacion VALUES (5, 'EVENTO', 'Evento', 'Notificaciones sobre eventos y actividades', true);


--
-- TOC entry 5385 (class 0 OID 20118)
-- Dependencies: 223
-- Data for Name: tipos_usuario; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.tipos_usuario VALUES (1, 'ADMIN_GLOBAL', 'Administrador Global', 'Administrador de todo el sistema', true);
INSERT INTO public.tipos_usuario VALUES (2, 'ADMIN_COLEGIO', 'Administrador de Colegio', 'Administrador de un colegio específico', true);
INSERT INTO public.tipos_usuario VALUES (3, 'PROFESOR', 'Profesor', 'Profesor de la institución', true);
INSERT INTO public.tipos_usuario VALUES (4, 'ACUDIENTE', 'Acudiente', 'Padre de familia o acudiente', true);
INSERT INTO public.tipos_usuario VALUES (5, 'ESTUDIANTE', 'Estudiante', 'Estudiante de la institución', true);


--
-- TOC entry 5388 (class 0 OID 20162)
-- Dependencies: 226
-- Data for Name: usuario_roles; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5386 (class 0 OID 20129)
-- Dependencies: 224
-- Data for Name: usuarios; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 5444 (class 0 OID 0)
-- Dependencies: 220
-- Name: tipos_documento_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.tipos_documento_id_seq', 5, true);


--
-- TOC entry 5445 (class 0 OID 0)
-- Dependencies: 249
-- Name: tipos_notificacion_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.tipos_notificacion_id_seq', 5, true);


--
-- TOC entry 5446 (class 0 OID 0)
-- Dependencies: 222
-- Name: tipos_usuario_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.tipos_usuario_id_seq', 5, true);


--
-- TOC entry 5109 (class 2606 OID 20420)
-- Name: acudientes acudientes_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.acudientes
    ADD CONSTRAINT acudientes_pkey PRIMARY KEY (id);


--
-- TOC entry 5062 (class 2606 OID 20197)
-- Name: anos_academicos anos_academicos_colegio_id_codigo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.anos_academicos
    ADD CONSTRAINT anos_academicos_colegio_id_codigo_key UNIQUE (colegio_id, codigo);


--
-- TOC entry 5064 (class 2606 OID 20195)
-- Name: anos_academicos anos_academicos_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.anos_academicos
    ADD CONSTRAINT anos_academicos_pkey PRIMARY KEY (id);


--
-- TOC entry 5119 (class 2606 OID 20480)
-- Name: asignaciones asignaciones_grupo_id_materia_id_ano_academico_id_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asignaciones
    ADD CONSTRAINT asignaciones_grupo_id_materia_id_ano_academico_id_key UNIQUE (grupo_id, materia_id, ano_academico_id);


--
-- TOC entry 5121 (class 2606 OID 20478)
-- Name: asignaciones asignaciones_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asignaciones
    ADD CONSTRAINT asignaciones_pkey PRIMARY KEY (id);


--
-- TOC entry 5130 (class 2606 OID 20571)
-- Name: asistencia asistencia_estudiante_id_grupo_id_fecha_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asistencia
    ADD CONSTRAINT asistencia_estudiante_id_grupo_id_fecha_key UNIQUE (estudiante_id, grupo_id, fecha);


--
-- TOC entry 5132 (class 2606 OID 20569)
-- Name: asistencia asistencia_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asistencia
    ADD CONSTRAINT asistencia_pkey PRIMARY KEY (id);


--
-- TOC entry 5127 (class 2606 OID 20530)
-- Name: calificaciones calificaciones_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.calificaciones
    ADD CONSTRAINT calificaciones_pkey PRIMARY KEY (id);


--
-- TOC entry 5032 (class 2606 OID 20106)
-- Name: colegios colegios_codigo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.colegios
    ADD CONSTRAINT colegios_codigo_key UNIQUE (codigo);


--
-- TOC entry 5034 (class 2606 OID 20104)
-- Name: colegios colegios_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.colegios
    ADD CONSTRAINT colegios_pkey PRIMARY KEY (id);


--
-- TOC entry 5135 (class 2606 OID 20603)
-- Name: conceptos_facturacion conceptos_facturacion_colegio_id_codigo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.conceptos_facturacion
    ADD CONSTRAINT conceptos_facturacion_colegio_id_codigo_key UNIQUE (colegio_id, codigo);


--
-- TOC entry 5137 (class 2606 OID 20601)
-- Name: conceptos_facturacion conceptos_facturacion_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.conceptos_facturacion
    ADD CONSTRAINT conceptos_facturacion_pkey PRIMARY KEY (id);


--
-- TOC entry 5111 (class 2606 OID 20442)
-- Name: estudiante_acudientes estudiante_acudientes_estudiante_id_acudiente_id_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.estudiante_acudientes
    ADD CONSTRAINT estudiante_acudientes_estudiante_id_acudiente_id_key UNIQUE (estudiante_id, acudiente_id);


--
-- TOC entry 5113 (class 2606 OID 20440)
-- Name: estudiante_acudientes estudiante_acudientes_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.estudiante_acudientes
    ADD CONSTRAINT estudiante_acudientes_pkey PRIMARY KEY (id);


--
-- TOC entry 5100 (class 2606 OID 20376)
-- Name: estudiantes estudiantes_codigo_estudiante_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.estudiantes
    ADD CONSTRAINT estudiantes_codigo_estudiante_key UNIQUE (codigo_estudiante);


--
-- TOC entry 5102 (class 2606 OID 20374)
-- Name: estudiantes estudiantes_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.estudiantes
    ADD CONSTRAINT estudiantes_pkey PRIMARY KEY (id);


--
-- TOC entry 5144 (class 2606 OID 20639)
-- Name: factura_detalles factura_detalles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.factura_detalles
    ADD CONSTRAINT factura_detalles_pkey PRIMARY KEY (id);


--
-- TOC entry 5139 (class 2606 OID 20621)
-- Name: facturas facturas_colegio_id_numero_factura_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.facturas
    ADD CONSTRAINT facturas_colegio_id_numero_factura_key UNIQUE (colegio_id, numero_factura);


--
-- TOC entry 5141 (class 2606 OID 20619)
-- Name: facturas facturas_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.facturas
    ADD CONSTRAINT facturas_pkey PRIMARY KEY (id);


--
-- TOC entry 5070 (class 2606 OID 20225)
-- Name: grados grados_colegio_id_codigo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.grados
    ADD CONSTRAINT grados_colegio_id_codigo_key UNIQUE (colegio_id, codigo);


--
-- TOC entry 5072 (class 2606 OID 20223)
-- Name: grados grados_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.grados
    ADD CONSTRAINT grados_pkey PRIMARY KEY (id);


--
-- TOC entry 5074 (class 2606 OID 20245)
-- Name: grupos grupos_colegio_id_ano_academico_id_codigo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.grupos
    ADD CONSTRAINT grupos_colegio_id_ano_academico_id_codigo_key UNIQUE (colegio_id, ano_academico_id, codigo);


--
-- TOC entry 5076 (class 2606 OID 20243)
-- Name: grupos grupos_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.grupos
    ADD CONSTRAINT grupos_pkey PRIMARY KEY (id);


--
-- TOC entry 5161 (class 2606 OID 20763)
-- Name: logs_auditoria logs_auditoria_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.logs_auditoria
    ADD CONSTRAINT logs_auditoria_pkey PRIMARY KEY (id);


--
-- TOC entry 5078 (class 2606 OID 20271)
-- Name: materias materias_colegio_id_codigo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.materias
    ADD CONSTRAINT materias_colegio_id_codigo_key UNIQUE (colegio_id, codigo);


--
-- TOC entry 5080 (class 2606 OID 20269)
-- Name: materias materias_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.materias
    ADD CONSTRAINT materias_pkey PRIMARY KEY (id);


--
-- TOC entry 5105 (class 2606 OID 20393)
-- Name: matriculas matriculas_estudiante_id_colegio_id_ano_academico_id_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.matriculas
    ADD CONSTRAINT matriculas_estudiante_id_colegio_id_ano_academico_id_key UNIQUE (estudiante_id, colegio_id, ano_academico_id);


--
-- TOC entry 5107 (class 2606 OID 20391)
-- Name: matriculas matriculas_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.matriculas
    ADD CONSTRAINT matriculas_pkey PRIMARY KEY (id);


--
-- TOC entry 5159 (class 2606 OID 20739)
-- Name: mensajes mensajes_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mensajes
    ADD CONSTRAINT mensajes_pkey PRIMARY KEY (id);


--
-- TOC entry 5066 (class 2606 OID 20211)
-- Name: niveles_educativos niveles_educativos_colegio_id_codigo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.niveles_educativos
    ADD CONSTRAINT niveles_educativos_colegio_id_codigo_key UNIQUE (colegio_id, codigo);


--
-- TOC entry 5068 (class 2606 OID 20209)
-- Name: niveles_educativos niveles_educativos_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.niveles_educativos
    ADD CONSTRAINT niveles_educativos_pkey PRIMARY KEY (id);


--
-- TOC entry 5155 (class 2606 OID 20719)
-- Name: notificacion_destinatarios notificacion_destinatarios_notificacion_id_usuario_id_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificacion_destinatarios
    ADD CONSTRAINT notificacion_destinatarios_notificacion_id_usuario_id_key UNIQUE (notificacion_id, usuario_id);


--
-- TOC entry 5157 (class 2606 OID 20717)
-- Name: notificacion_destinatarios notificacion_destinatarios_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificacion_destinatarios
    ADD CONSTRAINT notificacion_destinatarios_pkey PRIMARY KEY (id);


--
-- TOC entry 5153 (class 2606 OID 20695)
-- Name: notificaciones notificaciones_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificaciones
    ADD CONSTRAINT notificaciones_pkey PRIMARY KEY (id);


--
-- TOC entry 5146 (class 2606 OID 20663)
-- Name: pagos pagos_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pagos
    ADD CONSTRAINT pagos_pkey PRIMARY KEY (id);


--
-- TOC entry 5082 (class 2606 OID 20287)
-- Name: pensum pensum_grado_id_materia_id_ano_academico_id_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pensum
    ADD CONSTRAINT pensum_grado_id_materia_id_ano_academico_id_key UNIQUE (grado_id, materia_id, ano_academico_id);


--
-- TOC entry 5084 (class 2606 OID 20285)
-- Name: pensum pensum_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pensum
    ADD CONSTRAINT pensum_pkey PRIMARY KEY (id);


--
-- TOC entry 5115 (class 2606 OID 20461)
-- Name: periodos_academicos periodos_academicos_ano_academico_id_numero_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.periodos_academicos
    ADD CONSTRAINT periodos_academicos_ano_academico_id_numero_key UNIQUE (ano_academico_id, numero);


--
-- TOC entry 5117 (class 2606 OID 20459)
-- Name: periodos_academicos periodos_academicos_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.periodos_academicos
    ADD CONSTRAINT periodos_academicos_pkey PRIMARY KEY (id);


--
-- TOC entry 5088 (class 2606 OID 20319)
-- Name: personas personas_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.personas
    ADD CONSTRAINT personas_pkey PRIMARY KEY (id);


--
-- TOC entry 5090 (class 2606 OID 20321)
-- Name: personas personas_tipo_documento_id_numero_documento_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.personas
    ADD CONSTRAINT personas_tipo_documento_id_numero_documento_key UNIQUE (tipo_documento_id, numero_documento);


--
-- TOC entry 5096 (class 2606 OID 20355)
-- Name: profesor_colegios profesor_colegios_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profesor_colegios
    ADD CONSTRAINT profesor_colegios_pkey PRIMARY KEY (id);


--
-- TOC entry 5098 (class 2606 OID 20357)
-- Name: profesor_colegios profesor_colegios_profesor_id_colegio_id_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profesor_colegios
    ADD CONSTRAINT profesor_colegios_profesor_id_colegio_id_key UNIQUE (profesor_id, colegio_id);


--
-- TOC entry 5092 (class 2606 OID 20337)
-- Name: profesores profesores_codigo_empleado_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profesores
    ADD CONSTRAINT profesores_codigo_empleado_key UNIQUE (codigo_empleado);


--
-- TOC entry 5094 (class 2606 OID 20335)
-- Name: profesores profesores_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profesores
    ADD CONSTRAINT profesores_pkey PRIMARY KEY (id);


--
-- TOC entry 5055 (class 2606 OID 20154)
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);


--
-- TOC entry 5036 (class 2606 OID 20116)
-- Name: tipos_documento tipos_documento_codigo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_documento
    ADD CONSTRAINT tipos_documento_codigo_key UNIQUE (codigo);


--
-- TOC entry 5038 (class 2606 OID 20114)
-- Name: tipos_documento tipos_documento_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_documento
    ADD CONSTRAINT tipos_documento_pkey PRIMARY KEY (id);


--
-- TOC entry 5123 (class 2606 OID 20516)
-- Name: tipos_evaluacion tipos_evaluacion_colegio_id_codigo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_evaluacion
    ADD CONSTRAINT tipos_evaluacion_colegio_id_codigo_key UNIQUE (colegio_id, codigo);


--
-- TOC entry 5125 (class 2606 OID 20514)
-- Name: tipos_evaluacion tipos_evaluacion_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_evaluacion
    ADD CONSTRAINT tipos_evaluacion_pkey PRIMARY KEY (id);


--
-- TOC entry 5148 (class 2606 OID 20685)
-- Name: tipos_notificacion tipos_notificacion_codigo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_notificacion
    ADD CONSTRAINT tipos_notificacion_codigo_key UNIQUE (codigo);


--
-- TOC entry 5150 (class 2606 OID 20683)
-- Name: tipos_notificacion tipos_notificacion_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_notificacion
    ADD CONSTRAINT tipos_notificacion_pkey PRIMARY KEY (id);


--
-- TOC entry 5040 (class 2606 OID 20128)
-- Name: tipos_usuario tipos_usuario_codigo_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_usuario
    ADD CONSTRAINT tipos_usuario_codigo_key UNIQUE (codigo);


--
-- TOC entry 5042 (class 2606 OID 20126)
-- Name: tipos_usuario tipos_usuario_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_usuario
    ADD CONSTRAINT tipos_usuario_pkey PRIMARY KEY (id);


--
-- TOC entry 5060 (class 2606 OID 20169)
-- Name: usuario_roles usuario_roles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuario_roles
    ADD CONSTRAINT usuario_roles_pkey PRIMARY KEY (id);


--
-- TOC entry 5046 (class 2606 OID 20145)
-- Name: usuarios usuarios_email_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_email_key UNIQUE (email);


--
-- TOC entry 5048 (class 2606 OID 20141)
-- Name: usuarios usuarios_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_pkey PRIMARY KEY (id);


--
-- TOC entry 5050 (class 2606 OID 20143)
-- Name: usuarios usuarios_username_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuarios
    ADD CONSTRAINT usuarios_username_key UNIQUE (username);


--
-- TOC entry 5133 (class 1259 OID 20779)
-- Name: idx_asistencia_estudiante_fecha; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_asistencia_estudiante_fecha ON public.asistencia USING btree (estudiante_id, fecha);


--
-- TOC entry 5128 (class 1259 OID 20778)
-- Name: idx_calificaciones_estudiante_periodo; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_calificaciones_estudiante_periodo ON public.calificaciones USING btree (estudiante_id, periodo_academico_id);


--
-- TOC entry 5142 (class 1259 OID 20781)
-- Name: idx_facturas_acudiente_estado; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_facturas_acudiente_estado ON public.facturas USING btree (acudiente_id, estado);


--
-- TOC entry 5103 (class 1259 OID 20777)
-- Name: idx_matriculas_estudiante_colegio; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_matriculas_estudiante_colegio ON public.matriculas USING btree (estudiante_id, colegio_id);


--
-- TOC entry 5151 (class 1259 OID 20780)
-- Name: idx_notificaciones_colegio_fecha; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_notificaciones_colegio_fecha ON public.notificaciones USING btree (colegio_id, fecha_creacion);


--
-- TOC entry 5085 (class 1259 OID 20776)
-- Name: idx_personas_documento; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_personas_documento ON public.personas USING btree (tipo_documento_id, numero_documento);


--
-- TOC entry 5086 (class 1259 OID 20833)
-- Name: idx_personas_nombre_completo; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_personas_nombre_completo ON public.personas USING gin (to_tsvector('spanish'::regconfig, (((((((nombres)::text || ' '::text) || (COALESCE(segundo_nombre, ''::character varying))::text) || ' '::text) || (apellidos)::text) || ' '::text) || (COALESCE(segundo_apellido, ''::character varying))::text)));


--
-- TOC entry 5051 (class 1259 OID 20798)
-- Name: idx_roles_codigo_contexto; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX idx_roles_codigo_contexto ON public.roles USING btree (COALESCE((colegio_id)::text, 'GLOBAL'::text), codigo);


--
-- TOC entry 5052 (class 1259 OID 20835)
-- Name: idx_roles_colegio; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_roles_colegio ON public.roles USING btree (colegio_id, codigo) WHERE (colegio_id IS NOT NULL);


--
-- TOC entry 5053 (class 1259 OID 20834)
-- Name: idx_roles_globales; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_roles_globales ON public.roles USING btree (codigo) WHERE (colegio_id IS NULL);


--
-- TOC entry 5056 (class 1259 OID 20837)
-- Name: idx_usuario_roles_colegio; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_usuario_roles_colegio ON public.usuario_roles USING btree (usuario_id, colegio_id) WHERE (colegio_id IS NOT NULL);


--
-- TOC entry 5057 (class 1259 OID 20836)
-- Name: idx_usuario_roles_globales; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_usuario_roles_globales ON public.usuario_roles USING btree (usuario_id, rol_id) WHERE (colegio_id IS NULL);


--
-- TOC entry 5058 (class 1259 OID 20805)
-- Name: idx_usuario_roles_unico; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX idx_usuario_roles_unico ON public.usuario_roles USING btree (usuario_id, rol_id, COALESCE((colegio_id)::text, 'GLOBAL'::text));


--
-- TOC entry 5043 (class 1259 OID 20774)
-- Name: idx_usuarios_email; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_usuarios_email ON public.usuarios USING btree (email);


--
-- TOC entry 5044 (class 1259 OID 20775)
-- Name: idx_usuarios_username; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_usuarios_username ON public.usuarios USING btree (username);


--
-- TOC entry 5228 (class 2620 OID 20784)
-- Name: colegios trigger_actualizar_colegios; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trigger_actualizar_colegios BEFORE UPDATE ON public.colegios FOR EACH ROW EXECUTE FUNCTION public.actualizar_fecha_modificacion();


--
-- TOC entry 5232 (class 2620 OID 20786)
-- Name: personas trigger_actualizar_personas; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trigger_actualizar_personas BEFORE UPDATE ON public.personas FOR EACH ROW EXECUTE FUNCTION public.actualizar_fecha_modificacion();


--
-- TOC entry 5229 (class 2620 OID 20785)
-- Name: usuarios trigger_actualizar_usuarios; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trigger_actualizar_usuarios BEFORE UPDATE ON public.usuarios FOR EACH ROW EXECUTE FUNCTION public.actualizar_fecha_modificacion();


--
-- TOC entry 5230 (class 2620 OID 20831)
-- Name: roles trigger_validar_rol_contexto; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trigger_validar_rol_contexto BEFORE INSERT OR UPDATE ON public.roles FOR EACH ROW EXECUTE FUNCTION public.validar_rol_contexto();


--
-- TOC entry 5231 (class 2620 OID 20832)
-- Name: usuario_roles trigger_validar_usuario_rol_asignacion; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trigger_validar_usuario_rol_asignacion BEFORE INSERT OR UPDATE ON public.usuario_roles FOR EACH ROW EXECUTE FUNCTION public.validar_usuario_rol_asignacion();


--
-- TOC entry 5188 (class 2606 OID 20421)
-- Name: acudientes acudientes_persona_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.acudientes
    ADD CONSTRAINT acudientes_persona_id_fkey FOREIGN KEY (persona_id) REFERENCES public.personas(id);


--
-- TOC entry 5189 (class 2606 OID 20426)
-- Name: acudientes acudientes_usuario_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.acudientes
    ADD CONSTRAINT acudientes_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);


--
-- TOC entry 5166 (class 2606 OID 20198)
-- Name: anos_academicos anos_academicos_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.anos_academicos
    ADD CONSTRAINT anos_academicos_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5194 (class 2606 OID 20501)
-- Name: asignaciones asignaciones_ano_academico_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asignaciones
    ADD CONSTRAINT asignaciones_ano_academico_id_fkey FOREIGN KEY (ano_academico_id) REFERENCES public.anos_academicos(id);


--
-- TOC entry 5195 (class 2606 OID 20496)
-- Name: asignaciones asignaciones_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asignaciones
    ADD CONSTRAINT asignaciones_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5196 (class 2606 OID 20486)
-- Name: asignaciones asignaciones_grupo_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asignaciones
    ADD CONSTRAINT asignaciones_grupo_id_fkey FOREIGN KEY (grupo_id) REFERENCES public.grupos(id);


--
-- TOC entry 5197 (class 2606 OID 20491)
-- Name: asignaciones asignaciones_materia_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asignaciones
    ADD CONSTRAINT asignaciones_materia_id_fkey FOREIGN KEY (materia_id) REFERENCES public.materias(id);


--
-- TOC entry 5198 (class 2606 OID 20481)
-- Name: asignaciones asignaciones_profesor_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asignaciones
    ADD CONSTRAINT asignaciones_profesor_id_fkey FOREIGN KEY (profesor_id) REFERENCES public.profesores(id);


--
-- TOC entry 5206 (class 2606 OID 20587)
-- Name: asistencia asistencia_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asistencia
    ADD CONSTRAINT asistencia_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5207 (class 2606 OID 20572)
-- Name: asistencia asistencia_estudiante_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asistencia
    ADD CONSTRAINT asistencia_estudiante_id_fkey FOREIGN KEY (estudiante_id) REFERENCES public.estudiantes(id);


--
-- TOC entry 5208 (class 2606 OID 20577)
-- Name: asistencia asistencia_grupo_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asistencia
    ADD CONSTRAINT asistencia_grupo_id_fkey FOREIGN KEY (grupo_id) REFERENCES public.grupos(id);


--
-- TOC entry 5209 (class 2606 OID 20582)
-- Name: asistencia asistencia_registrado_por_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.asistencia
    ADD CONSTRAINT asistencia_registrado_por_fkey FOREIGN KEY (registrado_por) REFERENCES public.usuarios(id);


--
-- TOC entry 5200 (class 2606 OID 20536)
-- Name: calificaciones calificaciones_asignacion_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.calificaciones
    ADD CONSTRAINT calificaciones_asignacion_id_fkey FOREIGN KEY (asignacion_id) REFERENCES public.asignaciones(id);


--
-- TOC entry 5201 (class 2606 OID 20556)
-- Name: calificaciones calificaciones_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.calificaciones
    ADD CONSTRAINT calificaciones_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5202 (class 2606 OID 20531)
-- Name: calificaciones calificaciones_estudiante_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.calificaciones
    ADD CONSTRAINT calificaciones_estudiante_id_fkey FOREIGN KEY (estudiante_id) REFERENCES public.estudiantes(id);


--
-- TOC entry 5203 (class 2606 OID 20541)
-- Name: calificaciones calificaciones_periodo_academico_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.calificaciones
    ADD CONSTRAINT calificaciones_periodo_academico_id_fkey FOREIGN KEY (periodo_academico_id) REFERENCES public.periodos_academicos(id);


--
-- TOC entry 5204 (class 2606 OID 20551)
-- Name: calificaciones calificaciones_profesor_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.calificaciones
    ADD CONSTRAINT calificaciones_profesor_id_fkey FOREIGN KEY (profesor_id) REFERENCES public.profesores(id);


--
-- TOC entry 5205 (class 2606 OID 20546)
-- Name: calificaciones calificaciones_tipo_evaluacion_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.calificaciones
    ADD CONSTRAINT calificaciones_tipo_evaluacion_id_fkey FOREIGN KEY (tipo_evaluacion_id) REFERENCES public.tipos_evaluacion(id);


--
-- TOC entry 5210 (class 2606 OID 20604)
-- Name: conceptos_facturacion conceptos_facturacion_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.conceptos_facturacion
    ADD CONSTRAINT conceptos_facturacion_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5190 (class 2606 OID 20448)
-- Name: estudiante_acudientes estudiante_acudientes_acudiente_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.estudiante_acudientes
    ADD CONSTRAINT estudiante_acudientes_acudiente_id_fkey FOREIGN KEY (acudiente_id) REFERENCES public.acudientes(id);


--
-- TOC entry 5191 (class 2606 OID 20443)
-- Name: estudiante_acudientes estudiante_acudientes_estudiante_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.estudiante_acudientes
    ADD CONSTRAINT estudiante_acudientes_estudiante_id_fkey FOREIGN KEY (estudiante_id) REFERENCES public.estudiantes(id);


--
-- TOC entry 5183 (class 2606 OID 20377)
-- Name: estudiantes estudiantes_persona_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.estudiantes
    ADD CONSTRAINT estudiantes_persona_id_fkey FOREIGN KEY (persona_id) REFERENCES public.personas(id);


--
-- TOC entry 5213 (class 2606 OID 20645)
-- Name: factura_detalles factura_detalles_concepto_facturacion_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.factura_detalles
    ADD CONSTRAINT factura_detalles_concepto_facturacion_id_fkey FOREIGN KEY (concepto_facturacion_id) REFERENCES public.conceptos_facturacion(id);


--
-- TOC entry 5214 (class 2606 OID 20650)
-- Name: factura_detalles factura_detalles_estudiante_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.factura_detalles
    ADD CONSTRAINT factura_detalles_estudiante_id_fkey FOREIGN KEY (estudiante_id) REFERENCES public.estudiantes(id);


--
-- TOC entry 5215 (class 2606 OID 20640)
-- Name: factura_detalles factura_detalles_factura_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.factura_detalles
    ADD CONSTRAINT factura_detalles_factura_id_fkey FOREIGN KEY (factura_id) REFERENCES public.facturas(id);


--
-- TOC entry 5211 (class 2606 OID 20622)
-- Name: facturas facturas_acudiente_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.facturas
    ADD CONSTRAINT facturas_acudiente_id_fkey FOREIGN KEY (acudiente_id) REFERENCES public.acudientes(id);


--
-- TOC entry 5212 (class 2606 OID 20627)
-- Name: facturas facturas_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.facturas
    ADD CONSTRAINT facturas_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5168 (class 2606 OID 20231)
-- Name: grados grados_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.grados
    ADD CONSTRAINT grados_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5169 (class 2606 OID 20226)
-- Name: grados grados_nivel_educativo_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.grados
    ADD CONSTRAINT grados_nivel_educativo_id_fkey FOREIGN KEY (nivel_educativo_id) REFERENCES public.niveles_educativos(id);


--
-- TOC entry 5170 (class 2606 OID 20251)
-- Name: grupos grupos_ano_academico_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.grupos
    ADD CONSTRAINT grupos_ano_academico_id_fkey FOREIGN KEY (ano_academico_id) REFERENCES public.anos_academicos(id);


--
-- TOC entry 5171 (class 2606 OID 20256)
-- Name: grupos grupos_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.grupos
    ADD CONSTRAINT grupos_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5172 (class 2606 OID 20246)
-- Name: grupos grupos_grado_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.grupos
    ADD CONSTRAINT grupos_grado_id_fkey FOREIGN KEY (grado_id) REFERENCES public.grados(id);


--
-- TOC entry 5226 (class 2606 OID 20764)
-- Name: logs_auditoria logs_auditoria_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.logs_auditoria
    ADD CONSTRAINT logs_auditoria_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5227 (class 2606 OID 20769)
-- Name: logs_auditoria logs_auditoria_usuario_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.logs_auditoria
    ADD CONSTRAINT logs_auditoria_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);


--
-- TOC entry 5173 (class 2606 OID 20272)
-- Name: materias materias_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.materias
    ADD CONSTRAINT materias_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5184 (class 2606 OID 20404)
-- Name: matriculas matriculas_ano_academico_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.matriculas
    ADD CONSTRAINT matriculas_ano_academico_id_fkey FOREIGN KEY (ano_academico_id) REFERENCES public.anos_academicos(id);


--
-- TOC entry 5185 (class 2606 OID 20399)
-- Name: matriculas matriculas_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.matriculas
    ADD CONSTRAINT matriculas_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5186 (class 2606 OID 20394)
-- Name: matriculas matriculas_estudiante_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.matriculas
    ADD CONSTRAINT matriculas_estudiante_id_fkey FOREIGN KEY (estudiante_id) REFERENCES public.estudiantes(id);


--
-- TOC entry 5187 (class 2606 OID 20409)
-- Name: matriculas matriculas_grupo_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.matriculas
    ADD CONSTRAINT matriculas_grupo_id_fkey FOREIGN KEY (grupo_id) REFERENCES public.grupos(id);


--
-- TOC entry 5223 (class 2606 OID 20740)
-- Name: mensajes mensajes_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mensajes
    ADD CONSTRAINT mensajes_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5224 (class 2606 OID 20745)
-- Name: mensajes mensajes_usuario_emisor_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mensajes
    ADD CONSTRAINT mensajes_usuario_emisor_id_fkey FOREIGN KEY (usuario_emisor_id) REFERENCES public.usuarios(id);


--
-- TOC entry 5225 (class 2606 OID 20750)
-- Name: mensajes mensajes_usuario_receptor_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.mensajes
    ADD CONSTRAINT mensajes_usuario_receptor_id_fkey FOREIGN KEY (usuario_receptor_id) REFERENCES public.usuarios(id);


--
-- TOC entry 5167 (class 2606 OID 20212)
-- Name: niveles_educativos niveles_educativos_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.niveles_educativos
    ADD CONSTRAINT niveles_educativos_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5221 (class 2606 OID 20720)
-- Name: notificacion_destinatarios notificacion_destinatarios_notificacion_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificacion_destinatarios
    ADD CONSTRAINT notificacion_destinatarios_notificacion_id_fkey FOREIGN KEY (notificacion_id) REFERENCES public.notificaciones(id);


--
-- TOC entry 5222 (class 2606 OID 20725)
-- Name: notificacion_destinatarios notificacion_destinatarios_usuario_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificacion_destinatarios
    ADD CONSTRAINT notificacion_destinatarios_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);


--
-- TOC entry 5218 (class 2606 OID 20696)
-- Name: notificaciones notificaciones_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificaciones
    ADD CONSTRAINT notificaciones_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5219 (class 2606 OID 20701)
-- Name: notificaciones notificaciones_tipo_notificacion_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificaciones
    ADD CONSTRAINT notificaciones_tipo_notificacion_id_fkey FOREIGN KEY (tipo_notificacion_id) REFERENCES public.tipos_notificacion(id);


--
-- TOC entry 5220 (class 2606 OID 20706)
-- Name: notificaciones notificaciones_usuario_emisor_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.notificaciones
    ADD CONSTRAINT notificaciones_usuario_emisor_id_fkey FOREIGN KEY (usuario_emisor_id) REFERENCES public.usuarios(id);


--
-- TOC entry 5216 (class 2606 OID 20664)
-- Name: pagos pagos_factura_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pagos
    ADD CONSTRAINT pagos_factura_id_fkey FOREIGN KEY (factura_id) REFERENCES public.facturas(id);


--
-- TOC entry 5217 (class 2606 OID 20669)
-- Name: pagos pagos_registrado_por_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pagos
    ADD CONSTRAINT pagos_registrado_por_fkey FOREIGN KEY (registrado_por) REFERENCES public.usuarios(id);


--
-- TOC entry 5174 (class 2606 OID 20303)
-- Name: pensum pensum_ano_academico_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pensum
    ADD CONSTRAINT pensum_ano_academico_id_fkey FOREIGN KEY (ano_academico_id) REFERENCES public.anos_academicos(id);


--
-- TOC entry 5175 (class 2606 OID 20298)
-- Name: pensum pensum_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pensum
    ADD CONSTRAINT pensum_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5176 (class 2606 OID 20288)
-- Name: pensum pensum_grado_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pensum
    ADD CONSTRAINT pensum_grado_id_fkey FOREIGN KEY (grado_id) REFERENCES public.grados(id);


--
-- TOC entry 5177 (class 2606 OID 20293)
-- Name: pensum pensum_materia_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pensum
    ADD CONSTRAINT pensum_materia_id_fkey FOREIGN KEY (materia_id) REFERENCES public.materias(id);


--
-- TOC entry 5192 (class 2606 OID 20462)
-- Name: periodos_academicos periodos_academicos_ano_academico_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.periodos_academicos
    ADD CONSTRAINT periodos_academicos_ano_academico_id_fkey FOREIGN KEY (ano_academico_id) REFERENCES public.anos_academicos(id);


--
-- TOC entry 5193 (class 2606 OID 20467)
-- Name: periodos_academicos periodos_academicos_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.periodos_academicos
    ADD CONSTRAINT periodos_academicos_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5178 (class 2606 OID 20322)
-- Name: personas personas_tipo_documento_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.personas
    ADD CONSTRAINT personas_tipo_documento_id_fkey FOREIGN KEY (tipo_documento_id) REFERENCES public.tipos_documento(id);


--
-- TOC entry 5181 (class 2606 OID 20363)
-- Name: profesor_colegios profesor_colegios_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profesor_colegios
    ADD CONSTRAINT profesor_colegios_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5182 (class 2606 OID 20358)
-- Name: profesor_colegios profesor_colegios_profesor_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profesor_colegios
    ADD CONSTRAINT profesor_colegios_profesor_id_fkey FOREIGN KEY (profesor_id) REFERENCES public.profesores(id);


--
-- TOC entry 5179 (class 2606 OID 20338)
-- Name: profesores profesores_persona_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profesores
    ADD CONSTRAINT profesores_persona_id_fkey FOREIGN KEY (persona_id) REFERENCES public.personas(id);


--
-- TOC entry 5180 (class 2606 OID 20343)
-- Name: profesores profesores_usuario_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profesores
    ADD CONSTRAINT profesores_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);


--
-- TOC entry 5162 (class 2606 OID 20800)
-- Name: roles roles_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id) ON DELETE CASCADE;


--
-- TOC entry 5199 (class 2606 OID 20517)
-- Name: tipos_evaluacion tipos_evaluacion_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.tipos_evaluacion
    ADD CONSTRAINT tipos_evaluacion_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5163 (class 2606 OID 20182)
-- Name: usuario_roles usuario_roles_colegio_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuario_roles
    ADD CONSTRAINT usuario_roles_colegio_id_fkey FOREIGN KEY (colegio_id) REFERENCES public.colegios(id);


--
-- TOC entry 5164 (class 2606 OID 20806)
-- Name: usuario_roles usuario_roles_rol_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuario_roles
    ADD CONSTRAINT usuario_roles_rol_id_fkey FOREIGN KEY (rol_id) REFERENCES public.roles(id) ON DELETE CASCADE;


--
-- TOC entry 5165 (class 2606 OID 20172)
-- Name: usuario_roles usuario_roles_usuario_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.usuario_roles
    ADD CONSTRAINT usuario_roles_usuario_id_fkey FOREIGN KEY (usuario_id) REFERENCES public.usuarios(id);


-- Completed on 2025-09-01 10:31:25

--
-- PostgreSQL database dump complete
--

