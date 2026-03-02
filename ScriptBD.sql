CREATE DATABASE IF NOT EXISTS fothelcards_db;
USE fothelcards_db;

-- ==========================================
-- TABLAS (Limpias, sin datos de prueba)
-- ==========================================

DROP TABLE IF EXISTS usuarios;
CREATE TABLE usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    email VARCHAR(100),
    password VARCHAR(255) NOT NULL, 
    rol VARCHAR(50) NOT NULL
);

DROP TABLE IF EXISTS productos;
CREATE TABLE productos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    tipo VARCHAR(50) NOT NULL,
    estado VARCHAR(50) NOT NULL,
    ruta_portada VARCHAR(255)
);

DROP TABLE IF EXISTS configuracion;
CREATE TABLE configuracion (
    id INT PRIMARY KEY DEFAULT 1, -- Forzamos a que el ID sea 1
    resolucion VARCHAR(20) NOT NULL,
    modo_uso VARCHAR(50) NOT NULL,
    volumen DOUBLE NOT NULL,
    silenciar_audio BOOLEAN NOT NULL
);

-- ==========================================
-- PROCEDIMIENTOS ALMACENADOS
-- ==========================================
DELIMITER //

-- === LOGIN Y USUARIOS ===

DROP PROCEDURE IF EXISTS sp_Login //
CREATE PROCEDURE sp_Login(IN p_usuario VARCHAR(50), IN p_password VARCHAR(255))
BEGIN
    SELECT * FROM usuarios WHERE (username = p_usuario OR email = p_usuario) AND password = p_password;
END //

DROP PROCEDURE IF EXISTS sp_ObtenerUsuarios //
CREATE PROCEDURE sp_ObtenerUsuarios()
BEGIN
    SELECT * FROM usuarios;
END //

DROP PROCEDURE IF EXISTS sp_EliminarUsuario //
CREATE PROCEDURE sp_EliminarUsuario(IN p_id INT)
BEGIN
    DELETE FROM usuarios WHERE id = p_id;
END //

DROP PROCEDURE IF EXISTS sp_InsertarUsuario //
CREATE PROCEDURE sp_InsertarUsuario(IN p_usuario VARCHAR(50), IN p_password VARCHAR(255), IN p_rol VARCHAR(50))
BEGIN
    INSERT INTO usuarios (username, password, rol) VALUES (p_usuario, p_password, p_rol);
END //


-- === REGISTRO DE NUEVOS USUARIOS ===

DROP PROCEDURE IF EXISTS sp_VerificarDuplicado //
CREATE PROCEDURE sp_VerificarDuplicado (IN p_usuario VARCHAR(50), IN p_email VARCHAR(100))
BEGIN
    SELECT * FROM usuarios WHERE username = p_usuario OR email = p_email;
END //

DROP PROCEDURE IF EXISTS sp_InsertarUsuarioRegistro //
CREATE PROCEDURE sp_InsertarUsuarioRegistro (IN p_usuario VARCHAR(50), IN p_email VARCHAR(100), IN p_password VARCHAR(255), IN p_rol VARCHAR(50))
BEGIN
    INSERT INTO usuarios (username, email, password, rol) VALUES (p_usuario, p_email, p_password, p_rol);
END //


-- === PRODUCTOS ===

DROP PROCEDURE IF EXISTS sp_ObtenerProductos //
CREATE PROCEDURE sp_ObtenerProductos()
BEGIN
    SELECT * FROM productos;
END //

DROP PROCEDURE IF EXISTS sp_InsertarProducto //
CREATE PROCEDURE sp_InsertarProducto(IN p_nombre VARCHAR(100), IN p_tipo VARCHAR(50), IN p_estado VARCHAR(50), IN p_ruta VARCHAR(255))
BEGIN
    INSERT INTO productos (nombre, tipo, estado, ruta_portada) VALUES (p_nombre, p_tipo, p_estado, p_ruta);
END //

DROP PROCEDURE IF EXISTS sp_ActualizarProducto //
CREATE PROCEDURE sp_ActualizarProducto(IN p_id INT, IN p_estado VARCHAR(50))
BEGIN
    UPDATE productos SET estado = p_estado WHERE id = p_id;
END //

DROP PROCEDURE IF EXISTS sp_EditarProducto //
CREATE PROCEDURE sp_EditarProducto(
    IN p_id INT, 
    IN p_nombre VARCHAR(100), 
    IN p_tipo VARCHAR(50)
)
BEGIN
    UPDATE productos 
    SET nombre = p_nombre, tipo = p_tipo 
    WHERE id = p_id;
END //

DROP PROCEDURE IF EXISTS sp_EliminarProducto //
CREATE PROCEDURE sp_EliminarProducto(
    IN p_id INT
)
BEGIN
    DELETE FROM productos 
    WHERE id = p_id;
END //


DROP PROCEDURE IF EXISTS sp_ObtenerConfiguracion //
CREATE PROCEDURE sp_ObtenerConfiguracion()
BEGIN
    SELECT * FROM configuracion LIMIT 1;
END //

DROP PROCEDURE IF EXISTS sp_GuardarConfiguracion //
CREATE PROCEDURE sp_GuardarConfiguracion(
    IN p_resolucion VARCHAR(20), 
    IN p_modo_uso VARCHAR(50), 
    IN p_volumen DOUBLE, 
    IN p_silenciar_audio BOOLEAN
)
BEGIN
    -- Comprobamos si la fila de configuración ya existe
    IF EXISTS (SELECT 1 FROM configuracion WHERE id = 1) THEN
        -- Si existe, la actualizamos
        UPDATE configuracion 
        SET resolucion = p_resolucion, 
            modo_uso = p_modo_uso, 
            volumen = p_volumen, 
            silenciar_audio = p_silenciar_audio 
        WHERE id = 1;
    ELSE
        -- Si la base de datos está vacía, hacemos el primer INSERT desde el programa
        INSERT INTO configuracion (id, resolucion, modo_uso, volumen, silenciar_audio) 
        VALUES (1, p_resolucion, p_modo_uso, p_volumen, p_silenciar_audio);
    END IF;
END //
DELIMITER ;