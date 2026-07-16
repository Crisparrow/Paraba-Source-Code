USE ParabaDB;
GO

IF NOT EXISTS (SELECT 1 FROM EstadosViaje WHERE Nombre = 'En camino al pasajero')
BEGIN
    INSERT INTO EstadosViaje (Nombre, Estado)
    VALUES ('En camino al pasajero', 1);
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_DisponiblesPorConductor
    @IdConductor INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdEstadoSolicitado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Solicitado');
    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoCaminoPasajero INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @IdEstadoContraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Contraofertado');

    IF NOT EXISTS
    (
        SELECT 1
        FROM Conductores
        WHERE IdConductor = @IdConductor
            AND Estado = 1
            AND Verificado = 1
            AND Disponible = 1
    )
    BEGIN
        SELECT TOP 0
            v.IdViaje,
            v.IdPasajero,
            v.IdConductor,
            v.IdVehiculo,
            v.IdTipoServicio,
            ts.Nombre AS TipoServicio,
            v.IdEstadoViaje,
            ev.Nombre AS EstadoViaje,
            v.Origen,
            v.Destino,
            v.TarifaEstimada,
            v.TarifaFinal,
            v.TarifaSugerida,
            v.TarifaOfertada,
            v.TarifaContraoferta,
            v.TarifaAceptada,
            v.FechaSolicitud,
            v.FechaAceptacion,
            v.FechaInicio,
            v.FechaFin,
            v.FechaCancelacion,
            v.MotivoCancelacion
        FROM Viajes v
        INNER JOIN EstadosViaje ev ON ev.IdEstadoViaje = v.IdEstadoViaje
        INNER JOIN TiposServicio ts ON ts.IdTipoServicio = v.IdTipoServicio;
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM Viajes
        WHERE IdConductor = @IdConductor
            AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoCaminoPasajero, @IdEstadoEnCurso, @IdEstadoContraofertado)
    )
    BEGIN
        SELECT TOP 0
            v.IdViaje,
            v.IdPasajero,
            v.IdConductor,
            v.IdVehiculo,
            v.IdTipoServicio,
            ts.Nombre AS TipoServicio,
            v.IdEstadoViaje,
            ev.Nombre AS EstadoViaje,
            v.Origen,
            v.Destino,
            v.TarifaEstimada,
            v.TarifaFinal,
            v.TarifaSugerida,
            v.TarifaOfertada,
            v.TarifaContraoferta,
            v.TarifaAceptada,
            v.FechaSolicitud,
            v.FechaAceptacion,
            v.FechaInicio,
            v.FechaFin,
            v.FechaCancelacion,
            v.MotivoCancelacion
        FROM Viajes v
        INNER JOIN EstadosViaje ev ON ev.IdEstadoViaje = v.IdEstadoViaje
        INNER JOIN TiposServicio ts ON ts.IdTipoServicio = v.IdTipoServicio;
        RETURN;
    END;

    SELECT
        v.IdViaje,
        v.IdPasajero,
        v.IdConductor,
        v.IdVehiculo,
        v.IdTipoServicio,
        ts.Nombre AS TipoServicio,
        v.IdEstadoViaje,
        ev.Nombre AS EstadoViaje,
        v.Origen,
        v.Destino,
        v.TarifaEstimada,
        v.TarifaFinal,
        v.TarifaSugerida,
        v.TarifaOfertada,
        v.TarifaContraoferta,
        v.TarifaAceptada,
        v.FechaSolicitud,
        v.FechaAceptacion,
        v.FechaInicio,
        v.FechaFin,
        v.FechaCancelacion,
        v.MotivoCancelacion
    FROM Viajes v
    INNER JOIN EstadosViaje ev ON ev.IdEstadoViaje = v.IdEstadoViaje
    INNER JOIN TiposServicio ts ON ts.IdTipoServicio = v.IdTipoServicio
    WHERE v.IdConductor = @IdConductor
        AND v.IdEstadoViaje = @IdEstadoSolicitado
    ORDER BY v.FechaSolicitud DESC, v.IdViaje DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_ActivosPorConductor
    @IdConductor INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoCaminoPasajero INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @IdEstadoContraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Contraofertado');

    SELECT
        v.IdViaje,
        v.IdPasajero,
        v.IdConductor,
        v.IdVehiculo,
        v.IdTipoServicio,
        ts.Nombre AS TipoServicio,
        v.IdEstadoViaje,
        ev.Nombre AS EstadoViaje,
        v.Origen,
        v.Destino,
        v.TarifaEstimada,
        v.TarifaFinal,
        v.TarifaSugerida,
        v.TarifaOfertada,
        v.TarifaContraoferta,
        v.TarifaAceptada,
        v.FechaSolicitud,
        v.FechaAceptacion,
        v.FechaInicio,
        v.FechaFin,
        v.FechaCancelacion,
        v.MotivoCancelacion
    FROM Viajes v
    INNER JOIN EstadosViaje ev ON ev.IdEstadoViaje = v.IdEstadoViaje
    INNER JOIN TiposServicio ts ON ts.IdTipoServicio = v.IdTipoServicio
    WHERE v.IdConductor = @IdConductor
        AND v.IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoCaminoPasajero, @IdEstadoEnCurso, @IdEstadoContraofertado)
    ORDER BY v.FechaSolicitud DESC, v.IdViaje DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_Aceptar
    @IdConductor INT,
    @IdViaje INT,
    @UsuarioSistema VARCHAR(100) = 'App Conductor'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEstadoSolicitado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Solicitado');
    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoCaminoPasajero INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @IdEstadoContraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Contraofertado');
    DECLARE @EstadoAnterior VARCHAR(50);
    DECLARE @TarifaAnterior DECIMAL(10,2);
    DECLARE @TarifaNueva DECIMAL(10,2);

    IF NOT EXISTS (SELECT 1 FROM Conductores WHERE IdConductor = @IdConductor AND Estado = 1 AND Verificado = 1 AND Disponible = 1)
        THROW 51000, 'El conductor no esta aprobado o no esta disponible.', 1;

    IF EXISTS (SELECT 1 FROM Viajes WHERE IdConductor = @IdConductor AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoCaminoPasajero, @IdEstadoEnCurso, @IdEstadoContraofertado))
        THROW 51001, 'El conductor ya tiene un viaje activo.', 1;

    SELECT
        @EstadoAnterior = ev.Nombre,
        @TarifaAnterior = v.TarifaAceptada,
        @TarifaNueva = CASE WHEN v.TarifaOfertada > 0 THEN v.TarifaOfertada ELSE v.TarifaSugerida END
    FROM Viajes v
    INNER JOIN EstadosViaje ev ON ev.IdEstadoViaje = v.IdEstadoViaje
    WHERE v.IdViaje = @IdViaje
        AND v.IdConductor = @IdConductor
        AND v.IdEstadoViaje = @IdEstadoSolicitado;

    IF @EstadoAnterior IS NULL
        THROW 51002, 'Solo se puede aceptar un viaje solicitado asignado al conductor.', 1;

    BEGIN TRANSACTION;

    UPDATE Viajes
    SET
        IdEstadoViaje = @IdEstadoCaminoPasajero,
        TarifaAceptada = @TarifaNueva,
        TarifaFinal = @TarifaNueva,
        FechaAceptacion = ISNULL(FechaAceptacion, GETDATE())
    WHERE IdViaje = @IdViaje;

    UPDATE Conductores
    SET Disponible = 0
    WHERE IdConductor = @IdConductor;

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
        'Viaje aceptado',
        @EstadoAnterior,
        'En camino al pasajero',
        @TarifaAnterior,
        @TarifaNueva,
        @UsuarioSistema,
        'El conductor acepto y va a recoger al pasajero.',
        GETDATE()
    );

    COMMIT TRANSACTION;

    SELECT 1 AS FilasAfectadas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_Contraofertar
    @IdConductor INT,
    @IdViaje INT,
    @TarifaContraoferta DECIMAL(10,2),
    @UsuarioSistema VARCHAR(100) = 'App Conductor'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEstadoSolicitado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Solicitado');
    DECLARE @IdEstadoContraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Contraofertado');
    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoCaminoPasajero INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @EstadoAnterior VARCHAR(50);
    DECLARE @TarifaAnterior DECIMAL(10,2);

    IF @TarifaContraoferta <= 0
        THROW 51010, 'La contraoferta debe ser mayor a cero.', 1;

    IF NOT EXISTS (SELECT 1 FROM Conductores WHERE IdConductor = @IdConductor AND Estado = 1 AND Verificado = 1 AND Disponible = 1)
        THROW 51011, 'El conductor no esta aprobado o no esta disponible.', 1;

    IF EXISTS (SELECT 1 FROM Viajes WHERE IdConductor = @IdConductor AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoCaminoPasajero, @IdEstadoEnCurso, @IdEstadoContraofertado))
        THROW 51012, 'El conductor ya tiene un viaje activo.', 1;

    SELECT
        @EstadoAnterior = ev.Nombre,
        @TarifaAnterior = v.TarifaContraoferta
    FROM Viajes v
    INNER JOIN EstadosViaje ev ON ev.IdEstadoViaje = v.IdEstadoViaje
    WHERE v.IdViaje = @IdViaje
        AND v.IdConductor = @IdConductor
        AND v.IdEstadoViaje = @IdEstadoSolicitado;

    IF @EstadoAnterior IS NULL
        THROW 51013, 'Solo se puede contraofertar un viaje solicitado asignado al conductor.', 1;

    BEGIN TRANSACTION;

    UPDATE Viajes
    SET
        IdEstadoViaje = @IdEstadoContraofertado,
        TarifaContraoferta = @TarifaContraoferta,
        TarifaAceptada = NULL,
        TarifaFinal = 0
    WHERE IdViaje = @IdViaje;

    UPDATE Conductores
    SET Disponible = 0
    WHERE IdConductor = @IdConductor;

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
        'Contraoferta',
        @EstadoAnterior,
        'Contraofertado',
        @TarifaAnterior,
        @TarifaContraoferta,
        @UsuarioSistema,
        'El conductor registro una contraoferta y espera respuesta del pasajero demo.',
        GETDATE()
    );

    COMMIT TRANSACTION;

    SELECT 1 AS FilasAfectadas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_AceptarContraofertaPasajero
    @IdViaje INT,
    @UsuarioSistema VARCHAR(100) = 'Simulador Pasajero'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEstadoContraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Contraofertado');
    DECLARE @IdEstadoCaminoPasajero INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @IdConductor INT;
    DECLARE @TarifaContraoferta DECIMAL(10,2);

    SELECT
        @IdConductor = IdConductor,
        @TarifaContraoferta = TarifaContraoferta
    FROM Viajes
    WHERE IdViaje = @IdViaje
        AND IdEstadoViaje = @IdEstadoContraofertado
        AND TarifaContraoferta IS NOT NULL;

    IF @IdConductor IS NULL
        THROW 51020, 'El viaje no tiene una contraoferta pendiente.', 1;

    BEGIN TRANSACTION;

    UPDATE Viajes
    SET
        IdEstadoViaje = @IdEstadoCaminoPasajero,
        TarifaAceptada = @TarifaContraoferta,
        TarifaFinal = @TarifaContraoferta,
        FechaAceptacion = ISNULL(FechaAceptacion, GETDATE())
    WHERE IdViaje = @IdViaje;

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
        'Contraoferta aceptada',
        'Contraofertado',
        'En camino al pasajero',
        NULL,
        @TarifaContraoferta,
        @UsuarioSistema,
        'El pasajero demo acepto la contraoferta. El conductor va al punto de origen.',
        GETDATE()
    );

    COMMIT TRANSACTION;

    SELECT 1 AS FilasAfectadas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_Iniciar
    @IdConductor INT,
    @IdViaje INT,
    @UsuarioSistema VARCHAR(100) = 'App Conductor'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoCaminoPasajero INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @EstadoAnterior VARCHAR(50);
    DECLARE @TarifaAceptada DECIMAL(10,2);

    SELECT
        @EstadoAnterior = ev.Nombre,
        @TarifaAceptada = v.TarifaAceptada
    FROM Viajes v
    INNER JOIN EstadosViaje ev ON ev.IdEstadoViaje = v.IdEstadoViaje
    WHERE v.IdViaje = @IdViaje
        AND v.IdConductor = @IdConductor
        AND v.IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoCaminoPasajero);

    IF @TarifaAceptada IS NULL
        THROW 51040, 'Solo se puede iniciar un viaje cuando el conductor ya va a recoger al pasajero.', 1;

    BEGIN TRANSACTION;

    UPDATE Viajes
    SET
        IdEstadoViaje = @IdEstadoEnCurso,
        FechaInicio = ISNULL(FechaInicio, GETDATE())
    WHERE IdViaje = @IdViaje;

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
        'Viaje iniciado',
        @EstadoAnterior,
        'En curso',
        @TarifaAceptada,
        @TarifaAceptada,
        @UsuarioSistema,
        'El conductor recogio al pasajero e inicio el viaje.',
        GETDATE()
    );

    COMMIT TRANSACTION;

    SELECT 1 AS FilasAfectadas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_Cancelar
    @IdConductor INT,
    @IdViaje INT,
    @Motivo VARCHAR(300),
    @UsuarioSistema VARCHAR(100) = 'App Conductor'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEstadoFinalizado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Finalizado');
    DECLARE @IdEstadoCancelado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Cancelado');
    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoCaminoPasajero INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @IdEstadoContraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Contraofertado');
    DECLARE @EstadoAnterior VARCHAR(50);
    DECLARE @TarifaAnterior DECIMAL(10,2);

    IF LEN(LTRIM(RTRIM(ISNULL(@Motivo, '')))) < 10
        THROW 51030, 'Debe ingresar un motivo de cancelacion de al menos 10 caracteres.', 1;

    SELECT
        @EstadoAnterior = ev.Nombre,
        @TarifaAnterior = v.TarifaAceptada
    FROM Viajes v
    INNER JOIN EstadosViaje ev ON ev.IdEstadoViaje = v.IdEstadoViaje
    WHERE v.IdViaje = @IdViaje
        AND v.IdConductor = @IdConductor
        AND v.IdEstadoViaje NOT IN (@IdEstadoFinalizado, @IdEstadoCancelado);

    IF @EstadoAnterior IS NULL
        THROW 51031, 'No se puede cancelar un viaje finalizado, cancelado o que no pertenece al conductor.', 1;

    BEGIN TRANSACTION;

    UPDATE Viajes
    SET
        IdEstadoViaje = @IdEstadoCancelado,
        FechaCancelacion = ISNULL(FechaCancelacion, GETDATE()),
        FechaFin = ISNULL(FechaFin, GETDATE()),
        MotivoCancelacion = LTRIM(RTRIM(@Motivo))
    WHERE IdViaje = @IdViaje;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Viajes
        WHERE IdConductor = @IdConductor
            AND IdViaje <> @IdViaje
            AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoCaminoPasajero, @IdEstadoEnCurso, @IdEstadoContraofertado)
    )
    BEGIN
        UPDATE Conductores
        SET Disponible = 1
        WHERE IdConductor = @IdConductor;
    END;

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
        'Viaje cancelado',
        @EstadoAnterior,
        'Cancelado',
        @TarifaAnterior,
        @TarifaAnterior,
        @UsuarioSistema,
        LTRIM(RTRIM(@Motivo)),
        GETDATE()
    );

    COMMIT TRANSACTION;

    SELECT 1 AS FilasAfectadas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_Finalizar
    @IdConductor INT,
    @IdViaje INT,
    @UsuarioSistema VARCHAR(100) = 'App Conductor'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoCaminoPasajero INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @IdEstadoContraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Contraofertado');
    DECLARE @IdEstadoFinalizado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Finalizado');
    DECLARE @TarifaAceptada DECIMAL(10,2);

    SELECT @TarifaAceptada = TarifaAceptada
    FROM Viajes
    WHERE IdViaje = @IdViaje
        AND IdConductor = @IdConductor
        AND IdEstadoViaje = @IdEstadoEnCurso;

    IF @TarifaAceptada IS NULL
        THROW 51050, 'Solo se puede finalizar un viaje en curso asignado al conductor.', 1;

    BEGIN TRANSACTION;

    UPDATE Viajes
    SET
        IdEstadoViaje = @IdEstadoFinalizado,
        TarifaFinal = ISNULL(TarifaAceptada, TarifaFinal),
        FechaFin = ISNULL(FechaFin, GETDATE())
    WHERE IdViaje = @IdViaje;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Viajes
        WHERE IdConductor = @IdConductor
            AND IdViaje <> @IdViaje
            AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoCaminoPasajero, @IdEstadoEnCurso, @IdEstadoContraofertado)
    )
    BEGIN
        UPDATE Conductores
        SET Disponible = 1
        WHERE IdConductor = @IdConductor;
    END;

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
        'Viaje finalizado',
        'En curso',
        'Finalizado',
        @TarifaAceptada,
        @TarifaAceptada,
        @UsuarioSistema,
        'El conductor finalizo el viaje.',
        GETDATE()
    );

    COMMIT TRANSACTION;

    SELECT 1 AS FilasAfectadas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Conductores_ResumenOperacion
    @IdConductor INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdEstadoSolicitado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Solicitado');
    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoCaminoPasajero INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @IdEstadoContraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Contraofertado');
    DECLARE @IdEstadoFinalizado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Finalizado');
    DECLARE @MetaDiaria DECIMAL(10, 2) = 60.00;
    DECLARE @InicioHoy DATETIME = CONVERT(DATE, GETDATE());
    DECLARE @InicioManana DATETIME = DATEADD(DAY, 1, CONVERT(DATE, GETDATE()));

    ;WITH Resumen AS
    (
        SELECT
            c.IdConductor,
            c.Estado,
            c.Verificado,
            c.Disponible,
            PedidosDisponibles =
                (
                    SELECT COUNT(1)
                    FROM Viajes v
                    WHERE
                        v.IdConductor = c.IdConductor
                        AND v.IdEstadoViaje = @IdEstadoSolicitado
                ),
            ViajesActivos =
                (
                    SELECT COUNT(1)
                    FROM Viajes v
                    WHERE
                        v.IdConductor = c.IdConductor
                        AND v.IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoCaminoPasajero, @IdEstadoEnCurso, @IdEstadoContraofertado)
                ),
            ViajesHoy =
                (
                    SELECT COUNT(1)
                    FROM Viajes v
                    WHERE
                        v.IdConductor = c.IdConductor
                        AND v.FechaSolicitud >= @InicioHoy
                        AND v.FechaSolicitud < @InicioManana
                ),
            ViajesFinalizadosHoy =
                (
                    SELECT COUNT(1)
                    FROM Viajes v
                    WHERE
                        v.IdConductor = c.IdConductor
                        AND v.IdEstadoViaje = @IdEstadoFinalizado
                        AND v.FechaFin >= @InicioHoy
                        AND v.FechaFin < @InicioManana
                ),
            GananciaHoy =
                (
                    SELECT ISNULL(SUM(v.TarifaFinal), 0)
                    FROM Viajes v
                    WHERE
                        v.IdConductor = c.IdConductor
                        AND v.IdEstadoViaje = @IdEstadoFinalizado
                        AND v.FechaFin >= @InicioHoy
                        AND v.FechaFin < @InicioManana
                )
        FROM Conductores c
        WHERE c.IdConductor = @IdConductor
    )
    SELECT
        IdConductor,
        Conectado = CAST(CASE WHEN Estado = 1 AND Verificado = 1 AND (Disponible = 1 OR ViajesActivos > 0) THEN 1 ELSE 0 END AS BIT),
        Prioridad = CASE
            WHEN Estado = 1 AND Verificado = 1 AND ViajesActivos > 0 THEN 95
            WHEN Estado = 1 AND Verificado = 1 AND Disponible = 1 THEN 91
            WHEN Estado = 1 AND Verificado = 1 THEN 60
            ELSE 0
        END,
        PedidosDisponibles,
        ViajesActivos,
        ViajesHoy,
        ViajesFinalizadosHoy,
        GananciaHoy = CAST(GananciaHoy AS DECIMAL(10, 2)),
        ObjetivoTitulo = CASE
            WHEN GananciaHoy >= @MetaDiaria THEN 'Objetivo diario logrado'
            WHEN ViajesActivos > 0 THEN 'Completa el viaje activo'
            ELSE 'En este momento no hay objetivos activos'
        END,
        ObjetivoDetalle = CASE
            WHEN GananciaHoy >= @MetaDiaria THEN 'Buen trabajo. Sigue disponible para recibir mas pedidos.'
            WHEN ViajesActivos > 0 THEN 'Sigue el flujo: recoger pasajero, iniciar y finalizar.'
            ELSE 'Conoce como obtener un objetivo nuevo'
        END,
        ObjetivoActual = CAST(GananciaHoy AS DECIMAL(10, 2)),
        ObjetivoMeta = @MetaDiaria,
        EstadoOperativo = CASE
            WHEN Estado = 0 OR Verificado = 0 THEN 'Pedidos no disponibles'
            WHEN ViajesActivos > 0 THEN 'Viaje activo'
            WHEN Disponible = 1 THEN 'Pedidos disponibles'
            ELSE 'Pedidos no disponibles'
        END
    FROM Resumen;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Demo_Viajes_CrearSolicitudConductor
    @IdConductor INT,
    @IdTipoServicio INT = NULL,
    @UsuarioSistema VARCHAR(100) = 'Simulador PARABA'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEstadoSolicitado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Solicitado');
    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoCaminoPasajero INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @IdEstadoContraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Contraofertado');
    DECLARE @IdPasajero INT = (SELECT TOP 1 IdPasajero FROM Pasajeros ORDER BY IdPasajero);
    DECLARE @IdVehiculo INT;
    DECLARE @Origen VARCHAR(200);
    DECLARE @Destino VARCHAR(200);
    DECLARE @Tarifa DECIMAL(10,2);
    DECLARE @IdViaje INT;

    IF NOT EXISTS (SELECT 1 FROM Conductores WHERE IdConductor = @IdConductor AND Estado = 1 AND Verificado = 1)
        THROW 51100, 'El conductor demo no esta aprobado.', 1;

    IF EXISTS (SELECT 1 FROM Viajes WHERE IdConductor = @IdConductor AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoCaminoPasajero, @IdEstadoEnCurso, @IdEstadoContraofertado))
        THROW 51101, 'Finaliza o cancela el viaje activo antes de crear otro pedido demo.', 1;

    IF @IdTipoServicio IS NULL
    BEGIN
        SELECT TOP 1 @IdTipoServicio = IdTipoServicio
        FROM Vehiculos
        WHERE IdConductor = @IdConductor
            AND Estado = 1
            AND Verificado = 1
        ORDER BY IdVehiculo;
    END;

    SELECT TOP 1 @IdVehiculo = IdVehiculo
    FROM Vehiculos
    WHERE IdConductor = @IdConductor
        AND IdTipoServicio = @IdTipoServicio
        AND Estado = 1
        AND Verificado = 1
    ORDER BY IdVehiculo;

    IF @IdPasajero IS NULL OR @IdVehiculo IS NULL OR @IdTipoServicio IS NULL
        THROW 51102, 'Faltan pasajero, vehiculo o tipo de servicio para crear el pedido demo.', 1;

    SELECT
        @Origen = CASE @IdTipoServicio
            WHEN 2 THEN 'Demo moto: Universidad Gabriel Rene Moreno'
            WHEN 3 THEN 'Demo confort: Hotel Los Tajibos'
            WHEN 4 THEN 'Demo XL: Terminal Bimodal'
            WHEN 5 THEN 'Demo premium: Equipetrol Norte'
            ELSE 'Demo taxi: Plaza 24 de Septiembre'
        END,
        @Destino = CASE @IdTipoServicio
            WHEN 2 THEN 'Demo moto: Ventura Mall'
            WHEN 3 THEN 'Demo confort: Aeropuerto Viru Viru'
            WHEN 4 THEN 'Demo XL: Fexpocruz'
            WHEN 5 THEN 'Demo premium: Urubo'
            ELSE 'Demo taxi: Ventura Mall'
        END,
        @Tarifa = CASE @IdTipoServicio
            WHEN 2 THEN 15.00
            WHEN 3 THEN 32.00
            WHEN 4 THEN 45.00
            WHEN 5 THEN 58.00
            ELSE 22.50
        END;

    BEGIN TRANSACTION;

    UPDATE Conductores
    SET Disponible = 1
    WHERE IdConductor = @IdConductor;

    INSERT INTO Viajes
    (
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
    )
    VALUES
    (
        @IdPasajero,
        @IdConductor,
        @IdVehiculo,
        @IdTipoServicio,
        @IdEstadoSolicitado,
        @Origen,
        @Destino,
        @Tarifa,
        0,
        @Tarifa,
        @Tarifa,
        NULL,
        NULL,
        GETDATE(),
        NULL,
        NULL
    );

    SET @IdViaje = SCOPE_IDENTITY();

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
        'Pedido demo creado',
        'Nuevo',
        'Solicitado',
        NULL,
        @Tarifa,
        @UsuarioSistema,
        'Solicitud creada para probar el modulo Viajes/Pedidos de la app conductor.',
        GETDATE()
    );

    COMMIT TRANSACTION;

    SELECT @IdViaje AS IdViaje;
END;
GO
