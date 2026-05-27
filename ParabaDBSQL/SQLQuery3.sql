USE ParabaDB;
GO

DELETE FROM Calificaciones;
DELETE FROM DocumentosConductor;
DELETE FROM Viajes;
DELETE FROM Tarifas;
DELETE FROM Vehiculos;
DELETE FROM Pasajeros;
DELETE FROM Conductores;
DELETE FROM EstadosViaje;
DELETE FROM TiposServicio;
GO

DBCC CHECKIDENT ('Calificaciones', RESEED, 0);
DBCC CHECKIDENT ('DocumentosConductor', RESEED, 0);
DBCC CHECKIDENT ('Viajes', RESEED, 0);
DBCC CHECKIDENT ('Tarifas', RESEED, 0);
DBCC CHECKIDENT ('Vehiculos', RESEED, 0);
DBCC CHECKIDENT ('Pasajeros', RESEED, 0);
DBCC CHECKIDENT ('Conductores', RESEED, 0);
DBCC CHECKIDENT ('EstadosViaje', RESEED, 0);
DBCC CHECKIDENT ('TiposServicio', RESEED, 0);
GO