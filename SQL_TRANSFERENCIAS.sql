-- ============================================
-- SCRIPT SQL PARA TABLA TRANSFERENCIA (SIMPLIFICADO)
-- ============================================

-- ELIMINAR TABLA SI EXISTE
IF OBJECT_ID('dbo.Transferencia', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Transferencia;
    PRINT '?? Tabla Transferencia eliminada.';
END
GO

-- CREAR LA TABLA TRANSFERENCIA
CREATE TABLE [dbo].[Transferencia] (
    [Referencia] INT IDENTITY(1,1) NOT NULL,
    [IdempotencyKey] NVARCHAR(100) NOT NULL,
    [IdCuentaBancariaOrigen] INT NOT NULL,
    [NumeroCuentaDestino] BIGINT NOT NULL,
    [Monto] BIGINT NOT NULL,
    [Comision] BIGINT NOT NULL DEFAULT 500,
    [MontoTotal] BIGINT NOT NULL,
    [SaldoAnterior] BIGINT NOT NULL,
   [SaldoPosterior] BIGINT NOT NULL,
  [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
    [FechaEjecucion] DATETIME NOT NULL,
    [Estado] INT NOT NULL,
    [Descripcion] NVARCHAR(500) NULL,
    [IdAprobador] INT NULL,
    
    CONSTRAINT [PK_Transferencia] PRIMARY KEY CLUSTERED ([Referencia] ASC),
  
    CONSTRAINT [FK_Transferencia_CuentaBancaria] FOREIGN KEY ([IdCuentaBancariaOrigen]) 
        REFERENCES [dbo].[CuentaBancaria]([Id]) 
        ON DELETE NO ACTION,
 
    CONSTRAINT [FK_Transferencia_Aprobador] FOREIGN KEY ([IdAprobador]) 
        REFERENCES [dbo].[Cuenta]([Id]) 
        ON DELETE NO ACTION,
    
    CONSTRAINT [UQ_Transferencia_IdempotencyKey] UNIQUE ([IdempotencyKey])
);
GO

-- Índices
CREATE NONCLUSTERED INDEX [IX_Transferencia_IdCuentaBancariaOrigen] 
ON [dbo].[Transferencia] ([IdCuentaBancariaOrigen]);
GO

CREATE NONCLUSTERED INDEX [IX_Transferencia_Estado] 
ON [dbo].[Transferencia] ([Estado]);
GO

CREATE NONCLUSTERED INDEX [IX_Transferencia_FechaEjecucion] 
ON [dbo].[Transferencia] ([FechaEjecucion]);
GO

PRINT '? Tabla Transferencia creada exitosamente.';
GO

-- CONSULTAS ÚTILES
-- Ver todas las transferencias con información completa
SELECT 
    t.Referencia,
    t.IdempotencyKey,
    cb.NumeroTarjeta AS 'Tarjeta Origen',
    c.Nombre + ' ' + c.PrimerApellido AS 'Cliente',
    t.NumeroCuentaDestino AS 'Cuenta Destino',
    t.Monto,
    t.Comision,
    t.MontoTotal,
  t.SaldoAnterior,
    t.SaldoPosterior,
    t.FechaCreacion,
    t.FechaEjecucion,
    CASE t.Estado
  WHEN 0 THEN 'Pendiente'
        WHEN 1 THEN 'Programada'
        WHEN 2 THEN 'Exitosa'
        WHEN 3 THEN 'Fallida'
  WHEN 4 THEN 'Cancelada'
        WHEN 5 THEN 'Rechazada'
    END AS 'Estado',
    a.Nombre + ' ' + a.PrimerApellido AS 'Aprobador'
FROM [dbo].[Transferencia] t
INNER JOIN [dbo].[CuentaBancaria] cb ON t.IdCuentaBancariaOrigen = cb.Id
INNER JOIN [dbo].[Cuenta] c ON cb.IdCuenta = c.Id
LEFT JOIN [dbo].[Cuenta] a ON t.IdAprobador = a.Id
ORDER BY t.FechaCreacion DESC;
GO

-- Transferencias pendientes de aprobación
SELECT 
    t.Referencia,
    cb.NumeroTarjeta,
    c.Nombre + ' ' + c.PrimerApellido AS 'Cliente',
    t.Monto,
    t.MontoTotal,
t.FechaCreacion
FROM [dbo].[Transferencia] t
INNER JOIN [dbo].[CuentaBancaria] cb ON t.IdCuentaBancariaOrigen = cb.Id
INNER JOIN [dbo].[Cuenta] c ON cb.IdCuenta = c.Id
WHERE t.Estado = 0
ORDER BY t.FechaCreacion;
GO

/*
ESTADOS DE TRANSFERENCIA:
0 = Pendiente (esperando aprobación)
1 = Programada (para ejecutar a futuro)
2 = Exitosa
3 = Fallida
4 = Cancelada
5 = Rechazada

REGLAS DE NEGOCIO:
- Comisión fija: ?500
- Umbral aprobación: ?100,000
- Límite diario: ?500,000
- Máx programación: 90 días
- Cancelación: 24 horas antes
*/
