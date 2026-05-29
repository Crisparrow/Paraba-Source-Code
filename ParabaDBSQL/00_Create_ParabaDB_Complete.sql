USE master;
GO

IF DB_ID('ParabaDB') IS NOT NULL
BEGIN
    ALTER DATABASE ParabaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ParabaDB;
END
GO

CREATE DATABASE ParabaDB;
GO

USE ParabaDB;
GO

CREATE TABLE Paises
(
    IdPais INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    CodigoIso VARCHAR(10) NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL
);
GO

CREATE TABLE Departamentos
(
    IdDepartamento INT IDENTITY(1,1) PRIMARY KEY,
    IdPais INT NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT FK_Departamentos_Paises FOREIGN KEY (IdPais) REFERENCES Paises(IdPais)
);
GO

CREATE TABLE Ciudades
(
    IdCiudad INT IDENTITY(1,1) PRIMARY KEY,
    IdDepartamento INT NOT NULL,
    Nombre VARCHAR(120) NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT FK_Ciudades_Departamentos FOREIGN KEY (IdDepartamento) REFERENCES Departamentos(IdDepartamento)
);
GO

CREATE TABLE Zonas
(
    IdZona INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(200) NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    IdCiudad INT NOT NULL,
    CoberturaActiva BIT NOT NULL DEFAULT 1,
    EsZonaRiesgo BIT NOT NULL DEFAULT 0,
    AltaDemanda BIT NOT NULL DEFAULT 0,
    ObservacionOperativa VARCHAR(300) NOT NULL DEFAULT '',
    CONSTRAINT FK_Zonas_Ciudades FOREIGN KEY (IdCiudad) REFERENCES Ciudades(IdCiudad)
);
GO

CREATE TABLE TiposServicio
(
    IdTipoServicio INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Estado BIT NOT NULL
);
GO

CREATE TABLE EstadosViaje
(
    IdEstadoViaje INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Estado BIT NOT NULL
);
GO

CREATE TABLE TiposVia
(
    IdTipoVia INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    PorcentajeIncremento DECIMAL(10,2) NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL
);
GO

CREATE TABLE Conductores
(
    IdConductor INT IDENTITY(1,1) PRIMARY KEY,
    NombreCompleto VARCHAR(150) NOT NULL,
    DocumentoIdentidad VARCHAR(30) NOT NULL,
    Telefono VARCHAR(30) NOT NULL,
    Correo VARCHAR(150) NOT NULL,
    LicenciaConducir VARCHAR(50) NOT NULL,
    FechaVencimientoLicencia DATE NOT NULL,
    Disponible BIT NOT NULL,
    Verificado BIT NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL
);
GO

CREATE TABLE Vehiculos
(
    IdVehiculo INT IDENTITY(1,1) PRIMARY KEY,
    IdConductor INT NOT NULL,
    IdTipoServicio INT NOT NULL,
    Placa VARCHAR(30) NOT NULL,
    Marca VARCHAR(80) NOT NULL,
    Modelo VARCHAR(80) NOT NULL,
    Color VARCHAR(50) NOT NULL,
    Anio INT NOT NULL,
    Verificado BIT NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT FK_Vehiculos_Conductores FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor),
    CONSTRAINT FK_Vehiculos_TiposServicio FOREIGN KEY (IdTipoServicio) REFERENCES TiposServicio(IdTipoServicio)
);
GO

CREATE TABLE Pasajeros
(
    IdPasajero INT IDENTITY(1,1) PRIMARY KEY,
    NombreCompleto VARCHAR(150) NOT NULL,
    DocumentoIdentidad VARCHAR(30) NOT NULL,
    Telefono VARCHAR(30) NOT NULL,
    Correo VARCHAR(150) NOT NULL,
    Verificado BIT NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL
);
GO

CREATE TABLE Tarifas
(
    IdTarifa INT IDENTITY(1,1) PRIMARY KEY,
    IdTipoServicio INT NOT NULL,
    TarifaBase DECIMAL(10,2) NOT NULL,
    CostoPorKilometro DECIMAL(10,2) NOT NULL,
    CostoPorMinuto DECIMAL(10,2) NOT NULL,
    TarifaMinima DECIMAL(10,2) NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT FK_Tarifas_TiposServicio FOREIGN KEY (IdTipoServicio) REFERENCES TiposServicio(IdTipoServicio)
);
GO

CREATE TABLE ComisionesServicio
(
    IdComisionServicio INT IDENTITY(1,1) PRIMARY KEY,
    IdTipoServicio INT NOT NULL,
    PorcentajeComision DECIMAL(10,2) NOT NULL,
    FechaInicioVigencia DATE NOT NULL,
    FechaFinVigencia DATE NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT FK_ComisionesServicio_TiposServicio FOREIGN KEY (IdTipoServicio) REFERENCES TiposServicio(IdTipoServicio)
);
GO

CREATE TABLE ReglasTarifa
(
    IdReglaTarifa INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    TipoRegla VARCHAR(50) NOT NULL,
    IdTipoServicio INT NULL,
    IdZona INT NULL,
    PorcentajeIncremento DECIMAL(10,2) NOT NULL,
    MontoIncremento DECIMAL(10,2) NOT NULL,
    HoraInicio TIME NULL,
    HoraFin TIME NULL,
    Prioridad INT NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT FK_ReglasTarifa_TiposServicio FOREIGN KEY (IdTipoServicio) REFERENCES TiposServicio(IdTipoServicio),
    CONSTRAINT FK_ReglasTarifa_Zonas FOREIGN KEY (IdZona) REFERENCES Zonas(IdZona)
);
GO

CREATE TABLE Viajes
(
    IdViaje INT IDENTITY(1,1) PRIMARY KEY,
    IdPasajero INT NOT NULL,
    IdConductor INT NOT NULL,
    IdVehiculo INT NOT NULL,
    IdTipoServicio INT NOT NULL,
    IdEstadoViaje INT NOT NULL,
    Origen VARCHAR(200) NOT NULL,
    Destino VARCHAR(200) NOT NULL,
    TarifaEstimada DECIMAL(10,2) NOT NULL,
    TarifaFinal DECIMAL(10,2) NOT NULL,
    FechaSolicitud DATETIME NOT NULL,
    FechaInicio DATETIME NULL,
    FechaFin DATETIME NULL,
    TarifaSugerida DECIMAL(10,2) NOT NULL DEFAULT 0,
    TarifaOfertada DECIMAL(10,2) NOT NULL DEFAULT 0,
    TarifaContraoferta DECIMAL(10,2) NULL,
    TarifaAceptada DECIMAL(10,2) NULL,
    CONSTRAINT FK_Viajes_Pasajeros FOREIGN KEY (IdPasajero) REFERENCES Pasajeros(IdPasajero),
    CONSTRAINT FK_Viajes_Conductores FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor),
    CONSTRAINT FK_Viajes_Vehiculos FOREIGN KEY (IdVehiculo) REFERENCES Vehiculos(IdVehiculo),
    CONSTRAINT FK_Viajes_TiposServicio FOREIGN KEY (IdTipoServicio) REFERENCES TiposServicio(IdTipoServicio),
    CONSTRAINT FK_Viajes_EstadosViaje FOREIGN KEY (IdEstadoViaje) REFERENCES EstadosViaje(IdEstadoViaje)
);
GO

CREATE TABLE Calificaciones
(
    IdCalificacion INT IDENTITY(1,1) PRIMARY KEY,
    IdViaje INT NOT NULL,
    IdPasajero INT NOT NULL,
    IdConductor INT NOT NULL,
    Puntaje INT NOT NULL,
    Comentario VARCHAR(300) NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT FK_Calificaciones_Viajes FOREIGN KEY (IdViaje) REFERENCES Viajes(IdViaje),
    CONSTRAINT FK_Calificaciones_Pasajeros FOREIGN KEY (IdPasajero) REFERENCES Pasajeros(IdPasajero),
    CONSTRAINT FK_Calificaciones_Conductores FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor)
);
GO

CREATE TABLE DocumentosConductor
(
    IdDocumentoConductor INT IDENTITY(1,1) PRIMARY KEY,
    IdConductor INT NOT NULL,
    TipoDocumento VARCHAR(100) NOT NULL,
    NumeroDocumento VARCHAR(80) NOT NULL,
    UrlArchivo VARCHAR(300) NOT NULL,
    FechaVencimiento DATE NULL,
    EstadoVerificacion VARCHAR(50) NOT NULL,
    Observacion VARCHAR(300) NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT FK_DocumentosConductor_Conductores FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor)
);
GO

CREATE TABLE LiquidacionesConductores
(
    IdLiquidacionConductor INT IDENTITY(1,1) PRIMARY KEY,
    IdConductor INT NOT NULL,
    FechaDesde DATE NOT NULL,
    FechaHasta DATE NOT NULL,
    PorcentajeComision DECIMAL(10,2) NOT NULL,
    TotalBruto DECIMAL(10,2) NOT NULL,
    TotalComisionParaba DECIMAL(10,2) NOT NULL,
    TotalNetoConductor DECIMAL(10,2) NOT NULL,
    Estado VARCHAR(30) NOT NULL,
    UsuarioCierre VARCHAR(100) NOT NULL,
    FechaCierre DATETIME NOT NULL,
    FechaPago DATETIME NULL,
    Observacion VARCHAR(300) NOT NULL,
    CONSTRAINT FK_LiquidacionesConductores_Conductores FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor)
);
GO

CREATE TABLE LiquidacionesConductoresDetalle
(
    IdLiquidacionConductorDetalle INT IDENTITY(1,1) PRIMARY KEY,
    IdLiquidacionConductor INT NOT NULL,
    IdViaje INT NOT NULL,
    TarifaFinal DECIMAL(10,2) NOT NULL,
    ComisionParaba DECIMAL(10,2) NOT NULL,
    NetoConductor DECIMAL(10,2) NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT FK_LiquidacionesDetalle_Liquidaciones FOREIGN KEY (IdLiquidacionConductor) REFERENCES LiquidacionesConductores(IdLiquidacionConductor),
    CONSTRAINT FK_LiquidacionesDetalle_Viajes FOREIGN KEY (IdViaje) REFERENCES Viajes(IdViaje)
);
GO

CREATE TABLE RolesAdmin
(
    IdRolAdmin INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(200) NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL
);
GO

CREATE TABLE UsuariosAdmin
(
    IdUsuarioAdmin INT IDENTITY(1,1) PRIMARY KEY,
    NombreCompleto VARCHAR(150) NOT NULL,
    Correo VARCHAR(150) NOT NULL,
    PasswordHash VARCHAR(500) NOT NULL,
    PasswordSalt VARCHAR(200) NOT NULL,
    PasswordIterations INT NOT NULL,
    Estado BIT NOT NULL,
    IntentosFallidos INT NOT NULL,
    UltimoAcceso DATETIME NULL,
    FechaRegistro DATETIME NOT NULL
);
GO

CREATE TABLE UsuariosAdminRoles
(
    IdUsuarioAdmin INT NOT NULL,
    IdRolAdmin INT NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT PK_UsuariosAdminRoles PRIMARY KEY (IdUsuarioAdmin, IdRolAdmin),
    CONSTRAINT FK_UsuariosAdminRoles_Usuarios FOREIGN KEY (IdUsuarioAdmin) REFERENCES UsuariosAdmin(IdUsuarioAdmin),
    CONSTRAINT FK_UsuariosAdminRoles_Roles FOREIGN KEY (IdRolAdmin) REFERENCES RolesAdmin(IdRolAdmin)
);
GO

CREATE TABLE AuditoriaAccesosAdmin
(
    IdAuditoriaAccesoAdmin INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuarioAdmin INT NULL,
    Correo VARCHAR(150) NOT NULL,
    Accion VARCHAR(80) NOT NULL,
    Exitoso BIT NOT NULL,
    IpOrigen VARCHAR(80) NOT NULL,
    Observacion VARCHAR(300) NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT FK_AuditoriaAccesosAdmin_Usuarios FOREIGN KEY (IdUsuarioAdmin) REFERENCES UsuariosAdmin(IdUsuarioAdmin)
);
GO

CREATE TABLE AuditoriaAdministrativa
(
    IdAuditoriaAdministrativa INT IDENTITY(1,1) PRIMARY KEY,
    Modulo VARCHAR(100) NOT NULL,
    Accion VARCHAR(120) NOT NULL,
    Entidad VARCHAR(100) NOT NULL,
    IdEntidad INT NULL,
    UsuarioSistema VARCHAR(150) NOT NULL,
    Observacion VARCHAR(500) NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE AuditoriaConductores
(
    IdAuditoriaConductor INT IDENTITY(1,1) PRIMARY KEY,
    IdConductor INT NOT NULL,
    Accion VARCHAR(80) NOT NULL,
    EstadoAnterior VARCHAR(80) NOT NULL,
    EstadoNuevo VARCHAR(80) NOT NULL,
    UsuarioSistema VARCHAR(100) NOT NULL,
    Observacion VARCHAR(300) NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT FK_AuditoriaConductores_Conductores FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor)
);
GO

CREATE TABLE AuditoriaViajes
(
    IdAuditoriaViaje INT IDENTITY(1,1) PRIMARY KEY,
    IdViaje INT NOT NULL,
    Accion VARCHAR(50) NOT NULL,
    EstadoAnterior VARCHAR(50) NOT NULL,
    EstadoNuevo VARCHAR(50) NOT NULL,
    TarifaAnterior DECIMAL(10,2) NULL,
    TarifaNueva DECIMAL(10,2) NULL,
    UsuarioSistema VARCHAR(100) NOT NULL,
    Observacion VARCHAR(300) NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    CONSTRAINT FK_AuditoriaViajes_Viajes FOREIGN KEY (IdViaje) REFERENCES Viajes(IdViaje)
);
GO

CREATE TABLE Reclamos
(
    IdReclamo INT IDENTITY(1,1) PRIMARY KEY,
    IdViaje INT NULL,
    IdPasajero INT NULL,
    IdConductor INT NULL,
    TipoReclamo VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(500) NOT NULL,
    Estado VARCHAR(50) NOT NULL DEFAULT 'Abierto',
    Prioridad VARCHAR(50) NOT NULL DEFAULT 'Media',
    UsuarioRegistro VARCHAR(150) NOT NULL,
    UsuarioCierre VARCHAR(150) NULL,
    ObservacionCierre VARCHAR(500) NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    FechaCierre DATETIME NULL,
    CONSTRAINT FK_Reclamos_Viajes FOREIGN KEY (IdViaje) REFERENCES Viajes(IdViaje),
    CONSTRAINT FK_Reclamos_Pasajeros FOREIGN KEY (IdPasajero) REFERENCES Pasajeros(IdPasajero),
    CONSTRAINT FK_Reclamos_Conductores FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor)
);
GO

INSERT INTO Paises (Nombre, CodigoIso, Estado, FechaRegistro)
VALUES ('Bolivia', 'BO', 1, GETDATE());
GO

INSERT INTO Departamentos (IdPais, Nombre, Estado, FechaRegistro)
VALUES (1, 'Santa Cruz', 1, GETDATE());
GO

INSERT INTO Ciudades (IdDepartamento, Nombre, Estado, FechaRegistro)
VALUES (1, 'Santa Cruz de la Sierra', 1, GETDATE());
GO

INSERT INTO Zonas (Nombre, Descripcion, Estado, FechaRegistro, IdCiudad, CoberturaActiva, EsZonaRiesgo, AltaDemanda, ObservacionOperativa)
VALUES
('Centro', 'Zona central de mayor movimiento', 1, GETDATE(), 1, 1, 0, 0, ''),
('Norte', 'Zona norte de la ciudad', 1, GETDATE(), 1, 1, 0, 0, ''),
('Sur', 'Zona sur de la ciudad', 1, GETDATE(), 1, 1, 0, 0, ''),
('Terminal', 'Zona cercana a la terminal de buses', 1, GETDATE(), 1, 1, 0, 1, 'Alta demanda en horarios de llegada de buses.'),
('Universidad', 'Zona universitaria', 1, GETDATE(), 1, 1, 0, 1, 'Alta demanda en horarios de entrada y salida.');
GO

INSERT INTO TiposServicio (Nombre, Estado)
VALUES ('Taxi', 1), ('Moto taxi', 1);
GO

INSERT INTO EstadosViaje (Nombre, Estado)
VALUES ('Solicitado', 1), ('Aceptado', 1), ('En curso', 1), ('Finalizado', 1), ('Cancelado', 1);
GO

INSERT INTO TiposVia (Nombre, PorcentajeIncremento, Estado, FechaRegistro)
VALUES
('Asfalto', 0, 1, GETDATE()),
('Tierra', 10, 1, GETDATE()),
('Ripio', 8, 1, GETDATE()),
('Mal estado', 15, 1, GETDATE()),
('Zona complicada', 20, 1, GETDATE());
GO

INSERT INTO Conductores
(NombreCompleto, DocumentoIdentidad, Telefono, Correo, LicenciaConducir, FechaVencimientoLicencia, Disponible, Verificado, Estado, FechaRegistro)
VALUES
('Carlos Mendoza', '7845123', '70012345', 'carlos.mendoza@paraba.com', 'LC-1001', '2027-12-31', 1, 1, 1, GETDATE()),
('Ana Rojas', '6954781', '70123456', 'ana.rojas@paraba.com', 'LC-1002', '2028-06-15', 0, 0, 1, GETDATE()),
('Miguel Suarez', '8123901', '70999888', 'miguel.suarez@paraba.com', 'LC-1003', '2027-09-20', 1, 1, 1, GETDATE());
GO

INSERT INTO Vehiculos
(IdConductor, IdTipoServicio, Placa, Marca, Modelo, Color, Anio, Verificado, Estado, FechaRegistro)
VALUES
(1, 1, '5482-ABC', 'Toyota', 'Corolla', 'Blanco', 2020, 1, 1, GETDATE()),
(2, 2, '7621-KLP', 'Honda', 'CB 125F', 'Rojo', 2022, 1, 1, GETDATE()),
(3, 1, '9012-PBB', 'Suzuki', 'Swift', 'Negro', 2021, 1, 1, GETDATE());
GO

INSERT INTO Pasajeros
(NombreCompleto, DocumentoIdentidad, Telefono, Correo, Verificado, Estado, FechaRegistro)
VALUES
('Mariana Vargas', '8123456', '72014578', 'mariana.vargas@correo.com', 1, 1, GETDATE()),
('Jorge Salinas', '7458961', '72123698', 'jorge.salinas@correo.com', 0, 1, GETDATE()),
('Lucia Roca', '8022331', '73334455', 'lucia.roca@correo.com', 1, 1, GETDATE());
GO

INSERT INTO Tarifas
(IdTipoServicio, TarifaBase, CostoPorKilometro, CostoPorMinuto, TarifaMinima, Estado, FechaRegistro)
VALUES
(1, 0, 0.80, 0.30, 8.00, 1, GETDATE()),
(2, 0, 0.50, 0.20, 5.00, 1, GETDATE());
GO

INSERT INTO ComisionesServicio
(IdTipoServicio, PorcentajeComision, FechaInicioVigencia, FechaFinVigencia, Estado, FechaRegistro)
VALUES
(1, 10.00, CAST(GETDATE() AS DATE), NULL, 1, GETDATE()),
(2, 8.00, CAST(GETDATE() AS DATE), NULL, 1, GETDATE());
GO

INSERT INTO ReglasTarifa
(Nombre, TipoRegla, IdTipoServicio, IdZona, PorcentajeIncremento, MontoIncremento, HoraInicio, HoraFin, Prioridad, Estado, FechaRegistro)
VALUES
('Lluvia - Taxi', 'Clima', 1, NULL, 20, 0, NULL, NULL, 1, 1, GETDATE()),
('Lluvia - Moto taxi', 'Clima', 2, NULL, 25, 0, NULL, NULL, 1, 1, GETDATE()),
('Alta demanda general', 'Demanda', NULL, NULL, 30, 0, NULL, NULL, 2, 1, GETDATE()),
('Horario nocturno', 'Horario', NULL, NULL, 15, 0, '22:00', '05:00', 3, 1, GETDATE()),
('Zona Terminal', 'Zona', NULL, 4, 10, 0, NULL, NULL, 4, 1, GETDATE()),
('Zona Universidad', 'Zona', NULL, 5, 5, 0, NULL, NULL, 5, 1, GETDATE());
GO

INSERT INTO Viajes
(IdPasajero, IdConductor, IdVehiculo, IdTipoServicio, IdEstadoViaje, Origen, Destino, TarifaEstimada, TarifaFinal, FechaSolicitud, FechaInicio, FechaFin, TarifaSugerida, TarifaOfertada, TarifaContraoferta, TarifaAceptada)
VALUES
(1, 1, 1, 1, 4, 'Plaza Principal', 'Terminal de Buses', 18.50, 18.50, DATEADD(DAY, -2, GETDATE()), DATEADD(DAY, -2, GETDATE()), DATEADD(DAY, -2, GETDATE()), 18.50, 18.00, NULL, 18.50),
(2, 2, 2, 2, 1, 'Universidad', 'Centro Comercial', 22.00, 0.00, DATEADD(DAY, -1, GETDATE()), NULL, NULL, 22.00, 20.00, 21.00, NULL),
(3, 3, 3, 1, 5, 'Zona Norte', 'Centro', 16.00, 0.00, GETDATE(), NULL, NULL, 16.00, 15.00, NULL, NULL);
GO

INSERT INTO Calificaciones
(IdViaje, IdPasajero, IdConductor, Puntaje, Comentario, Estado, FechaRegistro)
VALUES
(1, 1, 1, 5, 'Conductor puntual y vehiculo limpio.', 1, GETDATE()),
(2, 2, 2, 4, 'Buen servicio de moto taxi.', 1, GETDATE()),
(3, 3, 3, 2, 'El viaje fue cancelado y requiere revision.', 1, GETDATE());
GO

INSERT INTO DocumentosConductor
(IdConductor, TipoDocumento, NumeroDocumento, UrlArchivo, FechaVencimiento, EstadoVerificacion, Observacion, FechaRegistro)
VALUES
(1, 'Licencia de conducir', 'LC-1001', '/documentos/licencia-carlos.pdf', '2027-12-31', 'Aprobado', 'Documento aprobado por administracion.', GETDATE()),
(2, 'Carnet de identidad', '6954781', '/documentos/ci-ana.pdf', NULL, 'Pendiente', 'Pendiente de revision manual.', GETDATE()),
(3, 'Licencia de conducir', 'LC-1003', '/documentos/licencia-miguel.pdf', '2027-09-20', 'Aprobado', 'Documento aprobado por administracion.', GETDATE());
GO

INSERT INTO RolesAdmin (Nombre, Descripcion, Estado, FechaRegistro)
VALUES
('SuperAdmin', 'Acceso total al panel administrativo.', 1, GETDATE()),
('Operaciones', 'Gestion operativa de viajes.', 1, GETDATE()),
('Verificador', 'Revision de conductores y documentos.', 1, GETDATE()),
('Soporte', 'Atencion y soporte operativo.', 1, GETDATE()),
('Finanzas', 'Revision financiera y tarifas.', 1, GETDATE());
GO

INSERT INTO UsuariosAdmin
(NombreCompleto, Correo, PasswordHash, PasswordSalt, PasswordIterations, Estado, IntentosFallidos, UltimoAcceso, FechaRegistro)
VALUES
('Administrador PARABA', 'admin@paraba.com', 'hk5ZX5iGB+B+2ES6/8pguPpeztKFlN+zrscU03oAeKU=', 'yrJIM+fexdBlh84QhvLt6Q==', 100000, 1, 0, NULL, GETDATE());
GO

INSERT INTO UsuariosAdminRoles (IdUsuarioAdmin, IdRolAdmin, FechaRegistro)
VALUES (1, 1, GETDATE());
GO

INSERT INTO AuditoriaAccesosAdmin
(IdUsuarioAdmin, Correo, Accion, Exitoso, IpOrigen, Observacion, FechaRegistro)
VALUES
(1, 'admin@paraba.com', 'Seed', 1, 'Sistema', 'Usuario administrador inicial creado por script.', GETDATE());
GO

INSERT INTO AuditoriaAdministrativa
(Modulo, Accion, Entidad, IdEntidad, UsuarioSistema, Observacion, FechaRegistro)
VALUES
('Sistema', 'Base creada', 'ParabaDB', NULL, 'Script inicial', 'Base de datos completa creada desde script.', GETDATE());
GO

INSERT INTO AuditoriaConductores
(IdConductor, Accion, EstadoAnterior, EstadoNuevo, UsuarioSistema, Observacion, FechaRegistro)
VALUES
(1, 'Verificacion inicial', 'Pendiente', 'Verificado', 'Script inicial', 'Conductor cargado como verificado.', GETDATE()),
(2, 'Revision pendiente', 'Nuevo', 'Pendiente', 'Script inicial', 'Conductor pendiente de validacion.', GETDATE());
GO

INSERT INTO AuditoriaViajes
(IdViaje, Accion, EstadoAnterior, EstadoNuevo, TarifaAnterior, TarifaNueva, UsuarioSistema, Observacion, FechaRegistro)
VALUES
(1, 'Viaje finalizado', 'En curso', 'Finalizado', 18.50, 18.50, 'Script inicial', 'Viaje de ejemplo finalizado.', GETDATE()),
(3, 'Viaje cancelado', 'Solicitado', 'Cancelado', 16.00, 0.00, 'Script inicial', 'Viaje cancelado para prueba de soporte.', GETDATE());
GO

INSERT INTO LiquidacionesConductores
(IdConductor, FechaDesde, FechaHasta, PorcentajeComision, TotalBruto, TotalComisionParaba, TotalNetoConductor, Estado, UsuarioCierre, FechaCierre, FechaPago, Observacion)
VALUES
(1, DATEADD(DAY, -7, CAST(GETDATE() AS DATE)), CAST(GETDATE() AS DATE), 10.00, 18.50, 1.85, 16.65, 'Cerrada', 'admin@paraba.com', GETDATE(), NULL, 'Liquidacion inicial de ejemplo.');
GO

INSERT INTO LiquidacionesConductoresDetalle
(IdLiquidacionConductor, IdViaje, TarifaFinal, ComisionParaba, NetoConductor, FechaRegistro)
VALUES
(1, 1, 18.50, 1.85, 16.65, GETDATE());
GO

INSERT INTO Reclamos
(IdViaje, IdPasajero, IdConductor, TipoReclamo, Descripcion, Estado, Prioridad, UsuarioRegistro, UsuarioCierre, ObservacionCierre, FechaRegistro, FechaCierre)
VALUES
(3, 3, 3, 'Cancelacion', 'Reclamo de ejemplo por viaje cancelado.', 'Abierto', 'Media', 'admin@paraba.com', NULL, NULL, GETDATE(), NULL);
GO

SELECT name AS Tabla
FROM sys.tables
ORDER BY name;
GO
