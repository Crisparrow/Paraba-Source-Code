# PARABA

Sistema administrativo web de PARABA para taxi y moto taxi.

## Requisitos

- Visual Studio
- .NET 10
- SQL Server
- SQL Server Management Studio

## Como abrir el proyecto

1. Clonar este repositorio.
2. Abrir Visual Studio.
3. Abrir la solucion:
   `Paraba.UI/Paraba.slnx`
4. Restaurar/compilar la solucion.
5. Crear la base de datos ejecutando en SQL Server Management Studio:
   `ParabaDBSQL/00_Create_ParabaDB_Complete.sql`
6. Ejecutar el proyecto `Paraba.UI`.

## Acceso inicial

- Correo: `admin@paraba.com`
- Password: `ParabaAdmin2026!`

## Conexion SQL Server

La cadena esta en:

`Paraba.UI/Paraba.DAL/Connections/ConexionDAL.cs`

Valor esperado para SQL Server local:

`Server=localhost;Database=ParabaDB;Trusted_Connection=True;TrustServerCertificate=True;`

Si el servidor SQL de otra computadora tiene otro nombre, cambiar `localhost`.

## Base de datos

El script principal y completo es:

`ParabaDBSQL/00_Create_ParabaDB_Complete.sql`

Los scripts dentro de `ParabaDBSQL/Legacy` son antiguos y solo quedan como referencia.
