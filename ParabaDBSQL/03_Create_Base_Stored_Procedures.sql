USE ParabaDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Conductores_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdConductor,
        NombreCompleto,
        DocumentoIdentidad,
        Telefono,
        Correo,
        LicenciaConducir,
        FechaVencimientoLicencia,
        Disponible,
        Verificado,
        Estado,
        FechaRegistro
    FROM Conductores
    ORDER BY IdConductor;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Vehiculos_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdVehiculo,
        IdConductor,
        IdTipoServicio,
        Placa,
        Marca,
        Modelo,
        Color,
        Anio,
        Verificado,
        Estado,
        FechaRegistro
    FROM Vehiculos
    ORDER BY IdVehiculo;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_TiposServicio_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdTipoServicio,
        Nombre,
        Estado
    FROM TiposServicio
    ORDER BY IdTipoServicio;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Tarifas_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdTarifa,
        IdTipoServicio,
        TarifaBase,
        CostoPorKilometro,
        CostoPorMinuto,
        TarifaMinima,
        Estado,
        FechaRegistro
    FROM Tarifas
    ORDER BY IdTarifa;
END;
GO
