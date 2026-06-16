USE ParabaDB;
GO

SET IDENTITY_INSERT TiposServicio ON;

IF EXISTS (SELECT 1 FROM TiposServicio WHERE IdTipoServicio = 1)
    UPDATE TiposServicio SET Nombre = 'Taxi economico', Estado = 1 WHERE IdTipoServicio = 1;
ELSE
    INSERT INTO TiposServicio (IdTipoServicio, Nombre, Estado) VALUES (1, 'Taxi economico', 1);

IF EXISTS (SELECT 1 FROM TiposServicio WHERE IdTipoServicio = 2)
    UPDATE TiposServicio SET Nombre = 'Moto taxi', Estado = 1 WHERE IdTipoServicio = 2;
ELSE
    INSERT INTO TiposServicio (IdTipoServicio, Nombre, Estado) VALUES (2, 'Moto taxi', 1);

IF NOT EXISTS (SELECT 1 FROM TiposServicio WHERE IdTipoServicio = 3)
    INSERT INTO TiposServicio (IdTipoServicio, Nombre, Estado) VALUES (3, 'Taxi confort', 1);

IF NOT EXISTS (SELECT 1 FROM TiposServicio WHERE IdTipoServicio = 4)
    INSERT INTO TiposServicio (IdTipoServicio, Nombre, Estado) VALUES (4, 'Taxi XL', 1);

IF NOT EXISTS (SELECT 1 FROM TiposServicio WHERE IdTipoServicio = 5)
    INSERT INTO TiposServicio (IdTipoServicio, Nombre, Estado) VALUES (5, 'Taxi premium', 1);

SET IDENTITY_INSERT TiposServicio OFF;
GO

UPDATE Vehiculos SET IdTipoServicio = 3 WHERE IdVehiculo = 3;
GO

UPDATE Tarifas SET TarifaBase = 0, CostoPorKilometro = 0.80, CostoPorMinuto = 0.80, TarifaMinima = 10.00, Estado = 1 WHERE IdTipoServicio = 1;
UPDATE Tarifas SET TarifaBase = 0, CostoPorKilometro = 0.50, CostoPorMinuto = 0.40, TarifaMinima = 6.00, Estado = 1 WHERE IdTipoServicio = 2;

IF NOT EXISTS (SELECT 1 FROM Tarifas WHERE IdTipoServicio = 3)
    INSERT INTO Tarifas (IdTipoServicio, TarifaBase, CostoPorKilometro, CostoPorMinuto, TarifaMinima, Estado, FechaRegistro)
    VALUES (3, 0, 1.00, 0.90, 12.00, 1, GETDATE());
ELSE
    UPDATE Tarifas SET TarifaBase = 0, CostoPorKilometro = 1.00, CostoPorMinuto = 0.90, TarifaMinima = 12.00, Estado = 1 WHERE IdTipoServicio = 3;

IF NOT EXISTS (SELECT 1 FROM Tarifas WHERE IdTipoServicio = 4)
    INSERT INTO Tarifas (IdTipoServicio, TarifaBase, CostoPorKilometro, CostoPorMinuto, TarifaMinima, Estado, FechaRegistro)
    VALUES (4, 0, 1.20, 1.00, 15.00, 1, GETDATE());
ELSE
    UPDATE Tarifas SET TarifaBase = 0, CostoPorKilometro = 1.20, CostoPorMinuto = 1.00, TarifaMinima = 15.00, Estado = 1 WHERE IdTipoServicio = 4;

IF NOT EXISTS (SELECT 1 FROM Tarifas WHERE IdTipoServicio = 5)
    INSERT INTO Tarifas (IdTipoServicio, TarifaBase, CostoPorKilometro, CostoPorMinuto, TarifaMinima, Estado, FechaRegistro)
    VALUES (5, 0, 1.60, 1.20, 20.00, 1, GETDATE());
ELSE
    UPDATE Tarifas SET TarifaBase = 0, CostoPorKilometro = 1.60, CostoPorMinuto = 1.20, TarifaMinima = 20.00, Estado = 1 WHERE IdTipoServicio = 5;
GO

IF NOT EXISTS (SELECT 1 FROM ComisionesServicio WHERE IdTipoServicio = 3)
    INSERT INTO ComisionesServicio (IdTipoServicio, PorcentajeComision, FechaInicioVigencia, FechaFinVigencia, Estado, FechaRegistro)
    VALUES (3, 11.00, CAST(GETDATE() AS DATE), NULL, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM ComisionesServicio WHERE IdTipoServicio = 4)
    INSERT INTO ComisionesServicio (IdTipoServicio, PorcentajeComision, FechaInicioVigencia, FechaFinVigencia, Estado, FechaRegistro)
    VALUES (4, 12.00, CAST(GETDATE() AS DATE), NULL, 1, GETDATE());

IF NOT EXISTS (SELECT 1 FROM ComisionesServicio WHERE IdTipoServicio = 5)
    INSERT INTO ComisionesServicio (IdTipoServicio, PorcentajeComision, FechaInicioVigencia, FechaFinVigencia, Estado, FechaRegistro)
    VALUES (5, 15.00, CAST(GETDATE() AS DATE), NULL, 1, GETDATE());
GO

SELECT IdTipoServicio, Nombre, Estado FROM TiposServicio ORDER BY IdTipoServicio;
SELECT IdTipoServicio, CostoPorKilometro, CostoPorMinuto, TarifaMinima FROM Tarifas ORDER BY IdTipoServicio;
GO

