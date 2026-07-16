USE ParabaDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Conductores_ActualizarDisponibleApp
    @IdConductor INT,
    @Disponible BIT,
    @UsuarioSistema VARCHAR(100) = 'App Conductor'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoCaminoPasajero INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @IdEstadoContraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Contraofertado');
    DECLARE @EstadoAnterior VARCHAR(50);
    DECLARE @EstadoNuevo VARCHAR(50);

    IF NOT EXISTS (SELECT 1 FROM Conductores WHERE IdConductor = @IdConductor AND Estado = 1 AND Verificado = 1)
        THROW 51200, 'El conductor no esta aprobado para operar.', 1;

    IF @Disponible = 1 AND EXISTS
    (
        SELECT 1
        FROM Viajes
        WHERE IdConductor = @IdConductor
            AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoCaminoPasajero, @IdEstadoEnCurso, @IdEstadoContraofertado)
    )
        THROW 51201, 'No puedes conectarte como disponible mientras tienes un viaje activo.', 1;

    SELECT @EstadoAnterior = CASE WHEN Disponible = 1 THEN 'Disponible' ELSE 'No disponible' END
    FROM Conductores
    WHERE IdConductor = @IdConductor;

    SET @EstadoNuevo = CASE WHEN @Disponible = 1 THEN 'Disponible' ELSE 'No disponible' END;

    BEGIN TRANSACTION;

    UPDATE Conductores
    SET Disponible = @Disponible
    WHERE IdConductor = @IdConductor;

    INSERT INTO AuditoriaConductores
    (
        IdConductor,
        Accion,
        EstadoAnterior,
        EstadoNuevo,
        UsuarioSistema,
        Observacion,
        FechaRegistro
    )
    VALUES
    (
        @IdConductor,
        'Disponibilidad actualizada',
        @EstadoAnterior,
        @EstadoNuevo,
        @UsuarioSistema,
        'Cambio de disponibilidad realizado desde la app del conductor.',
        GETDATE()
    );

    COMMIT TRANSACTION;

    SELECT 1 AS FilasAfectadas;
END;
GO
