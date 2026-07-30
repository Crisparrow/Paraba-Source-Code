USE ParabaDB;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.Vehiculos', 'EstadoVerificacion') IS NULL
BEGIN
    ALTER TABLE dbo.Vehiculos
    ADD EstadoVerificacion VARCHAR(30) NOT NULL
        CONSTRAINT DF_Vehiculos_EstadoVerificacion DEFAULT 'Pendiente';
END;
GO

UPDATE dbo.Vehiculos
SET EstadoVerificacion = 'Aprobado'
WHERE Verificado = 1 AND EstadoVerificacion = 'Pendiente';
GO

IF COL_LENGTH('dbo.Vehiculos', 'Observacion') IS NULL
BEGIN
    ALTER TABLE dbo.Vehiculos
    ADD Observacion VARCHAR(300) NOT NULL
        CONSTRAINT DF_Vehiculos_Observacion DEFAULT '';
END;
GO

IF COL_LENGTH('dbo.DocumentosConductor', 'EsVigente') IS NULL
BEGIN
    ALTER TABLE dbo.DocumentosConductor
    ADD EsVigente BIT NOT NULL
        CONSTRAINT DF_DocumentosConductor_EsVigente DEFAULT 1;
END;
GO

ALTER TABLE dbo.DocumentosConductor ALTER COLUMN UrlArchivo VARCHAR(500) NOT NULL;
GO

IF COL_LENGTH('dbo.TiposServicio', 'CategoriaVehiculo') IS NULL
BEGIN
    ALTER TABLE dbo.TiposServicio
    ADD CategoriaVehiculo VARCHAR(30) NOT NULL
        CONSTRAINT DF_TiposServicio_CategoriaVehiculo DEFAULT 'Taxi';
END;
GO

UPDATE dbo.TiposServicio
SET CategoriaVehiculo = CASE
    WHEN LOWER(Nombre) LIKE '%moto%' THEN 'Mototaxi'
    WHEN LOWER(Nombre) LIKE '%micro%' THEN 'Microbus'
    ELSE 'Taxi'
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.TiposServicio WHERE LOWER(Nombre) = 'microbus')
BEGIN
    INSERT INTO dbo.TiposServicio (Nombre, Estado, CategoriaVehiculo)
    VALUES ('Microbus', 1, 'Microbus');
END;
GO

;WITH Duplicados AS
(
    SELECT IdDocumentoConductor,
           ROW_NUMBER() OVER
           (
               PARTITION BY IdConductor, TipoDocumento
               ORDER BY FechaRegistro DESC, IdDocumentoConductor DESC
           ) AS NumeroFila
    FROM dbo.DocumentosConductor
    WHERE EsVigente = 1
)
UPDATE d
SET EsVigente = 0
FROM dbo.DocumentosConductor d
INNER JOIN Duplicados x ON x.IdDocumentoConductor = d.IdDocumentoConductor
WHERE x.NumeroFila > 1;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = 'UX_DocumentosConductor_Vigente'
      AND object_id = OBJECT_ID('dbo.DocumentosConductor')
)
BEGIN
    CREATE UNIQUE INDEX UX_DocumentosConductor_Vigente
        ON dbo.DocumentosConductor (IdConductor, TipoDocumento)
        WHERE EsVigente = 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_TiposServicio_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdTipoServicio, Nombre, CategoriaVehiculo, Estado
    FROM dbo.TiposServicio
    ORDER BY CASE CategoriaVehiculo WHEN 'Taxi' THEN 1 WHEN 'Mototaxi' THEN 2 WHEN 'Microbus' THEN 3 ELSE 4 END,
             Nombre;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Vehiculos_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdVehiculo, IdConductor, IdTipoServicio, Placa, Marca, Modelo, Color, Anio,
           Verificado, EstadoVerificacion, Observacion, Estado, FechaRegistro
    FROM dbo.Vehiculos
    ORDER BY FechaRegistro DESC, IdVehiculo DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DocumentosConductor_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdDocumentoConductor, IdConductor, TipoDocumento, NumeroDocumento, UrlArchivo,
           FechaVencimiento, EstadoVerificacion, Observacion, EsVigente, FechaRegistro
    FROM dbo.DocumentosConductor
    ORDER BY FechaRegistro DESC, IdDocumentoConductor DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DocumentosConductor_ObtenerPorId
    @IdDocumentoConductor INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdDocumentoConductor, IdConductor, TipoDocumento, NumeroDocumento, UrlArchivo,
           FechaVencimiento, EstadoVerificacion, Observacion, EsVigente, FechaRegistro
    FROM dbo.DocumentosConductor
    WHERE IdDocumentoConductor = @IdDocumentoConductor;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Conductores_RecalcularAprobacionPerfil
    @IdConductor INT,
    @EmitirResultado BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CategoriaVehiculo VARCHAR(30);
    SELECT TOP (1) @CategoriaVehiculo = ts.CategoriaVehiculo
    FROM dbo.Vehiculos v
    INNER JOIN dbo.TiposServicio ts ON ts.IdTipoServicio = v.IdTipoServicio
    WHERE v.IdConductor = @IdConductor
      AND v.Estado = 1
      AND v.EstadoVerificacion = 'Aprobado'
      AND v.Verificado = 1
    ORDER BY v.FechaRegistro DESC, v.IdVehiculo DESC;

    DECLARE @Aprobado BIT = CASE
        WHEN @CategoriaVehiculo IS NULL THEN 0
        WHEN NOT EXISTS
        (
            SELECT 1
            FROM (VALUES ('CedulaIdentidad'), ('LicenciaConducir'), ('FotoVerificacion'), ('DocumentoVehiculo')) r(TipoDocumento)
            WHERE NOT EXISTS
            (
                SELECT 1
                FROM dbo.DocumentosConductor d
                WHERE d.IdConductor = @IdConductor
                  AND d.TipoDocumento = r.TipoDocumento
                  AND d.EsVigente = 1
                  AND d.EstadoVerificacion = 'Aprobado'
                  AND (d.FechaVencimiento IS NULL OR d.FechaVencimiento >= CAST(GETDATE() AS DATE))
            )
        )
        AND (@CategoriaVehiculo <> 'Microbus' OR EXISTS
        (
            SELECT 1 FROM dbo.DocumentosConductor d
            WHERE d.IdConductor = @IdConductor
              AND d.TipoDocumento = 'DocumentoMicrobus'
              AND d.EsVigente = 1
              AND d.EstadoVerificacion = 'Aprobado'
              AND (d.FechaVencimiento IS NULL OR d.FechaVencimiento >= CAST(GETDATE() AS DATE))
        )) THEN 1 ELSE 0 END;

    UPDATE dbo.Conductores
    SET Verificado = @Aprobado,
        Disponible = CASE WHEN @Aprobado = 0 THEN 0 ELSE Disponible END
    WHERE IdConductor = @IdConductor;

    IF @EmitirResultado = 1 SELECT @Aprobado;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Vehiculos_Crear
    @IdConductor INT,
    @IdTipoServicio INT,
    @Placa VARCHAR(30),
    @Marca VARCHAR(80),
    @Modelo VARCHAR(80),
    @Color VARCHAR(50),
    @Anio INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Conductores WHERE IdConductor = @IdConductor AND Estado = 1)
        THROW 51200, 'El conductor no existe o esta inactivo.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.TiposServicio WHERE IdTipoServicio = @IdTipoServicio AND Estado = 1)
        THROW 51201, 'El tipo de servicio no existe o esta inactivo.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Vehiculos WHERE UPPER(Placa) = UPPER(@Placa) AND Estado = 1)
        THROW 51202, 'La placa ya esta registrada en un vehiculo activo.', 1;

    INSERT INTO dbo.Vehiculos
        (IdConductor, IdTipoServicio, Placa, Marca, Modelo, Color, Anio,
         Verificado, EstadoVerificacion, Observacion, Estado, FechaRegistro)
    VALUES
        (@IdConductor, @IdTipoServicio, UPPER(LTRIM(RTRIM(@Placa))), LTRIM(RTRIM(@Marca)),
         LTRIM(RTRIM(@Modelo)), LTRIM(RTRIM(@Color)), @Anio,
         0, 'Pendiente', 'Pendiente de revision administrativa.', 1, GETDATE());

    DECLARE @IdVehiculo INT = CAST(SCOPE_IDENTITY() AS INT);
    EXEC dbo.sp_Conductores_RecalcularAprobacionPerfil @IdConductor, 0;
    SELECT @IdVehiculo;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Vehiculos_ActualizarEstadoVerificacion
    @IdVehiculo INT,
    @EstadoVerificacion VARCHAR(30),
    @Observacion VARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;

    IF @EstadoVerificacion NOT IN ('Pendiente', 'Aprobado', 'Rechazado')
        THROW 51203, 'Estado de verificacion de vehiculo invalido.', 1;

    DECLARE @IdConductor INT;
    SELECT @IdConductor = IdConductor FROM dbo.Vehiculos WHERE IdVehiculo = @IdVehiculo;

    UPDATE dbo.Vehiculos
    SET EstadoVerificacion = @EstadoVerificacion,
        Verificado = CASE WHEN @EstadoVerificacion = 'Aprobado' THEN 1 ELSE 0 END,
        Observacion = LTRIM(RTRIM(@Observacion))
    WHERE IdVehiculo = @IdVehiculo;

    DECLARE @Filas INT = @@ROWCOUNT;
    IF @Filas > 0 EXEC dbo.sp_Conductores_RecalcularAprobacionPerfil @IdConductor, 0;
    SELECT @Filas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DocumentosConductor_Crear
    @IdConductor INT,
    @TipoDocumento VARCHAR(100),
    @NumeroDocumento VARCHAR(80),
    @UrlArchivo VARCHAR(500),
    @FechaVencimiento DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Conductores WHERE IdConductor = @IdConductor AND Estado = 1)
        THROW 51210, 'El conductor no existe o esta inactivo.', 1;

    BEGIN TRANSACTION;
    UPDATE dbo.DocumentosConductor
    SET EsVigente = 0
    WHERE IdConductor = @IdConductor AND TipoDocumento = @TipoDocumento AND EsVigente = 1;

    INSERT INTO dbo.DocumentosConductor
        (IdConductor, TipoDocumento, NumeroDocumento, UrlArchivo, FechaVencimiento,
         EstadoVerificacion, Observacion, EsVigente, FechaRegistro)
    VALUES
        (@IdConductor, LTRIM(RTRIM(@TipoDocumento)), LTRIM(RTRIM(@NumeroDocumento)),
         @UrlArchivo, @FechaVencimiento, 'Pendiente',
         'Pendiente de revision administrativa.', 1, GETDATE());

    DECLARE @IdDocumento INT = CAST(SCOPE_IDENTITY() AS INT);
    COMMIT TRANSACTION;
    EXEC dbo.sp_Conductores_RecalcularAprobacionPerfil @IdConductor, 0;
    SELECT @IdDocumento;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DocumentosConductor_ActualizarEstadoVerificacion
    @IdDocumentoConductor INT,
    @EstadoVerificacion VARCHAR(50),
    @Observacion VARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;

    IF @EstadoVerificacion NOT IN ('Pendiente', 'Aprobado', 'Rechazado')
        THROW 51211, 'Estado de verificacion de documento invalido.', 1;

    DECLARE @IdConductor INT;
    SELECT @IdConductor = IdConductor FROM dbo.DocumentosConductor
    WHERE IdDocumentoConductor = @IdDocumentoConductor;

    UPDATE dbo.DocumentosConductor
    SET EstadoVerificacion = @EstadoVerificacion,
        Observacion = LTRIM(RTRIM(@Observacion))
    WHERE IdDocumentoConductor = @IdDocumentoConductor AND EsVigente = 1;

    DECLARE @Filas INT = @@ROWCOUNT;
    IF @Filas > 0 EXEC dbo.sp_Conductores_RecalcularAprobacionPerfil @IdConductor, 0;
    SELECT @Filas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Microbus_SuscribirConductor
    @IdRutaMicrobus INT,
    @IdConductor INT,
    @PeriodoInicio DATE,
    @EstadoPago VARCHAR(20) = 'Pendiente'
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.RutasMicrobus WHERE IdRutaMicrobus = @IdRutaMicrobus AND Estado = 1)
        THROW 51220, 'La ruta de microbus no existe o esta inactiva.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Conductores WHERE IdConductor = @IdConductor AND Estado = 1 AND Verificado = 1)
        THROW 51221, 'El conductor debe estar activo y aprobado.', 1;

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.Vehiculos v
        INNER JOIN dbo.TiposServicio ts ON ts.IdTipoServicio = v.IdTipoServicio
        WHERE v.IdConductor = @IdConductor AND v.Estado = 1
          AND v.EstadoVerificacion = 'Aprobado' AND ts.CategoriaVehiculo = 'Microbus'
    )
        THROW 51222, 'Debe registrar y aprobar un micro o microbus antes de asignar una ruta.', 1;

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.DocumentosConductor
        WHERE IdConductor = @IdConductor AND TipoDocumento = 'DocumentoMicrobus'
          AND EsVigente = 1 AND EstadoVerificacion = 'Aprobado'
    )
        THROW 51223, 'El documento del micro o microbus debe estar aprobado.', 1;

    INSERT INTO dbo.SuscripcionesMicrobusConductor
        (IdRutaMicrobus, IdConductor, PeriodoInicio, PeriodoFin, MontoUsd, EstadoPago)
    SELECT @IdRutaMicrobus, @IdConductor, @PeriodoInicio,
           DATEADD(DAY, -1, DATEADD(MONTH, 1, @PeriodoInicio)),
           SuscripcionMensualChoferUsd, @EstadoPago
    FROM dbo.RutasMicrobus WHERE IdRutaMicrobus = @IdRutaMicrobus;

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END;
GO
