USE ParabaDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LiquidacionesConductores_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdLiquidacionConductor,
        IdConductor,
        FechaDesde,
        FechaHasta,
        PorcentajeComision,
        TotalBruto,
        TotalComisionParaba,
        TotalNetoConductor,
        Estado,
        UsuarioCierre,
        FechaCierre,
        FechaPago,
        Observacion
    FROM LiquidacionesConductores
    ORDER BY FechaCierre DESC, IdLiquidacionConductor DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LiquidacionesConductores_ListarIdsViajesLiquidados
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT IdViaje
    FROM LiquidacionesConductoresDetalle;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LiquidacionesConductores_ListarDetalles
    @IdLiquidacionConductor INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdLiquidacionConductorDetalle,
        IdLiquidacionConductor,
        IdViaje,
        TarifaFinal,
        ComisionParaba,
        NetoConductor,
        FechaRegistro
    FROM LiquidacionesConductoresDetalle
    WHERE IdLiquidacionConductor = @IdLiquidacionConductor
    ORDER BY IdViaje;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LiquidacionesConductores_MarcarPagada
    @IdLiquidacionConductor INT,
    @Observacion VARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE LiquidacionesConductores
    SET
        Estado = 'Pagada',
        FechaPago = GETDATE(),
        Observacion = @Observacion
    WHERE
        IdLiquidacionConductor = @IdLiquidacionConductor
        AND Estado = 'Cerrada';

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LiquidacionesConductores_Anular
    @IdLiquidacionConductor INT,
    @Observacion VARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE LiquidacionesConductores
    SET
        Estado = 'Anulada',
        Observacion = @Observacion
    WHERE
        IdLiquidacionConductor = @IdLiquidacionConductor
        AND Estado = 'Cerrada';

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LiquidacionesConductores_CrearCabecera
    @IdConductor INT,
    @FechaDesde DATE,
    @FechaHasta DATE,
    @PorcentajeComision DECIMAL(10,2),
    @TotalBruto DECIMAL(10,2),
    @TotalComisionParaba DECIMAL(10,2),
    @TotalNetoConductor DECIMAL(10,2),
    @Estado VARCHAR(30),
    @UsuarioCierre VARCHAR(100),
    @FechaCierre DATETIME,
    @Observacion VARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO LiquidacionesConductores
    (
        IdConductor,
        FechaDesde,
        FechaHasta,
        PorcentajeComision,
        TotalBruto,
        TotalComisionParaba,
        TotalNetoConductor,
        Estado,
        UsuarioCierre,
        FechaCierre,
        Observacion
    )
    OUTPUT INSERTED.IdLiquidacionConductor
    VALUES
    (
        @IdConductor,
        @FechaDesde,
        @FechaHasta,
        @PorcentajeComision,
        @TotalBruto,
        @TotalComisionParaba,
        @TotalNetoConductor,
        @Estado,
        @UsuarioCierre,
        @FechaCierre,
        @Observacion
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_LiquidacionesConductores_CrearDetalle
    @IdLiquidacionConductor INT,
    @IdViaje INT,
    @TarifaFinal DECIMAL(10,2),
    @ComisionParaba DECIMAL(10,2),
    @NetoConductor DECIMAL(10,2),
    @FechaRegistro DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO LiquidacionesConductoresDetalle
    (
        IdLiquidacionConductor,
        IdViaje,
        TarifaFinal,
        ComisionParaba,
        NetoConductor,
        FechaRegistro
    )
    VALUES
    (
        @IdLiquidacionConductor,
        @IdViaje,
        @TarifaFinal,
        @ComisionParaba,
        @NetoConductor,
        @FechaRegistro
    );
END;
GO
