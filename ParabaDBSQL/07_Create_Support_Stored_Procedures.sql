USE ParabaDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Reclamos_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdReclamo,
        IdViaje,
        IdPasajero,
        IdConductor,
        TipoReclamo,
        Descripcion,
        Estado,
        Prioridad,
        UsuarioRegistro,
        UsuarioCierre,
        ObservacionCierre,
        FechaRegistro,
        FechaCierre
    FROM Reclamos
    ORDER BY FechaRegistro DESC, IdReclamo DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Reclamos_Registrar
    @IdViaje INT = NULL,
    @IdPasajero INT = NULL,
    @IdConductor INT = NULL,
    @TipoReclamo VARCHAR(100),
    @Descripcion VARCHAR(500),
    @Prioridad VARCHAR(50),
    @UsuarioRegistro VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Reclamos
    (
        IdViaje,
        IdPasajero,
        IdConductor,
        TipoReclamo,
        Descripcion,
        Estado,
        Prioridad,
        UsuarioRegistro,
        FechaRegistro
    )
    VALUES
    (
        @IdViaje,
        @IdPasajero,
        @IdConductor,
        @TipoReclamo,
        @Descripcion,
        'Abierto',
        @Prioridad,
        @UsuarioRegistro,
        GETDATE()
    );

    SELECT SCOPE_IDENTITY() AS IdReclamo;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Reclamos_Cerrar
    @IdReclamo INT,
    @Estado VARCHAR(50),
    @UsuarioCierre VARCHAR(150),
    @ObservacionCierre VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Reclamos
    SET
        Estado = @Estado,
        UsuarioCierre = @UsuarioCierre,
        ObservacionCierre = @ObservacionCierre,
        FechaCierre = GETDATE()
    WHERE
        IdReclamo = @IdReclamo
        AND Estado <> 'Cerrado';

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO
