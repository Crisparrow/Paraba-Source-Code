USE ParabaDB;
GO

IF NOT EXISTS (SELECT 1 FROM EstadosViaje WHERE Nombre = 'Contraofertado')
BEGIN
    INSERT INTO EstadosViaje (Nombre, Estado)
    VALUES ('Contraofertado', 1);
END;
GO

IF COL_LENGTH('dbo.Viajes', 'FechaAceptacion') IS NULL
BEGIN
    ALTER TABLE Viajes ADD FechaAceptacion DATETIME NULL;
END;
GO

IF COL_LENGTH('dbo.Viajes', 'FechaCancelacion') IS NULL
BEGIN
    ALTER TABLE Viajes ADD FechaCancelacion DATETIME NULL;
END;
GO

IF COL_LENGTH('dbo.Viajes', 'MotivoCancelacion') IS NULL
BEGIN
    ALTER TABLE Viajes ADD MotivoCancelacion VARCHAR(300) NULL;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_DisponiblesPorConductor
    @IdConductor INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdEstadoSolicitado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Solicitado');
    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');

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
            AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoEnCurso)
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
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');

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
        AND v.IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoEnCurso)
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
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @EstadoAnterior VARCHAR(50);
    DECLARE @TarifaAnterior DECIMAL(10,2);
    DECLARE @TarifaNueva DECIMAL(10,2);

    IF NOT EXISTS (SELECT 1 FROM Conductores WHERE IdConductor = @IdConductor AND Estado = 1 AND Verificado = 1 AND Disponible = 1)
        THROW 51000, 'El conductor no esta aprobado o no esta disponible.', 1;

    IF EXISTS (SELECT 1 FROM Viajes WHERE IdConductor = @IdConductor AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoEnCurso))
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
        IdEstadoViaje = @IdEstadoAceptado,
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
        'Aceptado',
        @TarifaAnterior,
        @TarifaNueva,
        @UsuarioSistema,
        'El conductor acepto la solicitud de viaje.',
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
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @EstadoAnterior VARCHAR(50);
    DECLARE @TarifaAnterior DECIMAL(10,2);

    IF @TarifaContraoferta <= 0
        THROW 51010, 'La contraoferta debe ser mayor a cero.', 1;

    IF NOT EXISTS (SELECT 1 FROM Conductores WHERE IdConductor = @IdConductor AND Estado = 1 AND Verificado = 1 AND Disponible = 1)
        THROW 51011, 'El conductor no esta aprobado o no esta disponible.', 1;

    IF EXISTS (SELECT 1 FROM Viajes WHERE IdConductor = @IdConductor AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoEnCurso))
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
        'El conductor registro una contraoferta.',
        GETDATE()
    );

    COMMIT TRANSACTION;

    SELECT 1 AS FilasAfectadas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_AceptarContraofertaPasajero
    @IdViaje INT,
    @UsuarioSistema VARCHAR(100) = 'App Pasajero'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEstadoContraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Contraofertado');
    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
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

    IF NOT EXISTS (SELECT 1 FROM Conductores WHERE IdConductor = @IdConductor AND Estado = 1 AND Verificado = 1 AND Disponible = 1)
        THROW 51021, 'El conductor no esta aprobado o no esta disponible.', 1;

    IF EXISTS (SELECT 1 FROM Viajes WHERE IdConductor = @IdConductor AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoEnCurso))
        THROW 51022, 'El conductor ya tiene un viaje activo.', 1;

    BEGIN TRANSACTION;

    UPDATE Viajes
    SET
        IdEstadoViaje = @IdEstadoAceptado,
        TarifaAceptada = @TarifaContraoferta,
        TarifaFinal = @TarifaContraoferta,
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
        'Contraoferta aceptada',
        'Contraofertado',
        'Aceptado',
        NULL,
        @TarifaContraoferta,
        @UsuarioSistema,
        'El pasajero acepto la contraoferta del conductor.',
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
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
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
            AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoEnCurso)
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

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_Iniciar
    @IdConductor INT,
    @IdViaje INT,
    @UsuarioSistema VARCHAR(100) = 'App Conductor'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @TarifaAceptada DECIMAL(10,2);

    SELECT @TarifaAceptada = TarifaAceptada
    FROM Viajes
    WHERE IdViaje = @IdViaje
        AND IdConductor = @IdConductor
        AND IdEstadoViaje = @IdEstadoAceptado;

    IF @TarifaAceptada IS NULL
        THROW 51040, 'Solo se puede iniciar un viaje aceptado asignado al conductor.', 1;

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
        'Aceptado',
        'En curso',
        @TarifaAceptada,
        @TarifaAceptada,
        @UsuarioSistema,
        'El conductor inicio el viaje.',
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

    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @IdEstadoFinalizado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Finalizado');
    DECLARE @IdEstadoCancelado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Cancelado');
    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
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
            AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoEnCurso)
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
