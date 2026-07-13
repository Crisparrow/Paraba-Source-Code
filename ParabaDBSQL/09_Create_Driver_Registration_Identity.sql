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

IF COL_LENGTH('dbo.SolicitudesRegistroConductor', 'EstadoDatosConductor') IS NULL
BEGIN
    ALTER TABLE SolicitudesRegistroConductor
    ADD EstadoDatosConductor VARCHAR(30) NOT NULL CONSTRAINT DF_SolicitudesRegistroConductor_EstadoDatosConductor DEFAULT 'Pendiente';
END;
GO

IF COL_LENGTH('dbo.SolicitudesRegistroConductor', 'EstadoDatosVehiculo') IS NULL
BEGIN
    ALTER TABLE SolicitudesRegistroConductor
    ADD EstadoDatosVehiculo VARCHAR(30) NOT NULL CONSTRAINT DF_SolicitudesRegistroConductor_EstadoDatosVehiculo DEFAULT 'Pendiente';
END;
GO

IF COL_LENGTH('dbo.SolicitudesRegistroConductor', 'EstadoDocumentos') IS NULL
BEGIN
    ALTER TABLE SolicitudesRegistroConductor
    ADD EstadoDocumentos VARCHAR(30) NOT NULL CONSTRAINT DF_SolicitudesRegistroConductor_EstadoDocumentos DEFAULT 'Pendiente';
END;
GO

IF COL_LENGTH('dbo.SolicitudesRegistroConductor', 'ObservacionDatosConductor') IS NULL
BEGIN
    ALTER TABLE SolicitudesRegistroConductor
    ADD ObservacionDatosConductor VARCHAR(300) NOT NULL CONSTRAINT DF_SolicitudesRegistroConductor_ObservacionDatosConductor DEFAULT '';
END;
GO

IF COL_LENGTH('dbo.SolicitudesRegistroConductor', 'ObservacionDatosVehiculo') IS NULL
BEGIN
    ALTER TABLE SolicitudesRegistroConductor
    ADD ObservacionDatosVehiculo VARCHAR(300) NOT NULL CONSTRAINT DF_SolicitudesRegistroConductor_ObservacionDatosVehiculo DEFAULT '';
END;
GO

IF COL_LENGTH('dbo.SolicitudesRegistroConductor', 'ObservacionDocumentos') IS NULL
BEGIN
    ALTER TABLE SolicitudesRegistroConductor
    ADD ObservacionDocumentos VARCHAR(300) NOT NULL CONSTRAINT DF_SolicitudesRegistroConductor_ObservacionDocumentos DEFAULT '';
END;
GO

IF OBJECT_ID('dbo.SolicitudesRegistroConductorDocumentos', 'U') IS NULL
BEGIN
    CREATE TABLE SolicitudesRegistroConductorDocumentos
    (
        IdSolicitudRegistroConductorDocumento INT IDENTITY(1,1) PRIMARY KEY,
        IdSolicitudRegistroConductor INT NOT NULL,
        TipoDocumento VARCHAR(50) NOT NULL,
        NumeroDocumento VARCHAR(50) NOT NULL,
        UrlArchivo VARCHAR(300) NOT NULL,
        FechaVencimiento DATE NULL,
        EsOpcional BIT NOT NULL,
        EstadoVerificacion VARCHAR(30) NOT NULL,
        Observacion VARCHAR(300) NOT NULL,
        FechaRegistro DATETIME NOT NULL,
        FechaRevision DATETIME NULL,
        CONSTRAINT FK_SolicitudesRegistroConductorDocumentos_Solicitudes FOREIGN KEY (IdSolicitudRegistroConductor) REFERENCES SolicitudesRegistroConductor(IdSolicitudRegistroConductor),
        CONSTRAINT CK_SolicitudesRegistroConductorDocumentos_Estado CHECK (EstadoVerificacion IN ('Pendiente', 'Aprobado', 'Observado', 'Rechazado'))
    );
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
        EstadoDatosConductor,
        EstadoDatosVehiculo,
        EstadoDocumentos,
        ObservacionRevision,
        ObservacionDatosConductor,
        ObservacionDatosVehiculo,
        ObservacionDocumentos,
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
            EstadoDatosConductor,
            EstadoDatosVehiculo,
            EstadoDocumentos,
            ObservacionRevision,
            ObservacionDatosConductor,
            ObservacionDatosVehiculo,
            ObservacionDocumentos,
            FechaCreacion,
            FechaActualizacion
        )
        VALUES
        (
            @IdConductor,
            @Telefono,
            CASE WHEN @IdConductor IS NULL THEN 'Borrador' ELSE 'Aprobado' END,
            CASE WHEN @IdConductor IS NULL THEN 'Pendiente' ELSE 'Aprobado' END,
            CASE WHEN @IdConductor IS NULL THEN 'Pendiente' ELSE 'Aprobado' END,
            CASE WHEN @IdConductor IS NULL THEN 'Pendiente' ELSE 'Aprobado' END,
            '',
            '',
            '',
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

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductor_ListarSolicitudes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
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
        EstadoDatosConductor,
        EstadoDatosVehiculo,
        EstadoDocumentos,
        ObservacionRevision,
        ObservacionDatosConductor,
        ObservacionDatosVehiculo,
        ObservacionDocumentos,
        FechaCreacion,
        FechaActualizacion,
        FechaEnvio,
        FechaRevision
    FROM SolicitudesRegistroConductor
    ORDER BY FechaActualizacion DESC, IdSolicitudRegistroConductor DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductorDocumentos_Listar
    @IdSolicitudRegistroConductor INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdSolicitudRegistroConductorDocumento,
        IdSolicitudRegistroConductor,
        TipoDocumento,
        NumeroDocumento,
        UrlArchivo,
        FechaVencimiento,
        EsOpcional,
        EstadoVerificacion,
        Observacion,
        FechaRegistro,
        FechaRevision
    FROM SolicitudesRegistroConductorDocumentos
    WHERE IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor
    ORDER BY EsOpcional, TipoDocumento;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductorDocumentos_Guardar
    @IdSolicitudRegistroConductor INT,
    @TipoDocumento VARCHAR(50),
    @NumeroDocumento VARCHAR(50),
    @UrlArchivo VARCHAR(300),
    @FechaVencimiento DATE,
    @EsOpcional BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM SolicitudesRegistroConductorDocumentos
        WHERE IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor
            AND TipoDocumento = @TipoDocumento
    )
    BEGIN
        UPDATE SolicitudesRegistroConductorDocumentos
        SET
            NumeroDocumento = @NumeroDocumento,
            UrlArchivo = @UrlArchivo,
            FechaVencimiento = @FechaVencimiento,
            EsOpcional = @EsOpcional,
            EstadoVerificacion = 'Pendiente',
            Observacion = '',
            FechaRegistro = GETDATE(),
            FechaRevision = NULL
        WHERE IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor
            AND TipoDocumento = @TipoDocumento;
    END
    ELSE
    BEGIN
        INSERT INTO SolicitudesRegistroConductorDocumentos
        (
            IdSolicitudRegistroConductor,
            TipoDocumento,
            NumeroDocumento,
            UrlArchivo,
            FechaVencimiento,
            EsOpcional,
            EstadoVerificacion,
            Observacion,
            FechaRegistro
        )
        VALUES
        (
            @IdSolicitudRegistroConductor,
            @TipoDocumento,
            @NumeroDocumento,
            @UrlArchivo,
            @FechaVencimiento,
            @EsOpcional,
            'Pendiente',
            '',
            GETDATE()
        );
    END

    UPDATE SolicitudesRegistroConductor
    SET
        EstadoDocumentos = CASE WHEN EstadoDocumentos = 'Aprobado' THEN 'Pendiente' ELSE EstadoDocumentos END,
        FechaActualizacion = GETDATE()
    WHERE IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor;

    EXEC dbo.sp_RegistroConductorDocumentos_Listar @IdSolicitudRegistroConductor;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductor_ActivarSiAprobado
    @IdSolicitudRegistroConductor INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdConductor INT = NULL;
    DECLARE @Telefono VARCHAR(30);
    DECLARE @NombreCompleto VARCHAR(150);
    DECLARE @DocumentoIdentidad VARCHAR(30);
    DECLARE @Correo VARCHAR(150);
    DECLARE @LicenciaConducir VARCHAR(50);
    DECLARE @FechaVencimientoLicencia DATE;
    DECLARE @IdTipoServicio INT;
    DECLARE @Placa VARCHAR(30);
    DECLARE @Marca VARCHAR(80);
    DECLARE @Modelo VARCHAR(80);
    DECLARE @Color VARCHAR(50);
    DECLARE @Anio INT;

    SELECT
        @IdConductor = IdConductor,
        @Telefono = Telefono,
        @NombreCompleto = NombreCompleto,
        @DocumentoIdentidad = DocumentoIdentidad,
        @Correo = Correo,
        @LicenciaConducir = LicenciaConducir,
        @FechaVencimientoLicencia = FechaVencimientoLicencia,
        @IdTipoServicio = IdTipoServicio,
        @Placa = Placa,
        @Marca = Marca,
        @Modelo = Modelo,
        @Color = Color,
        @Anio = Anio
    FROM SolicitudesRegistroConductor
    WHERE
        IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor
        AND EstadoSolicitud = 'Aprobado'
        AND EstadoDatosConductor = 'Aprobado'
        AND EstadoDatosVehiculo = 'Aprobado'
        AND EstadoDocumentos = 'Aprobado';

    IF @Telefono IS NULL
    BEGIN
        RETURN;
    END

    IF @NombreCompleto IS NULL
        OR @DocumentoIdentidad IS NULL
        OR @LicenciaConducir IS NULL
        OR @FechaVencimientoLicencia IS NULL
        OR @IdTipoServicio IS NULL
        OR @Placa IS NULL
        OR @Marca IS NULL
        OR @Modelo IS NULL
        OR @Color IS NULL
        OR @Anio IS NULL
    BEGIN
        THROW 51000, 'La solicitud aprobada no tiene todos los datos obligatorios.', 1;
    END

    IF NOT EXISTS
    (
        SELECT 1
        FROM SolicitudesRegistroConductorDocumentos
        WHERE IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor
            AND TipoDocumento IN ('CarnetFrontal', 'CarnetReverso', 'Licencia', 'FotoConductor', 'FotoVehiculo', 'DocumentoVehiculo')
            AND UrlArchivo <> ''
        GROUP BY IdSolicitudRegistroConductor
        HAVING COUNT(DISTINCT TipoDocumento) = 6
    )
    BEGIN
        THROW 51001, 'La solicitud aprobada no tiene todos los documentos obligatorios.', 1;
    END

    IF @IdConductor IS NULL
    BEGIN
        SELECT TOP 1 @IdConductor = IdConductor
        FROM Conductores
        WHERE Telefono = @Telefono
        ORDER BY IdConductor DESC;
    END

    IF @IdConductor IS NULL
    BEGIN
        INSERT INTO Conductores
        (
            NombreCompleto,
            DocumentoIdentidad,
            Telefono,
            Correo,
            LicenciaConducir,
            FechaVencimientoLicencia,
            Disponible,
            Verificado,
            Estado,
            FechaRegistro
        )
        VALUES
        (
            @NombreCompleto,
            @DocumentoIdentidad,
            @Telefono,
            ISNULL(@Correo, ''),
            @LicenciaConducir,
            @FechaVencimientoLicencia,
            1,
            1,
            1,
            GETDATE()
        );

        SET @IdConductor = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE Conductores
        SET
            NombreCompleto = @NombreCompleto,
            DocumentoIdentidad = @DocumentoIdentidad,
            Correo = ISNULL(@Correo, ''),
            LicenciaConducir = @LicenciaConducir,
            FechaVencimientoLicencia = @FechaVencimientoLicencia,
            Verificado = 1,
            Estado = 1
        WHERE IdConductor = @IdConductor;
    END

    UPDATE SolicitudesRegistroConductor
    SET IdConductor = @IdConductor,
        FechaActualizacion = GETDATE()
    WHERE IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor;

    IF EXISTS
    (
        SELECT 1
        FROM Vehiculos
        WHERE IdConductor = @IdConductor AND Placa = @Placa
    )
    BEGIN
        UPDATE Vehiculos
        SET
            IdTipoServicio = @IdTipoServicio,
            Marca = @Marca,
            Modelo = @Modelo,
            Color = @Color,
            Anio = @Anio,
            Verificado = 1,
            Estado = 1
        WHERE IdConductor = @IdConductor AND Placa = @Placa;
    END
    ELSE
    BEGIN
        INSERT INTO Vehiculos
        (
            IdConductor,
            IdTipoServicio,
            Placa,
            Marca,
            Modelo,
            Color,
            Anio,
            Verificado,
            Estado,
            FechaRegistro
        )
        VALUES
        (
            @IdConductor,
            @IdTipoServicio,
            @Placa,
            @Marca,
            @Modelo,
            @Color,
            @Anio,
            1,
            1,
            GETDATE()
        );
    END

    DELETE dc
    FROM DocumentosConductor dc
    INNER JOIN SolicitudesRegistroConductorDocumentos srd
        ON srd.TipoDocumento = dc.TipoDocumento
    WHERE dc.IdConductor = @IdConductor
        AND srd.IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor;

    INSERT INTO DocumentosConductor
    (
        IdConductor,
        TipoDocumento,
        NumeroDocumento,
        UrlArchivo,
        FechaVencimiento,
        EstadoVerificacion,
        Observacion,
        FechaRegistro
    )
    SELECT
        @IdConductor,
        TipoDocumento,
        NumeroDocumento,
        UrlArchivo,
        FechaVencimiento,
        'Aprobado',
        Observacion,
        GETDATE()
    FROM SolicitudesRegistroConductorDocumentos
    WHERE IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor
        AND UrlArchivo <> '';
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistroConductor_RevisarCategoria
    @IdSolicitudRegistroConductor INT,
    @Categoria VARCHAR(30),
    @Estado VARCHAR(30),
    @Observacion VARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @Categoria = 'Conductor'
        BEGIN
            UPDATE SolicitudesRegistroConductor
            SET EstadoDatosConductor = @Estado,
                ObservacionDatosConductor = @Observacion,
                FechaRevision = GETDATE(),
                FechaActualizacion = GETDATE()
            WHERE IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor;
        END
        ELSE IF @Categoria = 'Vehiculo'
        BEGIN
            UPDATE SolicitudesRegistroConductor
            SET EstadoDatosVehiculo = @Estado,
                ObservacionDatosVehiculo = @Observacion,
                FechaRevision = GETDATE(),
                FechaActualizacion = GETDATE()
            WHERE IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor;
        END
        ELSE IF @Categoria = 'Documentos'
        BEGIN
            UPDATE SolicitudesRegistroConductor
            SET EstadoDocumentos = @Estado,
                ObservacionDocumentos = @Observacion,
                FechaRevision = GETDATE(),
                FechaActualizacion = GETDATE()
            WHERE IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor;
        END

        UPDATE SolicitudesRegistroConductor
        SET EstadoSolicitud = CASE
            WHEN EstadoDatosConductor = 'Aprobado'
                AND EstadoDatosVehiculo = 'Aprobado'
                AND EstadoDocumentos = 'Aprobado'
            THEN 'Aprobado'
            WHEN EstadoDatosConductor IN ('Observado', 'Rechazado')
                OR EstadoDatosVehiculo IN ('Observado', 'Rechazado')
                OR EstadoDocumentos IN ('Observado', 'Rechazado')
            THEN 'Observado'
            ELSE EstadoSolicitud
        END
        WHERE IdSolicitudRegistroConductor = @IdSolicitudRegistroConductor;

        EXEC dbo.sp_RegistroConductor_ActivarSiAprobado @IdSolicitudRegistroConductor;

        COMMIT TRANSACTION;

        SELECT CAST(1 AS INT) AS FilasAfectadas;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END

        ;THROW;
    END CATCH
END;
GO
