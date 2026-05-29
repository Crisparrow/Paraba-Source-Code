USE ParabaDB;
GO

CREATE TABLE TiposServicio
(
    IdTipoServicio INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(50) NOT NULL,
    Estado BIT NOT NULL
);
GO

CREATE TABLE EstadosViaje
(
    IdEstadoViaje INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(50) NOT NULL,
    Estado BIT NOT NULL
);
GO

CREATE TABLE Conductores
(
    IdConductor INT PRIMARY KEY IDENTITY(1,1),
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
    IdVehiculo INT PRIMARY KEY IDENTITY(1,1),
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

    FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor),
    FOREIGN KEY (IdTipoServicio) REFERENCES TiposServicio(IdTipoServicio)
);
GO

CREATE TABLE Pasajeros
(
    IdPasajero INT PRIMARY KEY IDENTITY(1,1),
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
    IdTarifa INT PRIMARY KEY IDENTITY(1,1),
    IdTipoServicio INT NOT NULL,
    TarifaBase DECIMAL(10,2) NOT NULL,
    CostoPorKilometro DECIMAL(10,2) NOT NULL,
    CostoPorMinuto DECIMAL(10,2) NOT NULL,
    TarifaMinima DECIMAL(10,2) NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL,

    FOREIGN KEY (IdTipoServicio) REFERENCES TiposServicio(IdTipoServicio)
);
GO

CREATE TABLE Viajes
(
    IdViaje INT PRIMARY KEY IDENTITY(1,1),
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

    FOREIGN KEY (IdPasajero) REFERENCES Pasajeros(IdPasajero),
    FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor),
    FOREIGN KEY (IdVehiculo) REFERENCES Vehiculos(IdVehiculo),
    FOREIGN KEY (IdTipoServicio) REFERENCES TiposServicio(IdTipoServicio),
    FOREIGN KEY (IdEstadoViaje) REFERENCES EstadosViaje(IdEstadoViaje)
);
GO

CREATE TABLE Calificaciones
(
    IdCalificacion INT PRIMARY KEY IDENTITY(1,1),
    IdViaje INT NOT NULL,
    IdPasajero INT NOT NULL,
    IdConductor INT NOT NULL,
    Puntaje INT NOT NULL,
    Comentario VARCHAR(300) NOT NULL,
    Estado BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL,

    FOREIGN KEY (IdViaje) REFERENCES Viajes(IdViaje),
    FOREIGN KEY (IdPasajero) REFERENCES Pasajeros(IdPasajero),
    FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor)
);
GO

CREATE TABLE DocumentosConductor
(
    IdDocumentoConductor INT PRIMARY KEY IDENTITY(1,1),
    IdConductor INT NOT NULL,
    TipoDocumento VARCHAR(100) NOT NULL,
    NumeroDocumento VARCHAR(80) NOT NULL,
    UrlArchivo VARCHAR(300) NOT NULL,
    FechaVencimiento DATE NULL,
    EstadoVerificacion VARCHAR(50) NOT NULL,
    Observacion VARCHAR(300) NOT NULL,
    FechaRegistro DATETIME NOT NULL,

    FOREIGN KEY (IdConductor) REFERENCES Conductores(IdConductor)
);
GO