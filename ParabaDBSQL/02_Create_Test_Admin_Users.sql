USE ParabaDB;
GO

-- Usuarios de prueba para validar permisos por rol del panel administrativo.
-- Password para todos: ParabaTest2026!

DECLARE @IdUsuario_Operaciones INT;
IF NOT EXISTS (SELECT 1 FROM UsuariosAdmin WHERE Correo = 'operaciones@paraba.com')
BEGIN
    INSERT INTO UsuariosAdmin (NombreCompleto, Correo, PasswordHash, PasswordSalt, PasswordIterations, Estado, IntentosFallidos, UltimoAcceso, FechaRegistro)
    VALUES ('Operador PARABA', 'operaciones@paraba.com', '6h4+R28yoQPTfyYErp9pF4SBrIcF8w+BefOQ+dUAP+M=', 'CnPk533A1qVCAQ8bCeQhTQ==', 100000, 1, 0, NULL, GETDATE());
    SET @IdUsuario_Operaciones = SCOPE_IDENTITY();
END
ELSE
BEGIN
    SELECT @IdUsuario_Operaciones = IdUsuarioAdmin FROM UsuariosAdmin WHERE Correo = 'operaciones@paraba.com';
    UPDATE UsuariosAdmin SET Estado = 1, PasswordHash = '6h4+R28yoQPTfyYErp9pF4SBrIcF8w+BefOQ+dUAP+M=', PasswordSalt = 'CnPk533A1qVCAQ8bCeQhTQ==', PasswordIterations = 100000 WHERE IdUsuarioAdmin = @IdUsuario_Operaciones;
END
IF NOT EXISTS (SELECT 1 FROM UsuariosAdminRoles ur INNER JOIN RolesAdmin r ON r.IdRolAdmin = ur.IdRolAdmin WHERE ur.IdUsuarioAdmin = @IdUsuario_Operaciones AND r.Nombre = 'Operaciones')
BEGIN
    INSERT INTO UsuariosAdminRoles (IdUsuarioAdmin, IdRolAdmin, FechaRegistro)
    SELECT @IdUsuario_Operaciones, IdRolAdmin, GETDATE() FROM RolesAdmin WHERE Nombre = 'Operaciones';
END
GO

DECLARE @IdUsuario_Soporte INT;
IF NOT EXISTS (SELECT 1 FROM UsuariosAdmin WHERE Correo = 'soporte@paraba.com')
BEGIN
    INSERT INTO UsuariosAdmin (NombreCompleto, Correo, PasswordHash, PasswordSalt, PasswordIterations, Estado, IntentosFallidos, UltimoAcceso, FechaRegistro)
    VALUES ('Soporte PARABA', 'soporte@paraba.com', 'NWRWnm+SatfwW42s0u924DTKPmX0AZpCdhe1KB0pqqg=', '2iLzaJblwtSlQBNPHGvS1g==', 100000, 1, 0, NULL, GETDATE());
    SET @IdUsuario_Soporte = SCOPE_IDENTITY();
END
ELSE
BEGIN
    SELECT @IdUsuario_Soporte = IdUsuarioAdmin FROM UsuariosAdmin WHERE Correo = 'soporte@paraba.com';
    UPDATE UsuariosAdmin SET Estado = 1, PasswordHash = 'NWRWnm+SatfwW42s0u924DTKPmX0AZpCdhe1KB0pqqg=', PasswordSalt = '2iLzaJblwtSlQBNPHGvS1g==', PasswordIterations = 100000 WHERE IdUsuarioAdmin = @IdUsuario_Soporte;
END
IF NOT EXISTS (SELECT 1 FROM UsuariosAdminRoles ur INNER JOIN RolesAdmin r ON r.IdRolAdmin = ur.IdRolAdmin WHERE ur.IdUsuarioAdmin = @IdUsuario_Soporte AND r.Nombre = 'Soporte')
BEGIN
    INSERT INTO UsuariosAdminRoles (IdUsuarioAdmin, IdRolAdmin, FechaRegistro)
    SELECT @IdUsuario_Soporte, IdRolAdmin, GETDATE() FROM RolesAdmin WHERE Nombre = 'Soporte';
END
GO

DECLARE @IdUsuario_Verificador INT;
IF NOT EXISTS (SELECT 1 FROM UsuariosAdmin WHERE Correo = 'verificador@paraba.com')
BEGIN
    INSERT INTO UsuariosAdmin (NombreCompleto, Correo, PasswordHash, PasswordSalt, PasswordIterations, Estado, IntentosFallidos, UltimoAcceso, FechaRegistro)
    VALUES ('Verificador PARABA', 'verificador@paraba.com', 'rVEK4qJ8rGCNP8BTfxLG9vLM7wxV3stNlN4l9xGghMQ=', '9VkOv90v3IhK39wTdWnPKQ==', 100000, 1, 0, NULL, GETDATE());
    SET @IdUsuario_Verificador = SCOPE_IDENTITY();
END
ELSE
BEGIN
    SELECT @IdUsuario_Verificador = IdUsuarioAdmin FROM UsuariosAdmin WHERE Correo = 'verificador@paraba.com';
    UPDATE UsuariosAdmin SET Estado = 1, PasswordHash = 'rVEK4qJ8rGCNP8BTfxLG9vLM7wxV3stNlN4l9xGghMQ=', PasswordSalt = '9VkOv90v3IhK39wTdWnPKQ==', PasswordIterations = 100000 WHERE IdUsuarioAdmin = @IdUsuario_Verificador;
END
IF NOT EXISTS (SELECT 1 FROM UsuariosAdminRoles ur INNER JOIN RolesAdmin r ON r.IdRolAdmin = ur.IdRolAdmin WHERE ur.IdUsuarioAdmin = @IdUsuario_Verificador AND r.Nombre = 'Verificador')
BEGIN
    INSERT INTO UsuariosAdminRoles (IdUsuarioAdmin, IdRolAdmin, FechaRegistro)
    SELECT @IdUsuario_Verificador, IdRolAdmin, GETDATE() FROM RolesAdmin WHERE Nombre = 'Verificador';
END
GO

DECLARE @IdUsuario_Finanzas INT;
IF NOT EXISTS (SELECT 1 FROM UsuariosAdmin WHERE Correo = 'finanzas@paraba.com')
BEGIN
    INSERT INTO UsuariosAdmin (NombreCompleto, Correo, PasswordHash, PasswordSalt, PasswordIterations, Estado, IntentosFallidos, UltimoAcceso, FechaRegistro)
    VALUES ('Finanzas PARABA', 'finanzas@paraba.com', 'sP2Ub5E7Ps1sVMnOomzoLGdsgtmqPgPSL3tq92n7VlA=', 'TGbOauOANZnkCN0GSqYcfw==', 100000, 1, 0, NULL, GETDATE());
    SET @IdUsuario_Finanzas = SCOPE_IDENTITY();
END
ELSE
BEGIN
    SELECT @IdUsuario_Finanzas = IdUsuarioAdmin FROM UsuariosAdmin WHERE Correo = 'finanzas@paraba.com';
    UPDATE UsuariosAdmin SET Estado = 1, PasswordHash = 'sP2Ub5E7Ps1sVMnOomzoLGdsgtmqPgPSL3tq92n7VlA=', PasswordSalt = 'TGbOauOANZnkCN0GSqYcfw==', PasswordIterations = 100000 WHERE IdUsuarioAdmin = @IdUsuario_Finanzas;
END
IF NOT EXISTS (SELECT 1 FROM UsuariosAdminRoles ur INNER JOIN RolesAdmin r ON r.IdRolAdmin = ur.IdRolAdmin WHERE ur.IdUsuarioAdmin = @IdUsuario_Finanzas AND r.Nombre = 'Finanzas')
BEGIN
    INSERT INTO UsuariosAdminRoles (IdUsuarioAdmin, IdRolAdmin, FechaRegistro)
    SELECT @IdUsuario_Finanzas, IdRolAdmin, GETDATE() FROM RolesAdmin WHERE Nombre = 'Finanzas';
END
GO

SELECT u.Correo, r.Nombre AS Rol, u.Estado FROM UsuariosAdmin u INNER JOIN UsuariosAdminRoles ur ON ur.IdUsuarioAdmin = u.IdUsuarioAdmin INNER JOIN RolesAdmin r ON r.IdRolAdmin = ur.IdRolAdmin WHERE u.Correo IN ('operaciones@paraba.com','soporte@paraba.com','verificador@paraba.com','finanzas@paraba.com') ORDER BY r.Nombre;
GO
