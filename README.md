# PARABA

Sistema administrativo web de PARABA para taxi y moto taxi.

## Requisitos

- Visual Studio 2022 actualizado
- .NET 8 SDK
- SQL Server
- SQL Server Management Studio
- Carga de trabajo .NET MAUI para la app movil
- Android SDK y Java 17 JDK para compilar Android

## Como abrir el proyecto

1. Clonar este repositorio.
2. Abrir Visual Studio 2022.
3. Abrir la solucion:
   `Paraba.UI/Paraba.slnx`
4. Restaurar/compilar la solucion.
5. Crear la base de datos ejecutando `00_Create_ParabaDB_Complete.sql` y después los scripts incrementales `01` a `17` en orden.
6. Ejecutar el proyecto `Paraba.UI` para el panel administrativo.
7. Ejecutar el proyecto `Paraba.API` para la base que consumiran las apps moviles.
8. Ejecutar el proyecto `Paraba.DriverApp` para la app del conductor.

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

`Server=localhost\\SQLEXPRESS;Database=ParabaDB;Trusted_Connection=True;TrustServerCertificate=True;`

También puede definirse la variable de entorno `PARABA_DB_CONNECTION` sin modificar el código.

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
- `POST /api/conductores/{idConductor}/vehiculos`
- `GET /api/conductores/{idConductor}/vehiculos`
- `POST /api/conductores/{idConductor}/documentos` (`multipart/form-data`)
- `GET /api/conductores/{idConductor}/documentos`
- `GET /api/tipos-servicio`
- `GET /api/conductores/{idConductor}/viajes`
- `GET /api/conductores/{idConductor}/viajes/activos`
- `POST /api/conductores/{idConductor}/viajes/{idViaje}/contraoferta`
- `POST /api/conductores/{idConductor}/viajes/{idViaje}/iniciar`
- `POST /api/conductores/{idConductor}/viajes/{idViaje}/finalizar`
- `POST /api/conductores/{idConductor}/viajes/{idViaje}/cancelar`
- `POST /api/viajes/solicitudes`
- `POST /api/viajes/{idViaje}/contraoferta/respuesta`
- `GET /api/servicios-aliados/microbuses/rutas`
- `GET /api/servicios-aliados/mototaxis/asociaciones`

Actualizaciones de viajes en tiempo real:

- Hub SignalR: `/hubs/trips?idConductor={idConductor}`

Esta API es la base para la app Android/iPhone del conductor. El panel administrativo no debe usarse como app movil.

### Archivos de conductores

En producción, definir la variable de entorno `PARABA_AZURE_BLOB_CONNECTION_STRING`.
Opcionalmente pueden definirse `PARABA_AZURE_BLOB_CONTAINER` y `PARABA_API_PUBLIC_BASE_URL`.
Los blobs se crean privados y se descargan a través de la API. Si la cadena Azure no está configurada,
la API usa `wwwroot/uploads` solamente como respaldo local de desarrollo; la app nunca se conecta directamente a SQL Server.

Para asignar una ruta de línea blanca, el conductor debe tener un vehículo de categoría `Microbus`,
el documento `DocumentoMicrobus`, el resto de documentos obligatorios y todas las revisiones aprobadas.

## App movil del conductor

Proyecto:

`Paraba.UI/Paraba.DriverApp`

Estado actual:

- Proyecto .NET MAUI en .NET 8.
- Compila para Windows.
- Compila para Android.
- Pantallas funcionales de Pedidos y Perfil, con vehículo, documentos, foto de verificación y estados de aprobación.

Notas:

- Android se puede compilar desde Windows con Android SDK y Java 17.
- iPhone/iOS comparte el mismo codigo MAUI, pero para compilar o publicar iOS se necesitara una Mac.
- La app movil consumira `Paraba.API`; no debe consultar SQL Server directamente.
