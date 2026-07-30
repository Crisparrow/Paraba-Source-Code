USE ParabaDB;
GO

IF OBJECT_ID('dbo.RutasMicrobus', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RutasMicrobus
    (
        IdRutaMicrobus INT IDENTITY(1,1) PRIMARY KEY,
        Nombre VARCHAR(100) NOT NULL,
        Origen VARCHAR(200) NOT NULL,
        Destino VARCHAR(200) NOT NULL,
        Recorrido VARCHAR(1000) NOT NULL,
        TarifaPasajeBs DECIMAL(10,2) NOT NULL CONSTRAINT DF_RutasMicrobus_Tarifa DEFAULT 2.00,
        SuscripcionMensualChoferUsd DECIMAL(10,2) NOT NULL CONSTRAINT DF_RutasMicrobus_Suscripcion DEFAULT 50.00,
        Estado BIT NOT NULL CONSTRAINT DF_RutasMicrobus_Estado DEFAULT 1,
        FechaRegistro DATETIME2 NOT NULL CONSTRAINT DF_RutasMicrobus_Fecha DEFAULT SYSDATETIME(),
        CONSTRAINT CK_RutasMicrobus_Tarifa CHECK (TarifaPasajeBs > 0),
        CONSTRAINT CK_RutasMicrobus_Suscripcion CHECK (SuscripcionMensualChoferUsd > 0)
    );
END;
GO

IF OBJECT_ID('dbo.SuscripcionesMicrobusConductor', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SuscripcionesMicrobusConductor
    (
        IdSuscripcionMicrobus INT IDENTITY(1,1) PRIMARY KEY,
        IdRutaMicrobus INT NOT NULL,
        IdConductor INT NOT NULL,
        PeriodoInicio DATE NOT NULL,
        PeriodoFin DATE NOT NULL,
        MontoUsd DECIMAL(10,2) NOT NULL CONSTRAINT DF_SuscripcionMicrobus_Monto DEFAULT 50.00,
        EstadoPago VARCHAR(20) NOT NULL CONSTRAINT DF_SuscripcionMicrobus_Pago DEFAULT 'Pendiente',
        Estado BIT NOT NULL CONSTRAINT DF_SuscripcionMicrobus_Estado DEFAULT 1,
        FechaRegistro DATETIME2 NOT NULL CONSTRAINT DF_SuscripcionMicrobus_Fecha DEFAULT SYSDATETIME(),
        CONSTRAINT FK_SuscripcionMicrobus_Ruta FOREIGN KEY (IdRutaMicrobus) REFERENCES dbo.RutasMicrobus(IdRutaMicrobus),
        CONSTRAINT FK_SuscripcionMicrobus_Conductor FOREIGN KEY (IdConductor) REFERENCES dbo.Conductores(IdConductor),
        CONSTRAINT CK_SuscripcionMicrobus_Periodo CHECK (PeriodoFin >= PeriodoInicio),
        CONSTRAINT CK_SuscripcionMicrobus_Pago CHECK (EstadoPago IN ('Pendiente', 'Pagado', 'Vencido')),
        CONSTRAINT UQ_SuscripcionMicrobus_Periodo UNIQUE (IdRutaMicrobus, IdConductor, PeriodoInicio)
    );
END;
GO

IF OBJECT_ID('dbo.AsociacionesMototaxi', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AsociacionesMototaxi
    (
        IdAsociacionMototaxi INT IDENTITY(1,1) PRIMARY KEY,
        Nombre VARCHAR(120) NOT NULL,
        Parada VARCHAR(250) NOT NULL,
        CostoMensualUsd DECIMAL(10,2) NOT NULL CONSTRAINT DF_AsociacionMototaxi_Costo DEFAULT 50.00,
        CuposTotales INT NOT NULL CONSTRAINT DF_AsociacionMototaxi_Cupos DEFAULT 20,
        Estado BIT NOT NULL CONSTRAINT DF_AsociacionMototaxi_Estado DEFAULT 1,
        FechaRegistro DATETIME2 NOT NULL CONSTRAINT DF_AsociacionMototaxi_Fecha DEFAULT SYSDATETIME(),
        CONSTRAINT CK_AsociacionMototaxi_Costo CHECK (CostoMensualUsd > 0),
        CONSTRAINT CK_AsociacionMototaxi_Cupos CHECK (CuposTotales BETWEEN 1 AND 20)
    );
END;
GO

IF OBJECT_ID('dbo.AsociacionMototaxiConductores', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AsociacionMototaxiConductores
    (
        IdAsociacionConductor INT IDENTITY(1,1) PRIMARY KEY,
        IdAsociacionMototaxi INT NOT NULL,
        IdConductor INT NOT NULL,
        NumeroRanura INT NOT NULL,
        EstadoPago VARCHAR(20) NOT NULL CONSTRAINT DF_AsociacionConductor_Pago DEFAULT 'Pendiente',
        PeriodoInicio DATE NOT NULL,
        PeriodoFin DATE NOT NULL,
        Estado BIT NOT NULL CONSTRAINT DF_AsociacionConductor_Estado DEFAULT 1,
        FechaRegistro DATETIME2 NOT NULL CONSTRAINT DF_AsociacionConductor_Fecha DEFAULT SYSDATETIME(),
        CONSTRAINT FK_AsociacionConductor_Asociacion FOREIGN KEY (IdAsociacionMototaxi) REFERENCES dbo.AsociacionesMototaxi(IdAsociacionMototaxi),
        CONSTRAINT FK_AsociacionConductor_Conductor FOREIGN KEY (IdConductor) REFERENCES dbo.Conductores(IdConductor),
        CONSTRAINT CK_AsociacionConductor_Ranura CHECK (NumeroRanura BETWEEN 1 AND 20),
        CONSTRAINT CK_AsociacionConductor_Periodo CHECK (PeriodoFin >= PeriodoInicio),
        CONSTRAINT CK_AsociacionConductor_Pago CHECK (EstadoPago IN ('Pendiente', 'Pagado', 'Vencido')),
        CONSTRAINT UQ_AsociacionConductor_Ranura UNIQUE (IdAsociacionMototaxi, NumeroRanura),
        CONSTRAINT UQ_AsociacionConductor_Chofer UNIQUE (IdAsociacionMototaxi, IdConductor)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.RutasMicrobus)
BEGIN
    INSERT INTO dbo.RutasMicrobus (Nombre, Origen, Destino, Recorrido)
    VALUES ('Ruta piloto PARABA', 'Parada central', 'Terminal de salida', 'Parada central -> corredor principal -> terminal de salida');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.AsociacionesMototaxi)
BEGIN
    INSERT INTO dbo.AsociacionesMototaxi (Nombre, Parada)
    VALUES ('Asociacion piloto PARABA', 'Parada de mototaxis principal');
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RutasMicrobus_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.IdRutaMicrobus,
        r.Nombre,
        r.Origen,
        r.Destino,
        r.Recorrido,
        r.TarifaPasajeBs,
        r.SuscripcionMensualChoferUsd,
        r.Estado,
        r.FechaRegistro,
        COUNT(CASE WHEN s.Estado = 1 AND s.PeriodoFin >= CAST(GETDATE() AS DATE) THEN 1 END) AS ChoferesSuscritos
    FROM dbo.RutasMicrobus r
    LEFT JOIN dbo.SuscripcionesMicrobusConductor s ON s.IdRutaMicrobus = r.IdRutaMicrobus
    GROUP BY r.IdRutaMicrobus, r.Nombre, r.Origen, r.Destino, r.Recorrido,
        r.TarifaPasajeBs, r.SuscripcionMensualChoferUsd, r.Estado, r.FechaRegistro
    ORDER BY r.Estado DESC, r.Nombre;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_AsociacionesMototaxi_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.IdAsociacionMototaxi,
        a.Nombre,
        a.Parada,
        a.CostoMensualUsd,
        a.CuposTotales,
        COUNT(CASE WHEN m.Estado = 1 AND m.PeriodoFin >= CAST(GETDATE() AS DATE) THEN 1 END) AS CuposOcupados,
        a.Estado,
        a.FechaRegistro
    FROM dbo.AsociacionesMototaxi a
    LEFT JOIN dbo.AsociacionMototaxiConductores m ON m.IdAsociacionMototaxi = a.IdAsociacionMototaxi
    GROUP BY a.IdAsociacionMototaxi, a.Nombre, a.Parada, a.CostoMensualUsd,
        a.CuposTotales, a.Estado, a.FechaRegistro
    ORDER BY a.Estado DESC, a.Nombre;
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
        THROW 51100, 'La ruta de microbus no existe o esta inactiva.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Conductores WHERE IdConductor = @IdConductor AND Estado = 1 AND Verificado = 1)
        THROW 51101, 'El conductor debe estar activo y aprobado.', 1;

    INSERT INTO dbo.SuscripcionesMicrobusConductor
        (IdRutaMicrobus, IdConductor, PeriodoInicio, PeriodoFin, MontoUsd, EstadoPago)
    SELECT @IdRutaMicrobus, @IdConductor, @PeriodoInicio, DATEADD(DAY, -1, DATEADD(MONTH, 1, @PeriodoInicio)),
        SuscripcionMensualChoferUsd, @EstadoPago
    FROM dbo.RutasMicrobus
    WHERE IdRutaMicrobus = @IdRutaMicrobus;

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_AsociacionMototaxi_AsignarRanura
    @IdAsociacionMototaxi INT,
    @IdConductor INT,
    @NumeroRanura INT,
    @PeriodoInicio DATE,
    @EstadoPago VARCHAR(20) = 'Pendiente'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @CuposTotales INT;
    SELECT @CuposTotales = CuposTotales
    FROM dbo.AsociacionesMototaxi WITH (UPDLOCK, HOLDLOCK)
    WHERE IdAsociacionMototaxi = @IdAsociacionMototaxi AND Estado = 1;

    IF @CuposTotales IS NULL
        THROW 51110, 'La asociacion no existe o esta inactiva.', 1;

    IF @NumeroRanura NOT BETWEEN 1 AND @CuposTotales
        THROW 51111, 'La ranura debe estar dentro de los cupos habilitados.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Conductores WHERE IdConductor = @IdConductor AND Estado = 1 AND Verificado = 1)
        THROW 51112, 'El conductor debe estar activo y aprobado.', 1;

    IF EXISTS (SELECT 1 FROM dbo.AsociacionMototaxiConductores WHERE IdAsociacionMototaxi = @IdAsociacionMototaxi AND NumeroRanura = @NumeroRanura)
        THROW 51113, 'La ranura seleccionada ya esta ocupada.', 1;

    INSERT INTO dbo.AsociacionMototaxiConductores
        (IdAsociacionMototaxi, IdConductor, NumeroRanura, EstadoPago, PeriodoInicio, PeriodoFin)
    VALUES
        (@IdAsociacionMototaxi, @IdConductor, @NumeroRanura, @EstadoPago, @PeriodoInicio,
         DATEADD(DAY, -1, DATEADD(MONTH, 1, @PeriodoInicio)));

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END;
GO
