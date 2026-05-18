-- =============================================
-- LIMPIAR TABLAS (en orden correcto)
-- =============================================
TRUNCATE TABLE resenas CASCADE;
TRUNCATE TABLE reservas CASCADE;
TRUNCATE TABLE horariosdisponibles CASCADE;
TRUNCATE TABLE servicios CASCADE;
TRUNCATE TABLE categorias CASCADE;
TRUNCATE TABLE usuarios CASCADE;

-- =============================================
-- REINICIAR SECUENCIAS
-- =============================================
ALTER SEQUENCE categorias_id_seq RESTART WITH 1;
ALTER SEQUENCE servicios_id_seq RESTART WITH 1;
ALTER SEQUENCE usuarios_id_seq RESTART WITH 1;
ALTER SEQUENCE reservas_id_seq RESTART WITH 1;
ALTER SEQUENCE resenas_id_seq RESTART WITH 1;
ALTER SEQUENCE horariosdisponibles_id_seq RESTART WITH 1;

-- =============================================
-- INSERTAR USUARIOS
-- =============================================
-- Contraseñas: "Admin123" para todos (hashed con BCrypt)
INSERT INTO usuarios (nombre, apellido, email, passwordhash, rol, estado, telefono, direccion, fecharegistro) 
VALUES 
('Admin', 'Sistema', 'admin@marketplace.com', '$2a$11$K8HqKp9qX8qY8qY8qY8qYu', 'Admin', true, '999999999', 'Av. Principal 123', CURRENT_TIMESTAMP),
('Juan', 'Perez', 'juan@example.com', '$2a$11$K8HqKp9qX8qY8qY8qY8qYu', 'Prestador', true, '987654321', 'Calle Las Flores 456', CURRENT_TIMESTAMP),
('Maria', 'Lopez', 'maria@example.com', '$2a$11$K8HqKp9qX8qY8qY8qY8qYu', 'Cliente', true, '976543210', 'Jr. Los Alamos 789', CURRENT_TIMESTAMP);

-- =============================================
-- INSERTAR CATEGORÍAS
-- =============================================
INSERT INTO categorias (nombre, descripcion, estado) 
VALUES 
('Limpieza', 'Servicios de limpieza general del hogar', true),
('Mantenimiento', 'Reparaciones y mantenimiento del hogar', true),
('Belleza', 'Peluquería, maquillaje y spa', true),
('Tecnología', 'Reparación de dispositivos electrónicos', true),
('Jardinería', 'Cuidado de jardines y áreas verdes', true),
('Fotografía', 'Fotografía profesional para eventos', true),
('Mascotas', 'Paseo y cuidado de mascotas', true);

-- =============================================
-- INSERTAR SERVICIOS
-- =============================================
INSERT INTO servicios (nombre, descripcion, precio, duracionminutos, categoriaid, usuarioid, estado, visitas, maxreservasporia, fechacreacion) 
VALUES 
('Limpieza de Casa', 'Limpieza profunda de toda la casa', 150.00, 180, 1, 2, true, 0, 5, CURRENT_TIMESTAMP),
('Reparación de PC', 'Diagnóstico y reparación de computadoras', 80.00, 60, 4, 2, true, 0, 10, CURRENT_TIMESTAMP),
('Corte de Cabello', 'Corte moderno para damas y caballeros', 40.00, 45, 3, 2, true, 0, 8, CURRENT_TIMESTAMP),
('Poda de Jardín', 'Poda y mantenimiento de jardines', 100.00, 120, 5, 2, true, 0, 4, CURRENT_TIMESTAMP),
('Mantenimiento de PC', 'Mantenimiento preventivo y limpieza interna', 60.00, 45, 4, 2, true, 0, 10, CURRENT_TIMESTAMP),
('Baño de Mascotas', 'Baño completo para perros y gatos', 50.00, 60, 7, 2, true, 0, 6, CURRENT_TIMESTAMP),
('Fotografía de Eventos', 'Fotografía profesional para bodas y eventos', 300.00, 240, 6, 2, true, 0, 3, CURRENT_TIMESTAMP);

-- =============================================
-- INSERTAR HORARIOS DISPONIBLES
-- =============================================
INSERT INTO horariosdisponibles (usuarioid, diasemana, horainicio, horafin) 
VALUES 
(2, 1, '09:00:00', '13:00:00'),
(2, 1, '14:00:00', '18:00:00'),
(2, 2, '09:00:00', '13:00:00'),
(2, 2, '14:00:00', '18:00:00'),
(2, 3, '09:00:00', '13:00:00'),
(2, 3, '14:00:00', '18:00:00'),
(2, 4, '09:00:00', '13:00:00'),
(2, 4, '14:00:00', '18:00:00'),
(2, 5, '09:00:00', '13:00:00'),
(2, 5, '14:00:00', '18:00:00');

-- =============================================
-- INSERTAR RESERVAS
-- =============================================
INSERT INTO reservas (usuarioid, servicioid, fechareserva, horainicio, horafin, estado, notas, preciofinal, fechacreacion) 
VALUES 
(3, 1, CURRENT_DATE + INTERVAL '1 day', '10:00:00', '13:00:00', 'Confirmada', 'Llevar productos de limpieza', 150.00, CURRENT_TIMESTAMP),
(3, 2, CURRENT_DATE + INTERVAL '2 days', '14:00:00', '15:00:00', 'Pendiente', 'Computadora no enciende', 80.00, CURRENT_TIMESTAMP),
(3, 3, CURRENT_DATE + INTERVAL '3 days', '11:00:00', '11:45:00', 'Confirmada', NULL, 40.00, CURRENT_TIMESTAMP),
(3, 1, CURRENT_DATE + INTERVAL '4 days', '15:00:00', '18:00:00', 'Pendiente', 'Traer productos ecológicos', 150.00, CURRENT_TIMESTAMP);

-- =============================================
-- INSERTAR RESEÑAS
-- =============================================
INSERT INTO resenas (usuarioid, servicioid, calificacion, comentario, fecha) 
VALUES 
(3, 1, 5, 'Excelente servicio, muy profesional. Limpiaron todo perfectamente.', CURRENT_TIMESTAMP),
(3, 2, 4, 'Buen trabajo, repararon mi PC rápidamente. Un poco caro pero vale la pena.', CURRENT_TIMESTAMP),
(3, 3, 5, 'Me encantó el corte, volveré definitivamente. Muy atento el profesional.', CURRENT_TIMESTAMP);
