# PARABA

Sistema administrativo web de PARABA para taxi y moto taxi.

## Requisitos

- Visual Studio 2022 actualizado
- .NET 8 SDK
- SQL Server
- SQL Server Management Studio

## Como abrir el proyecto

1. Clonar este repositorio.
2. Abrir Visual Studio 2022.
3. Abrir la solucion:
   `Paraba.UI/Paraba.slnx`
4. Restaurar/compilar la solucion.
5. Crear la base de datos ejecutando en SQL Server Management Studio los scripts de `ParabaDBSQL` en este orden:
   - `00_Create_ParabaDB_Complete.sql`
   - `01_Update_Service_Categories.sql`
   - `02_Create_Test_Admin_Users.sql`
   - `03_Create_Base_Stored_Procedures.sql`
   - `04_Create_Verification_Stored_Procedures.sql`
   - `05_Create_Admin_Trip_Stored_Procedures.sql`
   - `06_Create_Liquidation_Stored_Procedures.sql`
   - `07_Create_Support_Stored_Procedures.sql`
   - `08_Create_Admin_Audit_Stored_Procedures.sql`
6. Ejecutar el proyecto `Paraba.UI` para el panel administrativo.
7. Ejecutar el proyecto `Paraba.API` para la base que consumiran las apps moviles.

## Acceso inicial

- Correo: `admin@paraba.com`
- Password: `ParabaAdmin2026!`

Usuarios de prueba por rol:

- `operaciones@paraba.com` / `ParabaTest2026!`
- `soporte@paraba.com` / `ParabaTest2026!`
- `verificador@paraba.com` / `ParabaTest2026!`
- `finanzas@paraba.com` / `ParabaTest2026!`

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

## API para app del conductor

Proyecto:

`Paraba.UI/Paraba.API`

Endpoints iniciales:

- `GET /api/conductores/{idConductor}/perfil`
- `GET /api/conductores/{idConductor}/viajes`
- `GET /api/conductores/{idConductor}/viajes/activos`
- `POST /api/conductores/{idConductor}/viajes/{idViaje}/contraoferta`
- `POST /api/conductores/{idConductor}/viajes/{idViaje}/iniciar`
- `POST /api/conductores/{idConductor}/viajes/{idViaje}/finalizar`
- `POST /api/conductores/{idConductor}/viajes/{idViaje}/cancelar`

Esta API es la base para la app Android/iPhone del conductor. El panel administrativo no debe usarse como app movil.
