-- =============================================
-- Script: Vistas y Procedimientos para Historial de Transacciones
-- Descripción: Crea vistas que combinan transferencias y pagos de servicios
-- Fecha: 2025-11-22
-- =============================================

USE BancaEnLinea;
GO

-- =============================================
-- Vista: vw_HistorialTransacciones
-- Combina transferencias y pagos en un solo historial
-- =============================================
IF OBJECT_ID('vw_HistorialTransacciones', 'V') IS NOT NULL
    DROP VIEW vw_HistorialTransacciones;
GO

CREATE VIEW vw_HistorialTransacciones
AS
SELECT 
    -- Identificadores
    'TRF-' + CAST(t.Referencia AS VARCHAR(20)) AS Referencia,
    t.Referencia AS IdTransaccion,
    'Transferencia' AS TipoTransaccion,
    'transfer' AS TipoIcono,
  
    -- Fechas
    t.FechaCreacion,
  t.FechaEjecucion,
    
    -- Cliente
    c.Id AS IdCliente,
    c.Nombre + ' ' + c.PrimerApellido + ' ' + c.SegundoApellido AS NombreCliente,
    
    -- Cuenta bancaria
    cb.Id AS IdCuentaBancaria,
    cb.NumeroTarjeta AS NumeroCuenta,
    cb.Tipo AS TipoCuenta,
 cb.Moneda,
    CASE cb.Moneda 
        WHEN 0 THEN 'CRC'
  WHEN 1 THEN 'USD'
        ELSE 'CRC'
    END AS MonedaTexto,
    CASE cb.Moneda
      WHEN 0 THEN '?'
        WHEN 1 THEN '$'
ELSE '?'
    END AS SimboloMoneda,
    
    -- Montos
    t.Monto,
    t.Comision,
    t.MontoTotal,
    
    -- Estado
    t.Estado,
    CASE t.Estado
 WHEN 0 THEN 'Pendiente'
      WHEN 1 THEN 'Programada'
        WHEN 2 THEN 'Exitosa'
   WHEN 3 THEN 'Fallida'
        WHEN 4 THEN 'Cancelada'
        WHEN 5 THEN 'Rechazada'
        ELSE 'Desconocido'
    END AS EstadoTexto,
    
    -- Detalles
    ISNULL(t.Descripcion, 'Transferencia bancaria') AS Descripcion,
    CAST(t.NumeroCuentaDestino AS VARCHAR(20)) AS Destino,
    t.SaldoAnterior,
    t.SaldoPosterior

FROM Transferencia t
INNER JOIN CuentaBancaria cb ON t.IdCuentaBancariaOrigen = cb.Id
INNER JOIN Cuenta c ON cb.IdCuenta = c.Id

UNION ALL

SELECT 
    -- Identificadores
  'PAG-' + CAST(p.IdPago AS VARCHAR(20)) AS Referencia,
    p.IdPago AS IdTransaccion,
    'Pago de Servicio' AS TipoTransaccion,
    'payment' AS TipoIcono,
    
    -- Fechas
    p.FechaCreacion,
p.FechaEjecucion,
    
    -- Cliente
c.Id AS IdCliente,
    c.Nombre + ' ' + c.PrimerApellido + ' ' + c.SegundoApellido AS NombreCliente,
    
  -- Cuenta bancaria - CORREGIDO: Asegurar que siempre devuelva la moneda
    cb.Id AS IdCuentaBancaria,
    cb.NumeroTarjeta AS NumeroCuenta,
    cb.Tipo AS TipoCuenta,
    ISNULL(cb.Moneda, 0) AS Moneda,  -- Por defecto CRC si es NULL
    CASE ISNULL(cb.Moneda, 0)
        WHEN 0 THEN 'CRC'
        WHEN 1 THEN 'USD'
        ELSE 'CRC'
    END AS MonedaTexto,
    CASE ISNULL(cb.Moneda, 0)
      WHEN 0 THEN '?'
        WHEN 1 THEN '$'
        ELSE '?'
  END AS SimboloMoneda,
    
    -- Montos
  p.Monto,
    p.Comision,
    p.MontoTotal,
    
    -- Estado
    p.Estado,
    CASE p.Estado
        WHEN 0 THEN 'Pendiente'
        WHEN 1 THEN 'Programada'
  WHEN 2 THEN 'Exitosa'
        WHEN 3 THEN 'Fallida'
        WHEN 4 THEN 'Cancelada'
    WHEN 5 THEN 'Rechazada'
   ELSE 'Desconocido'
    END AS EstadoTexto,
    
    -- Detalles
    ISNULL(s.Nombre, 'Pago de servicio') AS Descripcion,
    CAST(ISNULL(cs.NumeroContrato, 0) AS VARCHAR(20)) AS Destino,
    NULL AS SaldoAnterior,  -- Los pagos no tienen saldo anterior/posterior
    NULL AS SaldoPosterior

FROM PagoServicio p
INNER JOIN CuentaBancaria cb ON p.IdCuentaBancariaOrigen = cb.Id
INNER JOIN Cuenta c ON cb.IdCuenta = c.Id
LEFT JOIN ContratoServicio cs ON p.IdContratoServicio = cs.IdContratoServicio
LEFT JOIN Servicio s ON cs.IdServicio = s.IdServicio;
GO

-- =============================================
-- Procedimiento: sp_ObtenerHistorialPorCliente
-- Obtiene el historial de transacciones de un cliente
-- =============================================
IF OBJECT_ID('sp_ObtenerHistorialPorCliente', 'P') IS NOT NULL
    DROP PROCEDURE sp_ObtenerHistorialPorCliente;
GO

CREATE PROCEDURE sp_ObtenerHistorialPorCliente
    @IdCliente INT
AS
BEGIN
    SET NOCOUNT ON;
  
    SELECT *
    FROM vw_HistorialTransacciones
    WHERE IdCliente = @IdCliente
    ORDER BY FechaCreacion DESC;
END;
GO

-- =============================================
-- Procedimiento: sp_ObtenerHistorialPorCuentaBancaria
-- Obtiene el historial de transacciones de una cuenta bancaria
-- =============================================
IF OBJECT_ID('sp_ObtenerHistorialPorCuentaBancaria', 'P') IS NOT NULL
    DROP PROCEDURE sp_ObtenerHistorialPorCuentaBancaria;
GO

CREATE PROCEDURE sp_ObtenerHistorialPorCuentaBancaria
    @IdCuentaBancaria INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT *
    FROM vw_HistorialTransacciones
    WHERE IdCuentaBancaria = @IdCuentaBancaria
    ORDER BY FechaCreacion DESC;
END;
GO

-- =============================================
-- Procedimiento: sp_ObtenerHistorialPorFechas
-- Obtiene el historial de transacciones en un rango de fechas
-- =============================================
IF OBJECT_ID('sp_ObtenerHistorialPorFechas', 'P') IS NOT NULL
    DROP PROCEDURE sp_ObtenerHistorialPorFechas;
GO

CREATE PROCEDURE sp_ObtenerHistorialPorFechas
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @IdCliente INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT *
    FROM vw_HistorialTransacciones
    WHERE FechaCreacion BETWEEN @FechaInicio AND @FechaFin
    AND (@IdCliente IS NULL OR IdCliente = @IdCliente)
    ORDER BY FechaCreacion DESC;
END;
GO

-- =============================================
-- Procedimiento: sp_ObtenerExtractoCuenta
-- Obtiene el extracto mensual de una cuenta bancaria
-- =============================================
IF OBJECT_ID('sp_ObtenerExtractoCuenta', 'P') IS NOT NULL
    DROP PROCEDURE sp_ObtenerExtractoCuenta;
GO

CREATE PROCEDURE sp_ObtenerExtractoCuenta
    @IdCuentaBancaria INT,
    @Mes INT,
    @Anio INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @FechaInicio DATETIME = DATEFROMPARTS(@Anio, @Mes, 1);
    DECLARE @FechaFin DATETIME = EOMONTH(@FechaInicio);
    
    -- Información de la cuenta
    SELECT 
        cb.Id,
        cb.NumeroTarjeta,
        cb.Tipo,
      cb.Moneda,
    CASE cb.Moneda 
       WHEN 0 THEN 'CRC'
 WHEN 1 THEN 'USD'
    ELSE 'CRC'
        END AS MonedaTexto,
cb.Saldo AS SaldoActual,
        c.Nombre + ' ' + c.PrimerApellido + ' ' + c.SegundoApellido AS TitularCuenta,
        c.Correo,
        @FechaInicio AS FechaInicio,
   @FechaFin AS FechaFin
    FROM CuentaBancaria cb
    INNER JOIN Cuenta c ON cb.IdCuenta = c.Id
    WHERE cb.Id = @IdCuentaBancaria;
 
    -- Transacciones del mes
  SELECT *
    FROM vw_HistorialTransacciones
    WHERE IdCuentaBancaria = @IdCuentaBancaria
        AND FechaCreacion BETWEEN @FechaInicio AND @FechaFin
  ORDER BY FechaCreacion ASC;
    
    -- Resumen del mes
    SELECT 
        COUNT(*) AS TotalTransacciones,
        SUM(CASE WHEN TipoTransaccion = 'Transferencia' THEN 1 ELSE 0 END) AS TotalTransferencias,
  SUM(CASE WHEN TipoTransaccion = 'Pago de Servicio' THEN 1 ELSE 0 END) AS TotalPagos,
   SUM(Monto) AS TotalDebitos,
      SUM(Comision) AS TotalComisiones,
      SUM(MontoTotal) AS TotalDebitado
    FROM vw_HistorialTransacciones
    WHERE IdCuentaBancaria = @IdCuentaBancaria
        AND FechaCreacion BETWEEN @FechaInicio AND @FechaFin
        AND Estado = 2; -- Solo exitosas
END;
GO

-- =============================================
-- Procedimiento: sp_ObtenerEstadisticasCliente
-- Obtiene estadísticas de transacciones de un cliente
-- =============================================
IF OBJECT_ID('sp_ObtenerEstadisticasCliente', 'P') IS NOT NULL
  DROP PROCEDURE sp_ObtenerEstadisticasCliente;
GO

CREATE PROCEDURE sp_ObtenerEstadisticasCliente
    @IdCliente INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Estadísticas generales
    SELECT 
        COUNT(*) AS TotalTransacciones,
        SUM(CASE WHEN TipoTransaccion = 'Transferencia' THEN 1 ELSE 0 END) AS TotalTransferencias,
        SUM(CASE WHEN TipoTransaccion = 'Pago de Servicio' THEN 1 ELSE 0 END) AS TotalPagos,
      SUM(CASE WHEN Estado = 2 THEN 1 ELSE 0 END) AS TotalExitosas,
        SUM(CASE WHEN Estado = 0 THEN 1 ELSE 0 END) AS TotalPendientes,
        SUM(CASE WHEN Estado = 1 THEN 1 ELSE 0 END) AS TotalProgramadas,
      SUM(CASE WHEN Estado = 4 THEN 1 ELSE 0 END) AS TotalCanceladas,
  SUM(MontoTotal) AS TotalGastado,
        SUM(Comision) AS TotalComisiones
    FROM vw_HistorialTransacciones
    WHERE IdCliente = @IdCliente;
    
    -- Transacciones por mes (últimos 12 meses)
    SELECT 
        YEAR(FechaCreacion) AS Anio,
   MONTH(FechaCreacion) AS Mes,
      COUNT(*) AS Cantidad,
  SUM(MontoTotal) AS MontoTotal
    FROM vw_HistorialTransacciones
    WHERE IdCliente = @IdCliente
      AND FechaCreacion >= DATEADD(MONTH, -12, GETDATE())
    GROUP BY YEAR(FechaCreacion), MONTH(FechaCreacion)
 ORDER BY Anio DESC, Mes DESC;
END;
GO

-- =============================================
-- Índices para mejorar el rendimiento
-- =============================================

-- Índice en Transferencia por FechaCreacion
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transferencia_FechaCreacion')
    CREATE NONCLUSTERED INDEX IX_Transferencia_FechaCreacion
    ON Transferencia(FechaCreacion DESC)
    INCLUDE (IdCuentaBancariaOrigen, Monto, Estado);
GO

-- Índice en PagoServicio por FechaCreacion
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PagoServicio_FechaCreacion')
    CREATE NONCLUSTERED INDEX IX_PagoServicio_FechaCreacion
    ON PagoServicio(FechaCreacion DESC)
    INCLUDE (IdCuentaBancariaOrigen, Monto, Estado);
GO

-- =============================================
-- Pruebas de los procedimientos
-- =============================================

PRINT '? Vista vw_HistorialTransacciones creada exitosamente';
PRINT '? Procedimiento sp_ObtenerHistorialPorCliente creado exitosamente';
PRINT '? Procedimiento sp_ObtenerHistorialPorCuentaBancaria creado exitosamente';
PRINT '? Procedimiento sp_ObtenerHistorialPorFechas creado exitosamente';
PRINT '? Procedimiento sp_ObtenerExtractoCuenta creado exitosamente';
PRINT '? Procedimiento sp_ObtenerEstadisticasCliente creado exitosamente';
PRINT '? Índices creados exitosamente';
PRINT '';
PRINT '?? Ejemplos de uso:';
PRINT '   -- Ver historial de un cliente:';
PRINT '   EXEC sp_ObtenerHistorialPorCliente @IdCliente = 5;';
PRINT '';
PRINT '   -- Ver extracto de una cuenta:';
PRINT '   EXEC sp_ObtenerExtractoCuenta @IdCuentaBancaria = 2, @Mes = 11, @Anio = 2025;';
PRINT '';
PRINT '   -- Ver estadísticas de un cliente:';
PRINT '   EXEC sp_ObtenerEstadisticasCliente @IdCliente = 5;';
PRINT '';
PRINT '   -- Consultar la vista directamente:';
PRINT '   SELECT * FROM vw_HistorialTransacciones ORDER BY FechaCreacion DESC;';
PRINT '';
PRINT '?? NOTA: Los joins en PagoServicio ahora son LEFT JOIN para evitar';
PRINT '   que se pierdan registros si falta ContratoServicio o Servicio.';
PRINT '   La moneda siempre se obtiene de CuentaBancaria con ISNULL para';
PRINT '   garantizar que nunca sea NULL.';

GO
