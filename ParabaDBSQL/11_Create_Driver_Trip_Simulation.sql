USE ParabaDB;
GO

DECLARE @IdConductor INT = 1;
DECLARE @TelefonoNormalizado VARCHAR(30) = '+59170012345';
DECLARE @IdPasajero INT = 1;
DECLARE @IdVehiculo INT = 1;
DECLARE @IdTipoServicio INT = 1;
DECLARE @IdEstadoSolicitado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Solicitado');

UPDATE Conductores
SET
    Disponible = 1,
    Verificado = 1,
    Estado = 1
WHERE IdConductor = @IdConductor;

IF NOT EXISTS (SELECT 1 FROM SolicitudesRegistroConductor WHERE Telefono = @TelefonoNormalizado)
BEGIN
    INSERT INTO SolicitudesRegistroConductor
    (
        IdConductor,
        Telefono,
        NombreCompleto,
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
    SELECT
        c.IdConductor,
        @TelefonoNormalizado,
        c.NombreCompleto,
        'Aprobado',
        'Aprobado',
        'Aprobado',
        'Aprobado',
        '',
        '',
        '',
        '',
        GETDATE(),
        GETDATE()
    FROM Conductores c
    WHERE c.IdConductor = @IdConductor;
END
ELSE
BEGIN
    UPDATE SolicitudesRegistroConductor
    SET
        IdConductor = @IdConductor,
        EstadoSolicitud = 'Aprobado',
        EstadoDatosConductor = 'Aprobado',
        EstadoDatosVehiculo = 'Aprobado',
        EstadoDocumentos = 'Aprobado',
        FechaActualizacion = GETDATE()
    WHERE Telefono = @TelefonoNormalizado;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM Viajes
    WHERE IdConductor = @IdConductor
        AND IdEstadoViaje = @IdEstadoSolicitado
        AND Origen = 'Simulacion: Plaza 24 de Septiembre'
        AND Destino = 'Simulacion: Ventura Mall'
)
BEGIN
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
        'Simulacion: Plaza 24 de Septiembre',
        'Simulacion: Ventura Mall',
        22.50,
        0,
        22.50,
        22.50,
        NULL,
        NULL,
        GETDATE(),
        NULL,
        NULL
    );

    DECLARE @IdViaje INT = SCOPE_IDENTITY();

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
        'Solicitud simulada',
        'Nuevo',
        'Solicitado',
        NULL,
        22.50,
        'QA PARABA',
        'Viaje creado para probar la ventana Pedidos de la app conductor.',
        GETDATE()
    );
END;

SELECT
    c.IdConductor,
    c.NombreCompleto,
    c.Disponible,
    s.Telefono,
    s.EstadoSolicitud
FROM Conductores c
LEFT JOIN SolicitudesRegistroConductor s ON s.IdConductor = c.IdConductor
WHERE c.IdConductor = @IdConductor;

SELECT
    IdViaje,
    IdConductor,
    IdEstadoViaje,
    Origen,
    Destino,
    TarifaOfertada,
    FechaSolicitud
FROM Viajes
WHERE IdConductor = @IdConductor
    AND IdEstadoViaje = @IdEstadoSolicitado
ORDER BY IdViaje DESC;
GO
