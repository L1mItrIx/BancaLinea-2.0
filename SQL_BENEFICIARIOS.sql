-- QUERIES SQL PARA TABLA BENEFICIARIO (ACTUALIZADO)
-- Beneficiarios están relacionados con Cliente (Cuenta), NO con CuentaBancaria
-- Un cliente puede tener MÁXIMO 3 beneficiarios

-- CREAR LA TABLA BENEFICIARIO
IF OBJECT_ID('dbo.Beneficiario', 'U') IS NOT NULL
    DROP TABLE dbo.Beneficiario;
GO

CREATE TABLE [dbo].[Beneficiario] (
    [IdBeneficiario] INT IDENTITY(1,1) NOT NULL,
    [IdCuenta] INT NOT NULL,  -- FK hacia Cuenta (Cliente)
    [Alias] NVARCHAR(30) NOT NULL,
    [Banco] NVARCHAR(100) NOT NULL,
  [Moneda] INT NOT NULL,
    [NumeroCuentaDestino] BIGINT NOT NULL,
    [Pais] NVARCHAR(50) NOT NULL,
    
    CONSTRAINT [PK_Beneficiario] PRIMARY KEY CLUSTERED ([IdBeneficiario] ASC),
    CONSTRAINT [FK_Beneficiario_Cuenta] FOREIGN KEY ([IdCuenta]) 
        REFERENCES [dbo].[Cuenta]([Id]) 
    ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_Beneficiario_IdCuenta] 
ON [dbo].[Beneficiario] ([IdCuenta]);
GO

PRINT '? Tabla Beneficiario creada exitosamente.';
GO
