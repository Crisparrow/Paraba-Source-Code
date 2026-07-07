USE ParabaDB;
GO

IF OBJECT_ID('dbo.SolicitudesRegistroConductor', 'U') IS NULL
BEGIN
    CREATE TABLE SolicitudesRegistroConductor
    (
        IdSolicitudRegistroConductor INT IDENTITY(1,1) PRIMARY KEY,
        IdConductor INT NULL,
        Telefono VARCHAR(30) NOT NULL,
        NombreCompleto VARCHAR(150) NULL,
        DocumentoIdentidad VARCHAR(30) NULL,
        Correo VARCHAR(150) NULL,
        LicenciaConducir VARCHAR(50) NULL,
        FechaVencimientoLicencia DATE NULL,
        IdTipoServicio INT NULL,
        Placa VARCHAR(30) NULL,
        Marca VARCHAR(80) NULL,
        Modelo VARCHAR(80) NULL,
        Color VARCHAR(50) NULL,
        Anio INT NULL,
        EstadoSolicitud VARCHAR(30) NOT NULL,
        ObservacionRevision VARCHAR(300) NOT NULL,
        FechaCreacion DATETIME NOT NULL,
        FechaActualizacion DATETIME NOT NULL,
        FechaEnvio DATETIME NULL,
        FechaRevision DATETIME NULL,
        CONSTRAINT UQ_SolicitudesRegistroConductor_Telefono UNIQUE (Telefono),
        CONSTRAINT FK_SolicitudesRegistroConductor_Conductores FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor),
        CONSTRAINT FK_SolicitudesRegistroConductor_TiposServicio FOREIGN KEY (IdTipoServicio) REFERENCES TiposServicio(IdTipoServicio),
        CONSTRAINT CK_SolicitudesRegistroConductor_Estado CHECK (EstadoSolicitud IN ('Borrador', 'PendienteRevision', 'Observado', 'Aprobado', 'Rechazado'))
    );
END;
GO

IF OBJECT_ID('dbo.CodigosVerificacionConductor', 'U') IS NULL
BEGIN
    CREATE TABLE CodigosVerificacionConductor
    (
        IdCodigoVerificacionConductor INT IDENTITY(1,1) PRIMARY KEY,
        Telefono VARCHAR(30) NOT NULL,
        Codigo VARCHAR(10) NOT NULL,
        Usado BIT NOT NULL,
        FechaCreacion DATETIME NOT NULL,
        FechaExpiracion DATETIME NOT NULL,
        FechaUso DATETIME NULL
    );
END;
GO

IF OBJECT_ID('dbo.SesionesConductor', 'U') IS NULL
BEGIN
    CREATE TABLE SesionesConductor
    (
        IdSesionConductor INT IDENTITY(1,1) PRIMARY KEY,
        IdConductor INT NULL,
        IdSolicitudRegistroConductor INT NULL,
        Telefono VARCHAR(30) NOT NULL,
        Token VARCHAR(120) NOT NULL,
        FechaCreacion DATETIME NOT NULL,
        FechaExpiracion DATETIME NOT NULL,
        Revocada BIT NOT NULL,
        CONSTRAINT UQ_SesionesConductor_Token UNIQUE (Token),
        CONSTRAINT FK_SesionesConductor_Conductores FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor),
        CONSTRAINT FK_SesionesConductor_Solicitudes FOREIGN KEY (IdSolicitudRegistroConductor) REFERENCES SolicitudesRegistroConductor(IdSolicitudRegistroConductor)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CodigosVerificacionConductor_Telefono' AND object_id = OBJECT_ID('dbo.CodigosVerificacionConductor'))
BEGIN
    CREATE INDEX IX_CodigosVerificacionConductor_Telefono
    ON CodigosVerificacionConductor(Telefono, Usado, FechaExpiracion);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SesionesConductor_Token' AND object_id = OBJECT_ID('dbo.SesionesConductor'))
BEGIN
    CREATE INDEX IX_SesionesConductor_Token
    ON SesionesConductor(Token, Revocada, FechaExpiracion);
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductor_ObtenerPorTelefono
    @Telefono VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        IdSolicitudRegistroConductor,
        IdConductor,
        Telefono,
        NombreCompleto,
        DocumentoIdentidad,
        Correo,
        LicenciaConducir,
        FechaVencimientoLicencia,
        IdTipoServicio,
        Placa,
        Marca,
        Modelo,
        Color,
        Anio,
        EstadoSolicitud,
        ObservacionRevision,
        FechaCreacion,
        FechaActualizacion,
        FechaEnvio,
        FechaRevision
    FROM SolicitudesRegistroConductor
    WHERE Telefono = @Telefono;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductor_CrearOBuscarSolicitud
    @Telefono VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdConductor INT = NULL;

    SELECT TOP 1 @IdConductor = IdConductor
    FROM Conductores
    WHERE Telefono = @Telefono;

    IF NOT EXISTS (SELECT 1 FROM SolicitudesRegistroConductor WHERE Telefono = @Telefono)
    BEGIN
        INSERT INTO SolicitudesRegistroConductor
        (
            IdConductor,
            Telefono,
            EstadoSolicitud,
            ObservacionRevision,
            FechaCreacion,
            FechaActualizacion
        )
        VALUES
        (
            @IdConductor,
            @Telefono,
            CASE WHEN @IdConductor IS NULL THEN 'Borrador' ELSE 'Aprobado' END,
            '',
            GETDATE(),
            GETDATE()
        );
    END

    EXEC dbo.sp_RegistroConductor_ObtenerPorTelefono @Telefono;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductor_GuardarBorrador
    @Telefono VARCHAR(30),
    @NombreCompleto VARCHAR(150),
    @DocumentoIdentidad VARCHAR(30),
    @Correo VARCHAR(150),
    @LicenciaConducir VARCHAR(50),
    @FechaVencimientoLicencia DATE,
    @IdTipoServicio INT,
    @Placa VARCHAR(30),
    @Marca VARCHAR(80),
    @Modelo VARCHAR(80),
    @Color VARCHAR(50),
    @Anio INT
AS
BEGIN
    SET NOCOUNT ON;

    EXEC dbo.sp_RegistroConductor_CrearOBuscarSolicitud @Telefono;

    UPDATE SolicitudesRegistroConductor
    SET
        NombreCompleto = @NombreCompleto,
        DocumentoIdentidad = @DocumentoIdentidad,
        Correo = @Correo,
        LicenciaConducir = @LicenciaConducir,
        FechaVencimientoLicencia = @FechaVencimientoLicencia,
        IdTipoServicio = @IdTipoServicio,
        Placa = @Placa,
        Marca = @Marca,
        Modelo = @Modelo,
        Color = @Color,
        Anio = @Anio,
        EstadoSolicitud = CASE
            WHEN EstadoSolicitud IN ('PendienteRevision', 'Aprobado') THEN EstadoSolicitud
            ELSE 'Borrador'
        END,
        FechaActualizacion = GETDATE()
    WHERE Telefono = @Telefono;

    EXEC dbo.sp_RegistroConductor_ObtenerPorTelefono @Telefono;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductor_EnviarRevision
    @Telefono VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SolicitudesRegistroConductor
    SET
        EstadoSolicitud = 'PendienteRevision',
        ObservacionRevision = '',
        FechaEnvio = ISNULL(FechaEnvio, GETDATE()),
        FechaActualizacion = GETDATE()
    WHERE
        Telefono = @Telefono
        AND EstadoSolicitud IN ('Borrador', 'Observado', 'Rechazado');

    EXEC dbo.sp_RegistroConductor_ObtenerPorTelefono @Telefono;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductor_RegistrarCodigo
    @Telefono VARCHAR(30),
    @Codigo VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE CodigosVerificacionConductor
    SET Usado = 1, FechaUso = GETDATE()
    WHERE Telefono = @Telefono AND Usado = 0;

    INSERT INTO CodigosVerificacionConductor
    (
        Telefono,
        Codigo,
        Usado,
        FechaCreacion,
        FechaExpiracion
    )
    VALUES
    (
        @Telefono,
        @Codigo,
        0,
        GETDATE(),
        DATEADD(MINUTE, 10, GETDATE())
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductor_VerificarCodigo
    @Telefono VARCHAR(30),
    @Codigo VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdCodigo INT = NULL;

    SELECT TOP 1 @IdCodigo = IdCodigoVerificacionConductor
    FROM CodigosVerificacionConductor
    WHERE
        Telefono = @Telefono
        AND Codigo = @Codigo
        AND Usado = 0
        AND FechaExpiracion >= GETDATE()
    ORDER BY IdCodigoVerificacionConductor DESC;

    IF @IdCodigo IS NULL
    BEGIN
        SELECT CAST(0 AS BIT) AS CodigoValido;
        RETURN;
    END

    UPDATE CodigosVerificacionConductor
    SET Usado = 1, FechaUso = GETDATE()
    WHERE IdCodigoVerificacionConductor = @IdCodigo;

    SELECT CAST(1 AS BIT) AS CodigoValido;

    EXEC dbo.sp_RegistroConductor_CrearOBuscarSolicitud @Telefono;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductor_CrearSesion
    @Telefono VARCHAR(30),
    @Token VARCHAR(120)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdConductor INT = NULL;
    DECLARE @IdSolicitud INT = NULL;

    SELECT TOP 1 @IdConductor = IdConductor
    FROM Conductores
    WHERE Telefono = @Telefono;

    SELECT TOP 1 @IdSolicitud = IdSolicitudRegistroConductor
    FROM SolicitudesRegistroConductor
    WHERE Telefono = @Telefono;

    INSERT INTO SesionesConductor
    (
        IdConductor,
        IdSolicitudRegistroConductor,
        Telefono,
        Token,
        FechaCreacion,
        FechaExpiracion,
        Revocada
    )
    VALUES
    (
        @IdConductor,
        @IdSolicitud,
        @Telefono,
        @Token,
        GETDATE(),
        DATEADD(DAY, 30, GETDATE()),
        0
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductor_ValidarSesion
    @Telefono VARCHAR(30),
    @Token VARCHAR(120)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM SesionesConductor
        WHERE
            Telefono = @Telefono
            AND Token = @Token
            AND Revocada = 0
            AND FechaExpiracion >= GETDATE()
    )
    THEN 1 ELSE 0 END AS BIT) AS SesionValida;
END;
GO
