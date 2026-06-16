USE ParabaDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DocumentosConductor_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdDocumentoConductor,
        IdConductor,
        TipoDocumento,
        NumeroDocumento,
        UrlArchivo,
        FechaVencimiento,
        EstadoVerificacion,
        Observacion,
        FechaRegistro
    FROM DocumentosConductor
    ORDER BY IdDocumentoConductor;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DocumentosConductor_ObtenerPorId
    @IdDocumentoConductor INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdDocumentoConductor,
        IdConductor,
        TipoDocumento,
        NumeroDocumento,
        UrlArchivo,
        FechaVencimiento,
        EstadoVerificacion,
        Observacion,
        FechaRegistro
    FROM DocumentosConductor
    WHERE IdDocumentoConductor = @IdDocumentoConductor;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DocumentosConductor_ActualizarEstadoVerificacion
    @IdDocumentoConductor INT,
    @EstadoVerificacion VARCHAR(50),
    @Observacion VARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE DocumentosConductor
    SET
        EstadoVerificacion = @EstadoVerificacion,
        Observacion = @Observacion
    WHERE IdDocumentoConductor = @IdDocumentoConductor;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Conductores_ActualizarVerificado
    @IdConductor INT,
    @Verificado BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Conductores
    SET Verificado = @Verificado
    WHERE IdConductor = @IdConductor;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Conductores_ActualizarEstado
    @IdConductor INT,
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Conductores
    SET
        Estado = @Estado,
        Disponible = CASE WHEN @Estado = 0 THEN 0 ELSE Disponible END
    WHERE IdConductor = @IdConductor;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO
