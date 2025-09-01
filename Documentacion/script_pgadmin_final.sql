-- ===============================================
-- SCRIPT DE MODIFICACIONES BASE DE DATOS
-- Sistema de Gestión Académica Multi-Colegio
-- Compatible con pgAdmin - Versión Final
-- ===============================================

-- ===============================================
-- 1. MODIFICAR TABLA PERSONAS
-- Agregar segundo nombre y segundo apellido
-- ===============================================

DO $$
BEGIN
    RAISE NOTICE 'Iniciando modificaciones a la tabla personas...';
END $$;

-- Agregar columnas para segundo nombre y segundo apellido
DO $$
BEGIN
    -- Verificar si las columnas ya existen antes de agregarlas
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'personas' AND column_name = 'segundo_nombre') THEN
        ALTER TABLE personas ADD COLUMN segundo_nombre VARCHAR(100);
        RAISE NOTICE 'Columna segundo_nombre agregada a tabla personas';
    ELSE
        RAISE NOTICE 'Columna segundo_nombre ya existe en tabla personas';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'personas' AND column_name = 'segundo_apellido') THEN
        ALTER TABLE personas ADD COLUMN segundo_apellido VARCHAR(100);
        RAISE NOTICE 'Columna segundo_apellido agregada a tabla personas';
    ELSE
        RAISE NOTICE 'Columna segundo_apellido ya existe en tabla personas';
    END IF;
END $$;

-- Actualizar comentarios
COMMENT ON COLUMN personas.segundo_nombre IS 'Segundo nombre de la persona (opcional)';
COMMENT ON COLUMN personas.segundo_apellido IS 'Segundo apellido de la persona (opcional)';
COMMENT ON TABLE personas IS 'Tabla base para todas las personas del sistema. Incluye nombres completos (primer y segundo nombre/apellido) y datos básicos.';

DO $$
BEGIN
    RAISE NOTICE 'Tabla personas modificada exitosamente.';
END $$;

-- ===============================================
-- 2. MODIFICAR TABLA ROLES PARA PERMITIR ROLES GLOBALES
-- ===============================================

DO $$
BEGIN
    RAISE NOTICE 'Modificando tabla roles para permitir roles globales...';
END $$;

-- Eliminar constraints de foreign key existentes de manera segura
DO $$
BEGIN
    -- Eliminar constraint de usuario_roles hacia roles
    IF EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name = 'usuario_roles_rol_id_fkey' AND table_name = 'usuario_roles') THEN
        ALTER TABLE usuario_roles DROP CONSTRAINT usuario_roles_rol_id_fkey;
        RAISE NOTICE 'Constraint usuario_roles_rol_id_fkey eliminado';
    END IF;
    
    -- Eliminar constraint de roles hacia colegios
    IF EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name = 'roles_colegio_id_fkey' AND table_name = 'roles') THEN
        ALTER TABLE roles DROP CONSTRAINT roles_colegio_id_fkey;
        RAISE NOTICE 'Constraint roles_colegio_id_fkey eliminado';
    END IF;
END $$;

-- Permitir colegio_id NULL para roles globales
DO $$
BEGIN
    -- Verificar si la columna ya permite NULL
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'roles' 
        AND column_name = 'colegio_id' 
        AND is_nullable = 'NO'
    ) THEN
        ALTER TABLE roles ALTER COLUMN colegio_id DROP NOT NULL;
        RAISE NOTICE 'Columna colegio_id en tabla roles ahora permite NULL';
    ELSE
        RAISE NOTICE 'Columna colegio_id en tabla roles ya permite NULL';
    END IF;
END $$;

-- Eliminar constraint único anterior si existe
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name = 'roles_colegio_id_codigo_key' AND table_name = 'roles') THEN
        ALTER TABLE roles DROP CONSTRAINT roles_colegio_id_codigo_key;
        RAISE NOTICE 'Constraint único anterior eliminado';
    END IF;
END $$;

-- Crear nuevo constraint único que maneja tanto roles globales como por colegio
DROP INDEX IF EXISTS idx_roles_codigo_contexto;
CREATE UNIQUE INDEX idx_roles_codigo_contexto 
ON roles (COALESCE(colegio_id::text, 'GLOBAL'), codigo);

-- Agregar constraint para validar códigos de roles globales
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.check_constraints WHERE constraint_name = 'check_roles_globales') THEN
        ALTER TABLE roles DROP CONSTRAINT check_roles_globales;
        RAISE NOTICE 'Constraint check_roles_globales anterior eliminado';
    END IF;
    
    ALTER TABLE roles ADD CONSTRAINT check_roles_globales
    CHECK (
        (colegio_id IS NULL AND codigo IN ('ADMIN_GLOBAL', 'SUPER_ADMIN')) 
        OR 
        (colegio_id IS NOT NULL AND codigo NOT IN ('ADMIN_GLOBAL', 'SUPER_ADMIN'))
    );
    RAISE NOTICE 'Constraint check_roles_globales creado';
END $$;

-- Restaurar foreign key con ON DELETE CASCADE de manera segura
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name = 'roles_colegio_id_fkey' AND table_name = 'roles') THEN
        ALTER TABLE roles ADD CONSTRAINT roles_colegio_id_fkey 
        FOREIGN KEY (colegio_id) REFERENCES colegios(id) ON DELETE CASCADE;
        RAISE NOTICE 'Constraint roles_colegio_id_fkey creado';
    ELSE
        RAISE NOTICE 'Constraint roles_colegio_id_fkey ya existe';
    END IF;
END $$;

DO $$
BEGIN
    RAISE NOTICE 'Tabla roles modificada exitosamente.';
END $$;

-- ===============================================
-- 3. MODIFICAR TABLA USUARIO_ROLES PARA ROLES GLOBALES
-- ===============================================

DO $$
BEGIN
    RAISE NOTICE 'Modificando tabla usuario_roles...';
END $$;

-- Permitir colegio_id NULL para asignaciones de roles globales
DO $$
BEGIN
    -- Verificar si la columna ya permite NULL
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'usuario_roles' 
        AND column_name = 'colegio_id' 
        AND is_nullable = 'NO'
    ) THEN
        ALTER TABLE usuario_roles ALTER COLUMN colegio_id DROP NOT NULL;
        RAISE NOTICE 'Columna colegio_id en tabla usuario_roles ahora permite NULL';
    ELSE
        RAISE NOTICE 'Columna colegio_id en tabla usuario_roles ya permite NULL';
    END IF;
END $$;

-- Eliminar constraint único anterior
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name = 'usuario_roles_usuario_id_rol_id_colegio_id_key' AND table_name = 'usuario_roles') THEN
        ALTER TABLE usuario_roles DROP CONSTRAINT usuario_roles_usuario_id_rol_id_colegio_id_key;
        RAISE NOTICE 'Constraint único anterior eliminado de usuario_roles';
    END IF;
END $$;

-- Crear nuevo índice único que maneja roles globales
DROP INDEX IF EXISTS idx_usuario_roles_unico;
CREATE UNIQUE INDEX idx_usuario_roles_unico 
ON usuario_roles (usuario_id, rol_id, COALESCE(colegio_id::text, 'GLOBAL'));

-- NOTA: No agregamos el constraint con subconsultas porque PostgreSQL no lo permite
-- La consistencia se manejará a nivel de aplicación y mediante triggers

-- Restaurar foreign keys de manera segura
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name = 'usuario_roles_rol_id_fkey' AND table_name = 'usuario_roles') THEN
        ALTER TABLE usuario_roles ADD CONSTRAINT usuario_roles_rol_id_fkey 
        FOREIGN KEY (rol_id) REFERENCES roles(id) ON DELETE CASCADE;
        RAISE NOTICE 'Constraint usuario_roles_rol_id_fkey creado';
    ELSE
        RAISE NOTICE 'Constraint usuario_roles_rol_id_fkey ya existe';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.table_constraints WHERE constraint_name = 'usuario_roles_colegio_id_fkey' AND table_name = 'usuario_roles') THEN
        ALTER TABLE usuario_roles ADD CONSTRAINT usuario_roles_colegio_id_fkey 
        FOREIGN KEY (colegio_id) REFERENCES colegios(id) ON DELETE CASCADE;
        RAISE NOTICE 'Constraint usuario_roles_colegio_id_fkey creado';
    ELSE
        RAISE NOTICE 'Constraint usuario_roles_colegio_id_fkey ya existe';
    END IF;
END $$;

DO $$
BEGIN
    RAISE NOTICE 'Tabla usuario_roles modificada exitosamente.';
END $$;

-- ===============================================
-- 4. INSERTAR ROLES GLOBALES PREDETERMINADOS
-- ===============================================

DO $$
BEGIN
    RAISE NOTICE 'Insertando roles globales predeterminados...';
END $$;

-- Insertar rol ADMIN_GLOBAL de manera segura
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM roles WHERE codigo = 'ADMIN_GLOBAL' AND colegio_id IS NULL) THEN
        INSERT INTO roles (id, colegio_id, codigo, nombre, descripcion, permisos, activo) 
        VALUES (
            uuid_generate_v4(),
            NULL, -- Rol global
            'ADMIN_GLOBAL',
            'Administrador Global del Sistema',
            'Acceso completo a todos los colegios y funcionalidades del sistema',
            '{
                "modulos": {
                    "estudiantes": {"ver": true, "crear": true, "editar": true, "eliminar": true, "exportar": true},
                    "profesores": {"ver": true, "crear": true, "editar": true, "eliminar": true, "asignar_materias": true},
                    "calificaciones": {"ver": true, "crear": true, "editar": true, "eliminar": true, "publicar": true, "ver_otras_materias": true},
                    "asistencia": {"ver": true, "registrar": true, "editar": true, "justificar": true, "reportes": true},
                    "financiero": {"ver_facturas": true, "crear_facturas": true, "recibir_pagos": true, "anular_facturas": true, "descuentos": true, "reportes_financieros": true},
                    "reportes": {"boletines": true, "certificados": true, "listas_estudiantes": true, "reportes_academicos": true, "reportes_financieros": true, "reportes_disciplinarios": true},
                    "comunicaciones": {"enviar_notificaciones": true, "mensajes_acudientes": true, "mensajes_masivos": true, "crear_circulares": true},
                    "administracion": {"gestionar_usuarios": true, "configurar_colegio": true, "gestionar_roles": true, "logs_auditoria": true, "backup_restaurar": true}
                },
                "restricciones": {
                    "solo_sus_estudiantes": false,
                    "solo_sus_materias": false,
                    "solo_su_grupo": false,
                    "horario_acceso": {"activo": false},
                    "ip_permitidas": [],
                    "solo_horario_laboral": false
                },
                "configuracion_especial": {
                    "puede_ver_notas_finales": true,
                    "puede_editar_despues_publicacion": true,
                    "requiere_autorizacion_cambios": false,
                    "notificar_cambios_importantes": false,
                    "limite_estudiantes_consulta": 0,
                    "acceso_todos_colegios": true,
                    "puede_crear_colegios": true,
                    "puede_eliminar_colegios": true,
                    "gestiona_licencias": true
                }
            }'::jsonb,
            true
        );
        RAISE NOTICE 'Rol ADMIN_GLOBAL insertado exitosamente';
    ELSE
        RAISE NOTICE 'Rol ADMIN_GLOBAL ya existe';
    END IF;
END $$;

-- Insertar rol SUPER_ADMIN de manera segura
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM roles WHERE codigo = 'SUPER_ADMIN' AND colegio_id IS NULL) THEN
        INSERT INTO roles (id, colegio_id, codigo, nombre, descripcion, permisos, activo) 
        VALUES (
            uuid_generate_v4(),
            NULL, -- Rol global
            'SUPER_ADMIN',
            'Super Administrador',
            'Acceso de desarrollador con permisos de sistema y base de datos',
            '{
                "modulos": {
                    "estudiantes": {"ver": true, "crear": true, "editar": true, "eliminar": true, "exportar": true},
                    "profesores": {"ver": true, "crear": true, "editar": true, "eliminar": true, "asignar_materias": true},
                    "calificaciones": {"ver": true, "crear": true, "editar": true, "eliminar": true, "publicar": true, "ver_otras_materias": true},
                    "asistencia": {"ver": true, "registrar": true, "editar": true, "justificar": true, "reportes": true},
                    "financiero": {"ver_facturas": true, "crear_facturas": true, "recibir_pagos": true, "anular_facturas": true, "descuentos": true, "reportes_financieros": true},
                    "reportes": {"boletines": true, "certificados": true, "listas_estudiantes": true, "reportes_academicos": true, "reportes_financieros": true, "reportes_disciplinarios": true},
                    "comunicaciones": {"enviar_notificaciones": true, "mensajes_acudientes": true, "mensajes_masivos": true, "crear_circulares": true},
                    "administracion": {"gestionar_usuarios": true, "configurar_colegio": true, "gestionar_roles": true, "logs_auditoria": true, "backup_restaurar": true},
                    "sistema": {"acceso_base_datos": true, "ejecutar_scripts": true, "ver_logs_sistema": true, "configurar_servidor": true, "gestionar_backups": true}
                },
                "restricciones": {
                    "solo_sus_estudiantes": false,
                    "solo_sus_materias": false,
                    "solo_su_grupo": false,
                    "horario_acceso": {"activo": false},
                    "ip_permitidas": [],
                    "solo_horario_laboral": false
                },
                "configuracion_especial": {
                    "puede_ver_notas_finales": true,
                    "puede_editar_despues_publicacion": true,
                    "requiere_autorizacion_cambios": false,
                    "notificar_cambios_importantes": false,
                    "limite_estudiantes_consulta": 0,
                    "acceso_todos_colegios": true,
                    "puede_crear_colegios": true,
                    "puede_eliminar_colegios": true,
                    "gestiona_licencias": true,
                    "acceso_sistema_completo": true,
                    "puede_ejecutar_sql": true
                }
            }'::jsonb,
            true
        );
        RAISE NOTICE 'Rol SUPER_ADMIN insertado exitosamente';
    ELSE
        RAISE NOTICE 'Rol SUPER_ADMIN ya existe';
    END IF;
END $$;

DO $$
BEGIN
    RAISE NOTICE 'Roles globales procesados exitosamente.';
END $$;

-- ===============================================
-- 5. CREAR FUNCIONES AUXILIARES
-- ===============================================

DO $$
BEGIN
    RAISE NOTICE 'Creando funciones auxiliares...';
END $$;

-- Función para verificar si un usuario es admin global
CREATE OR REPLACE FUNCTION es_admin_global(usuario_uuid UUID)
RETURNS BOOLEAN AS $$
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
$$ LANGUAGE plpgsql;

-- Función para obtener colegios accesibles por un usuario
CREATE OR REPLACE FUNCTION obtener_colegios_usuario(usuario_uuid UUID)
RETURNS TABLE(colegio_id UUID, nombre_colegio VARCHAR) AS $$
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
$$ LANGUAGE plpgsql;

-- Función para verificar permisos específicos
CREATE OR REPLACE FUNCTION tiene_permiso_usuario(
    usuario_uuid UUID, 
    colegio_uuid UUID, 
    modulo_nombre VARCHAR, 
    accion_nombre VARCHAR
)
RETURNS BOOLEAN AS $$
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
$$ LANGUAGE plpgsql;

-- Función para validar consistencia de roles y colegios
CREATE OR REPLACE FUNCTION validar_consistencia_usuario_rol(
    p_usuario_id UUID,
    p_rol_id UUID,
    p_colegio_id UUID
) RETURNS BOOLEAN AS $$
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
$$ LANGUAGE plpgsql;

DO $$
BEGIN
    RAISE NOTICE 'Funciones auxiliares creadas exitosamente.';
END $$;

-- ===============================================
-- 6. CREAR VISTAS ÚTILES
-- ===============================================

DO $$
BEGIN
    RAISE NOTICE 'Creando vistas auxiliares...';
END $$;

-- Vista para usuarios con sus roles y colegios
CREATE OR REPLACE VIEW vista_usuarios_roles AS
SELECT 
    u.id as usuario_id,
    u.username,
    u.email,
    ur.id as usuario_rol_id,
    r.id as rol_id,
    r.codigo as rol_codigo,
    r.nombre as rol_nombre,
    ur.colegio_id,
    c.nombre as colegio_nombre,
    CASE 
        WHEN ur.colegio_id IS NULL THEN 'GLOBAL'
        ELSE 'COLEGIO'
    END as tipo_rol,
    ur.activo as rol_activo,
    ur.fecha_asignacion
FROM usuarios u
JOIN usuario_roles ur ON u.id = ur.usuario_id
JOIN roles r ON ur.rol_id = r.id
LEFT JOIN colegios c ON ur.colegio_id = c.id
WHERE u.activo = true;

-- Vista para roles globales
CREATE OR REPLACE VIEW vista_roles_globales AS
SELECT 
    id,
    codigo,
    nombre,
    descripcion,
    permisos,
    activo,
    'GLOBAL' as ambito
FROM roles
WHERE colegio_id IS NULL;

-- Vista para roles por colegio
CREATE OR REPLACE VIEW vista_roles_colegio AS
SELECT 
    r.id,
    r.codigo,
    r.nombre,
    r.descripcion,
    r.permisos,
    r.activo,
    r.colegio_id,
    c.nombre as colegio_nombre,
    'COLEGIO' as ambito
FROM roles r
JOIN colegios c ON r.colegio_id = c.id
WHERE r.colegio_id IS NOT NULL;

DO $$
BEGIN
    RAISE NOTICE 'Vistas auxiliares creadas exitosamente.';
END $$;

-- ===============================================
-- 7. CREAR TRIGGERS DE VALIDACIÓN
-- ===============================================

DO $$
BEGIN
    RAISE NOTICE 'Creando triggers de validación...';
END $$;

-- Trigger para validar roles globales vs por colegio
CREATE OR REPLACE FUNCTION validar_rol_contexto()
RETURNS TRIGGER AS $$
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
$$ LANGUAGE plpgsql;

-- Trigger para validar asignación de usuario_roles
CREATE OR REPLACE FUNCTION validar_usuario_rol_asignacion()
RETURNS TRIGGER AS $$
BEGIN
    -- Usar la función de validación
    IF NOT validar_consistencia_usuario_rol(NEW.usuario_id, NEW.rol_id, NEW.colegio_id) THEN
        RAISE EXCEPTION 'Inconsistencia en asignación de rol: el rol y colegio no son compatibles';
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Aplicar triggers
DROP TRIGGER IF EXISTS trigger_validar_rol_contexto ON roles;
CREATE TRIGGER trigger_validar_rol_contexto
    BEFORE INSERT OR UPDATE ON roles
    FOR EACH ROW EXECUTE FUNCTION validar_rol_contexto();

DROP TRIGGER IF EXISTS trigger_validar_usuario_rol_asignacion ON usuario_roles;
CREATE TRIGGER trigger_validar_usuario_rol_asignacion
    BEFORE INSERT OR UPDATE ON usuario_roles
    FOR EACH ROW EXECUTE FUNCTION validar_usuario_rol_asignacion();

DO $$
BEGIN
    RAISE NOTICE 'Triggers de validación creados exitosamente.';
END $$;

-- ===============================================
-- 8. ACTUALIZAR ÍNDICES EXISTENTES
-- ===============================================

DO $$
BEGIN
    RAISE NOTICE 'Actualizando índices...';
END $$;

-- Índices para la tabla personas (con nuevos campos)
CREATE INDEX IF NOT EXISTS idx_personas_nombre_completo ON personas 
USING GIN (to_tsvector('spanish', nombres || ' ' || COALESCE(segundo_nombre, '') || ' ' || apellidos || ' ' || COALESCE(segundo_apellido, '')));

-- Índices para roles globales
CREATE INDEX IF NOT EXISTS idx_roles_globales ON roles (codigo) WHERE colegio_id IS NULL;
CREATE INDEX IF NOT EXISTS idx_roles_colegio ON roles (colegio_id, codigo) WHERE colegio_id IS NOT NULL;

-- Índices para usuario_roles con roles globales
CREATE INDEX IF NOT EXISTS idx_usuario_roles_globales ON usuario_roles (usuario_id, rol_id) WHERE colegio_id IS NULL;
CREATE INDEX IF NOT EXISTS idx_usuario_roles_colegio ON usuario_roles (usuario_id, colegio_id) WHERE colegio_id IS NOT NULL;

DO $$
BEGIN
    RAISE NOTICE 'Índices actualizados exitosamente.';
END $$;

-- ===============================================
-- 9. COMENTARIOS FINALES Y DOCUMENTACIÓN
-- ===============================================

-- Actualizar comentarios en las tablas modificadas
COMMENT ON TABLE roles IS 'Roles del sistema. Puede ser global (colegio_id IS NULL) para ADMIN_GLOBAL y SUPER_ADMIN, o específico por colegio.';
COMMENT ON COLUMN roles.colegio_id IS 'ID del colegio. NULL para roles globales (ADMIN_GLOBAL, SUPER_ADMIN).';

COMMENT ON TABLE usuario_roles IS 'Asignación de roles a usuarios. Para roles globales, colegio_id es NULL. La consistencia se valida mediante triggers.';
COMMENT ON COLUMN usuario_roles.colegio_id IS 'ID del colegio. NULL para asignaciones de roles globales.';

-- Comentarios en funciones
COMMENT ON FUNCTION es_admin_global(UUID) IS 'Verifica si un usuario tiene rol de administrador global (ADMIN_GLOBAL o SUPER_ADMIN).';
COMMENT ON FUNCTION obtener_colegios_usuario(UUID) IS 'Obtiene todos los colegios a los que un usuario tiene acceso. Admin globales ven todos los colegios.';
COMMENT ON FUNCTION tiene_permiso_usuario(UUID, UUID, VARCHAR, VARCHAR) IS 'Verifica si un usuario tiene un permiso específico en un colegio. Admin globales tienen todos los permisos.';
COMMENT ON FUNCTION validar_consistencia_usuario_rol(UUID, UUID, UUID) IS 'Valida que la asignación de rol y colegio sea consistente.';

-- ===============================================
-- 10. VERIFICACIONES FINALES
-- ===============================================

DO $$
BEGIN
    RAISE NOTICE 'Ejecutando verificaciones finales...';
END $$;

-- Verificar que los roles globales se insertaron correctamente
DO $$
DECLARE
    count_roles_globales INTEGER;
BEGIN
    SELECT COUNT(*) INTO count_roles_globales 
    FROM roles 
    WHERE colegio_id IS NULL AND codigo IN ('ADMIN_GLOBAL', 'SUPER_ADMIN');
    
    IF count_roles_globales >= 2 THEN
        RAISE NOTICE 'Verificación exitosa: Se encontraron % roles globales', count_roles_globales;
    ELSE
        RAISE WARNING 'Advertencia: Se esperaban al menos 2 roles globales, pero se encontraron %', count_roles_globales;
    END IF;
END $$;

-- Verificar que las columnas se agregaron a personas
DO $$
DECLARE
    count_columnas INTEGER;
BEGIN
    SELECT COUNT(*) INTO count_columnas
    FROM information_schema.columns 
    WHERE table_name = 'personas' 
    AND column_name IN ('segundo_nombre', 'segundo_apellido')
    AND table_schema = 'public';
    
    IF count_columnas >= 2 THEN
        RAISE NOTICE 'Verificación exitosa: Columnas segundo_nombre y segundo_apellido presentes en tabla personas';
    ELSE
        RAISE WARNING 'Advertencia: No se encontraron todas las columnas esperadas en tabla personas';
    END IF;
END $$;

-- Verificar que los triggers se crearon correctamente
DO $$
DECLARE
    count_triggers INTEGER;
BEGIN
    SELECT COUNT(*) INTO count_triggers 
    FROM information_schema.triggers 
    WHERE trigger_name IN ('trigger_validar_rol_contexto', 'trigger_validar_usuario_rol_asignacion');
    
    IF count_triggers >= 2 THEN
        RAISE NOTICE 'Verificación exitosa: Triggers de validación creados correctamente';
    ELSE
        RAISE WARNING 'Advertencia: No se crearon todos los triggers esperados. Encontrados: %', count_triggers;
    END IF;
END $$;

-- Mensaje final de éxito
DO $$
BEGIN
    RAISE NOTICE '==========================================';
    RAISE NOTICE 'MODIFICACIONES COMPLETADAS EXITOSAMENTE';
    RAISE NOTICE '==========================================';
    RAISE NOTICE '';
    RAISE NOTICE 'Resumen de cambios aplicados:';
    RAISE NOTICE '1. ✓ Tabla personas: Agregadas columnas segundo_nombre y segundo_apellido';
    RAISE NOTICE '2. ✓ Tabla roles: Modificada para permitir roles globales (colegio_id NULL)';
    RAISE NOTICE '3. ✓ Tabla usuario_roles: Modificada para asignaciones de roles globales';
    RAISE NOTICE '4. ✓ Roles globales: ADMIN_GLOBAL y SUPER_ADMIN procesados';
    RAISE NOTICE '5. ✓ Funciones auxiliares: es_admin_global, obtener_colegios_usuario, tiene_permiso_usuario';
    RAISE NOTICE '6. ✓ Vistas: vista_usuarios_roles, vista_roles_globales, vista_roles_colegio';
    RAISE NOTICE '7. ✓ Triggers: Validación de contexto de roles y asignaciones';
    RAISE NOTICE '8. ✓ Índices: Actualizados para optimizar consultas';
    RAISE NOTICE '';
    RAISE NOTICE 'IMPORTANTE: La consistencia entre roles y colegios se valida mediante triggers.';
    RAISE NOTICE 'No se usaron CHECK constraints con subconsultas para mantener compatibilidad.';
    RAISE NOTICE '';
    RAISE NOTICE 'La base de datos está lista para manejar usuarios con roles globales.';
    RAISE NOTICE 'Siguiente paso: Crear el primer usuario ADMIN_GLOBAL.';
END $$;