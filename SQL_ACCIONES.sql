-- =============================================
-- Script: Crear tabla Accion para registro de actividades
-- Descripción: Tabla simple para registrar todas las acciones del sistema
-- Fecha: 2025-11-22
-- =============================================

USE BancaEnLinea;
GO

-- =============================================
-- Tabla: Accion
-- Registra todas las acciones realizadas en el sistema
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Accion')
BEGIN
    CREATE TABLE Accion
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Fecha DATETIME NOT NULL DEFAULT GETDATE(),
      Descripcion NVARCHAR(500) NOT NULL,
        IdUsuario INT NULL,
        NombreUsuario NVARCHAR(200) NULL
    );

  PRINT '? Tabla Accion creada exitosamente';
END
ELSE
BEGIN
    PRINT '?? La tabla Accion ya existe';
END
GO

-- Crear índice para mejorar las consultas por fecha
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Accion_Fecha')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Accion_Fecha
    ON Accion(Fecha DESC);
    
    PRINT '? Índice IX_Accion_Fecha creado exitosamente';
END
GO

-- Insertar datos de ejemplo
INSERT INTO Accion (Fecha, Descripcion, IdUsuario, NombreUsuario)
VALUES 
    (GETDATE(), 'Sistema iniciado', NULL, NULL),
    (GETDATE(), 'Se creó una cuenta de usuario', 5, 'Melissa Quijano'),
    (GETDATE(), 'Se realizó una transferencia de $100', 5, 'Melissa Quijano'),
    (GETDATE(), 'Se registró un pago de servicio de electricidad', 5, 'Melissa Quijano');

PRINT '? Datos de ejemplo insertados';
PRINT '';
PRINT '?? Para ver las acciones registradas:';
PRINT '   SELECT * FROM Accion ORDER BY Fecha DESC;';
PRINT '';
PRINT '?? Ejemplos de uso desde la API:';
PRINT '   POST /Acciones/RegistrarAccion';
PRINT '   GET /Acciones/ObtenerAcciones';

GO
