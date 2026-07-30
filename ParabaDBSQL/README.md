# Scripts de base de datos PARABA

Para instalar la base actual desde cero, ejecutar:

1. `00_Create_ParabaDB_Complete.sql`
2. Ejecutar en orden los scripts incrementales `01` a `17`.

El primer script crea la base `ParabaDB`, crea las tablas actuales del panel administrativo y carga datos iniciales.
Los scripts `09` a `17` agregan identidad del conductor, flujo completo de viajes,
actualizaciones en tiempo real, servicios aliados y el perfil verificable con documentos y vehiculos.

Usuario inicial del panel:

- Correo: `admin@paraba.com`
- Password: `ParabaAdmin2026!`

Importante:

- Si ya existe una base llamada `ParabaDB`, el script la elimina y la vuelve a crear.
- Ejecutarlo en SQL Server Management Studio con una conexion local a SQL Server.
- Los scripts dentro de `Legacy` son antiguos y quedan solo como referencia; no son necesarios para levantar el proyecto actual.
