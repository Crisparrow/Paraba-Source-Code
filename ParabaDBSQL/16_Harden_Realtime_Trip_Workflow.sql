USE ParabaDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Viajes_CrearSolicitudPasajero
    @IdPasajero INT,
    @IdConductor INT,
    @IdVehiculo INT,
    @IdTipoServicio INT,
    @Origen VARCHAR(200),
    @Destino VARCHAR(200),
    @TarifaSugerida DECIMAL(10,2),
    @TarifaOfertada DECIMAL(10,2),
    @UsuarioSistema VARCHAR(100) = 'API Pasajero'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdEstadoSolicitado INT = (SELECT TOP 1 IdEstadoViaje FROM dbo.EstadosViaje WHERE Nombre = 'Solicitado');
    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM dbo.EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM dbo.EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @IdEstadoContraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM dbo.EstadosViaje WHERE Nombre = 'Contraofertado');
    DECLARE @IdEstadoCamino INT = (SELECT TOP 1 IdEstadoViaje FROM dbo.EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @IdViaje INT;

    IF LEN(LTRIM(RTRIM(ISNULL(@Origen, '')))) < 3 OR LEN(LTRIM(RTRIM(ISNULL(@Destino, '')))) < 3
        THROW 51200, 'Origen y destino son obligatorios.', 1;

    IF @TarifaSugerida <= 0 OR @TarifaOfertada <= 0
        THROW 51201, 'La tarifa debe ser mayor a cero.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Pasajeros WHERE IdPasajero = @IdPasajero AND Estado = 1)
        THROW 51202, 'El pasajero no existe o esta inactivo.', 1;

    BEGIN TRANSACTION;

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.Conductores WITH (UPDLOCK, HOLDLOCK)
        WHERE IdConductor = @IdConductor AND Estado = 1 AND Verificado = 1 AND Disponible = 1
    )
        THROW 51203, 'El conductor no esta aprobado o no esta disponible.', 1;

    IF EXISTS
    (
        SELECT 1 FROM dbo.Viajes
        WHERE IdConductor = @IdConductor
          AND IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoEnCurso, @IdEstadoContraofertado, @IdEstadoCamino)
    )
        THROW 51204, 'El conductor ya tiene un viaje activo.', 1;

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.Vehiculos
        WHERE IdVehiculo = @IdVehiculo AND IdConductor = @IdConductor
          AND IdTipoServicio = @IdTipoServicio AND Estado = 1 AND Verificado = 1
    )
        THROW 51205, 'El vehiculo no esta aprobado para ese conductor y servicio.', 1;

    INSERT INTO dbo.Viajes
    (
        IdPasajero, IdConductor, IdVehiculo, IdTipoServicio, IdEstadoViaje,
        Origen, Destino, TarifaEstimada, TarifaFinal, TarifaSugerida,
        TarifaOfertada, TarifaContraoferta, TarifaAceptada, FechaSolicitud,
        FechaInicio, FechaFin
    )
    VALUES
    (
        @IdPasajero, @IdConductor, @IdVehiculo, @IdTipoServicio, @IdEstadoSolicitado,
        LTRIM(RTRIM(@Origen)), LTRIM(RTRIM(@Destino)), @TarifaSugerida, 0, @TarifaSugerida,
        @TarifaOfertada, NULL, NULL, GETDATE(), NULL, NULL
    );

    SET @IdViaje = CAST(SCOPE_IDENTITY() AS INT);

    INSERT INTO dbo.AuditoriaViajes
        (IdViaje, Accion, EstadoAnterior, EstadoNuevo, TarifaAnterior, TarifaNueva, UsuarioSistema, Observacion, FechaRegistro)
    VALUES
        (@IdViaje, 'Solicitud creada', 'Nuevo', 'Solicitado', NULL, @TarifaOfertada, @UsuarioSistema,
         'Solicitud de pasajero enviada al conductor en tiempo real.', GETDATE());

    COMMIT TRANSACTION;
    SELECT @IdViaje;
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

    DECLARE @Solicitado INT = (SELECT TOP 1 IdEstadoViaje FROM dbo.EstadosViaje WHERE Nombre = 'Solicitado');
    DECLARE @Aceptado INT = (SELECT TOP 1 IdEstadoViaje FROM dbo.EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @EnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM dbo.EstadosViaje WHERE Nombre = 'En curso');
    DECLARE @Contraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM dbo.EstadosViaje WHERE Nombre = 'Contraofertado');
    DECLARE @Camino INT = (SELECT TOP 1 IdEstadoViaje FROM dbo.EstadosViaje WHERE Nombre = 'En camino al pasajero');
    DECLARE @Tarifa DECIMAL(10,2);

    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM dbo.Conductores WITH (UPDLOCK, HOLDLOCK) WHERE IdConductor = @IdConductor AND Estado = 1 AND Verificado = 1 AND Disponible = 1)
        THROW 51210, 'El conductor no esta aprobado o no esta disponible.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Viajes WHERE IdConductor = @IdConductor AND IdViaje <> @IdViaje AND IdEstadoViaje IN (@Aceptado, @EnCurso, @Contraofertado, @Camino))
        THROW 51211, 'El conductor ya tiene un viaje activo.', 1;

    SELECT @Tarifa = CASE WHEN TarifaOfertada > 0 THEN TarifaOfertada ELSE TarifaSugerida END
    FROM dbo.Viajes WITH (UPDLOCK, HOLDLOCK)
    WHERE IdViaje = @IdViaje AND IdConductor = @IdConductor AND IdEstadoViaje = @Solicitado;

    IF @Tarifa IS NULL
        THROW 51212, 'Solo se puede aceptar un viaje solicitado asignado al conductor.', 1;

    UPDATE dbo.Viajes
    SET IdEstadoViaje = @Aceptado, TarifaAceptada = @Tarifa, TarifaFinal = @Tarifa,
        FechaAceptacion = ISNULL(FechaAceptacion, GETDATE())
    WHERE IdViaje = @IdViaje;

    UPDATE dbo.Conductores SET Disponible = 0 WHERE IdConductor = @IdConductor;

    INSERT INTO dbo.AuditoriaViajes
        (IdViaje, Accion, EstadoAnterior, EstadoNuevo, TarifaAnterior, TarifaNueva, UsuarioSistema, Observacion, FechaRegistro)
    VALUES
        (@IdViaje, 'Viaje aceptado', 'Solicitado', 'Aceptado', NULL, @Tarifa, @UsuarioSistema,
         'El conductor acepto la solicitud.', GETDATE());

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

    DECLARE @Contraofertado INT = (SELECT TOP 1 IdEstadoViaje FROM dbo.EstadosViaje WHERE Nombre = 'Contraofertado');
    DECLARE @Aceptado INT = (SELECT TOP 1 IdEstadoViaje FROM dbo.EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @Tarifa DECIMAL(10,2);

    BEGIN TRANSACTION;

    SELECT @Tarifa = TarifaContraoferta
    FROM dbo.Viajes WITH (UPDLOCK, HOLDLOCK)
    WHERE IdViaje = @IdViaje AND IdEstadoViaje = @Contraofertado AND TarifaContraoferta IS NOT NULL;

    IF @Tarifa IS NULL
        THROW 51220, 'El viaje no tiene una contraoferta pendiente.', 1;

    UPDATE dbo.Viajes
    SET IdEstadoViaje = @Aceptado, TarifaAceptada = @Tarifa, TarifaFinal = @Tarifa,
        FechaAceptacion = ISNULL(FechaAceptacion, GETDATE())
    WHERE IdViaje = @IdViaje;

    INSERT INTO dbo.AuditoriaViajes
        (IdViaje, Accion, EstadoAnterior, EstadoNuevo, TarifaAnterior, TarifaNueva, UsuarioSistema, Observacion, FechaRegistro)
    VALUES
        (@IdViaje, 'Contraoferta aceptada', 'Contraofertado', 'Aceptado', NULL, @Tarifa, @UsuarioSistema,
         'El pasajero acepto la contraoferta del conductor.', GETDATE());

    COMMIT TRANSACTION;
    SELECT 1 AS FilasAfectadas;
END;
GO
