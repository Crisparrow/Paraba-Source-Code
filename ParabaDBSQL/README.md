# Scripts de base de datos PARABA

Para instalar la base completa desde cero, ejecutar solo:

1. `00_Create_ParabaDB_Complete.sql`

Ese script crea la base `ParabaDB`, crea todas las tablas actuales del panel administrativo y carga datos iniciales.

Usuario inicial del panel:

- Correo: `admin@paraba.com`
- Password: `ParabaAdmin2026!`

Importante:

- Si ya existe una base llamada `ParabaDB`, el script la elimina y la vuelve a crear.
- Ejecutarlo en SQL Server Management Studio con una conexion local a SQL Server.
- Los scripts dentro de `Legacy` son antiguos y quedan solo como referencia; no son necesarios para levantar el proyecto actual.
