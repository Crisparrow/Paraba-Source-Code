USE ParabaDB;
GO

INSERT INTO TiposServicio (Nombre, Estado)
VALUES
('Taxi', 1),
('Moto taxi', 1);
GO

INSERT INTO EstadosViaje (Nombre, Estado)
VALUES
('Solicitado', 1),
('Aceptado', 1),
('En curso', 1),
('Finalizado', 1),
('Cancelado', 1);
GO

INSERT INTO Conductores
(
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
)
VALUES
(
    'Carlos Mendoza',
    '7845123',
    '70012345',
    'carlos.mendoza@paraba.com',
    'LC-1001',
    '2027-12-31',
    1,
    1,
    1,
    GETDATE()
),
(
    'Ana Rojas',
    '6954781',
    '70123456',
    'ana.rojas@paraba.com',
    'LC-1002',
    '2028-06-15',
    0,
    1,
    1,
    GETDATE()
);
GO

INSERT INTO Vehiculos
(
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
)
VALUES
(
    1,
    1,
    '5482-ABC',
    'Toyota',
    'Corolla',
    'Blanco',
    2020,
    1,
    1,
    GETDATE()
),
(
    2,
    2,
    '7621-KLP',
    'Honda',
    'CB 125F',
    'Rojo',
    2022,
    1,
    1,
    GETDATE()
);
GO

INSERT INTO Pasajeros
(
    NombreCompleto,
    DocumentoIdentidad,
    Telefono,
    Correo,
    Verificado,
    Estado,
    FechaRegistro
)
VALUES
(
    'Mariana Vargas',
    '8123456',
    '72014578',
    'mariana.vargas@correo.com',
    1,
    1,
    GETDATE()
),
(
    'Jorge Salinas',
    '7458961',
    '72123698',
    'jorge.salinas@correo.com',
    0,
    1,
    GETDATE()
);
GO

INSERT INTO Tarifas
(
    IdTipoServicio,
    TarifaBase,
    CostoPorKilometro,
    CostoPorMinuto,
    TarifaMinima,
    Estado,
    FechaRegistro
)
VALUES
(
    1,
    8.00,
    3.00,
    0.80,
    10.00,
    1,
    GETDATE()
),
(
    2,
    4.00,
    1.80,
    0.40,
    6.00,
    1,
    GETDATE()
);
GO

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
    FechaSolicitud,
    FechaInicio,
    FechaFin
)
VALUES
(
    1,
    1,
    1,
    1,
    4,
    'Plaza Principal',
    'Terminal de Buses',
    18.50,
    18.50,
    GETDATE(),
    GETDATE(),
    GETDATE()
),
(
    2,
    2,
    2,
    2,
    1,
    'Universidad',
    'Centro Comercial',
    22.00,
    0,
    GETDATE(),
    NULL,
    NULL
);
GO

INSERT INTO Calificaciones
(
    IdViaje,
    IdPasajero,
    IdConductor,
    Puntaje,
    Comentario,
    Estado,
    FechaRegistro
)
VALUES
(
    1,
    1,
    1,
    5,
    'Conductor puntual y vehiculo limpio.',
    1,
    GETDATE()
),
(
    2,
    2,
    2,
    4,
    'Buen servicio de moto taxi.',
    1,
    GETDATE()
);
GO

INSERT INTO DocumentosConductor
(
    IdConductor,
    TipoDocumento,
    NumeroDocumento,
    UrlArchivo,
    FechaVencimiento,
    EstadoVerificacion,
    Observacion,
    FechaRegistro
)
VALUES
(
    1,
    'Licencia de conducir',
    'LC-1001',
    '/documentos/licencia-carlos.pdf',
    '2027-12-31',
    'Aprobado',
    'Documento vigente.',
    GETDATE()
),
(
    2,
    'Carnet de identidad',
    '6954781',
    '/documentos/ci-ana.pdf',
    NULL,
    'Pendiente',
    'Pendiente de revision manual.',
    GETDATE()
);
GO
