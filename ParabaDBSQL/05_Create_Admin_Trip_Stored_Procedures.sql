USE ParabaDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_ListarAdmin
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdViaje,
        IdPasajero,
        IdConductor,
        IdVehiculo,
        IdTipoServicio,
        IdEstadoViaje,
        Origen,
        Destino,
        TarifaEstimada,
        TarifaFinal,
        TarifaSugerida,
        TarifaOfertada,
        TarifaContraoferta,
        TarifaAceptada,
        FechaSolicitud,
        FechaInicio,
        FechaFin
    FROM Viajes
    ORDER BY IdViaje;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_CancelarAdmin
    @IdViaje INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Viajes
    SET
        IdEstadoViaje = 5,
        FechaFin = ISNULL(FechaFin, GETDATE())
    WHERE
        IdViaje = @IdViaje
        AND IdEstadoViaje IN (1, 2, 3);

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_AuditoriaViajes_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdAuditoriaViaje,
        IdViaje,
        Accion,
        EstadoAnterior,
        EstadoNuevo,
        TarifaAnterior,
        TarifaNueva,
        UsuarioSistema,
        Observacion,
        FechaRegistro
    FROM AuditoriaViajes
    ORDER BY FechaRegistro DESC, IdAuditoriaViaje DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_AuditoriaViajes_Registrar
    @IdViaje INT,
    @Accion VARCHAR(50),
    @EstadoAnterior VARCHAR(50),
    @EstadoNuevo VARCHAR(50),
    @TarifaAnterior DECIMAL(10,2) = NULL,
    @TarifaNueva DECIMAL(10,2) = NULL,
    @UsuarioSistema VARCHAR(100),
    @Observacion VARCHAR(300),
    @FechaRegistro DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AuditoriaViajes
    (
        IdViaje,
        Accion,
        EstadoAnterior,
        EstadoNuevo,
        TarifaAnterior,
        TarifaNueva,
        UsuarioSistema,
        Observacion,
        FechaRegistro
    )
    VALUES
    (
        @IdViaje,
        @Accion,
        @EstadoAnterior,
        @EstadoNuevo,
        @TarifaAnterior,
        @TarifaNueva,
        @UsuarioSistema,
        @Observacion,
        @FechaRegistro
    );
END;
GO
