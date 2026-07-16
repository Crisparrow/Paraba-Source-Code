USE ParabaDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Conductores_ResumenOperacion
    @IdConductor INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdEstadoSolicitado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Solicitado');
    DECLARE @IdEstadoAceptado INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'Aceptado');
    DECLARE @IdEstadoEnCurso INT = (SELECT TOP 1 IdEstadoViaje FROM EstadosViaje WHERE Nombre = 'En curso');
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
                        AND v.IdEstadoViaje IN (@IdEstadoAceptado, @IdEstadoEnCurso)
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
            WHEN ViajesActivos > 0 THEN 'Finaliza el viaje para sumar ganancias al historial.'
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
