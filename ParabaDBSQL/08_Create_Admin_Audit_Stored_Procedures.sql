USE ParabaDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_AuditoriaAdministrativa_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdAuditoriaAdministrativa,
        Modulo,
        Accion,
        Entidad,
        IdEntidad,
        UsuarioSistema,
        Observacion,
        FechaRegistro
    FROM AuditoriaAdministrativa
    ORDER BY FechaRegistro DESC, IdAuditoriaAdministrativa DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_AuditoriaAdministrativa_Registrar
    @Modulo VARCHAR(100),
    @Accion VARCHAR(120),
    @Entidad VARCHAR(100),
    @IdEntidad INT = NULL,
    @UsuarioSistema VARCHAR(150),
    @Observacion VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AuditoriaAdministrativa
    (
        Modulo,
        Accion,
        Entidad,
        IdEntidad,
        UsuarioSistema,
        Observacion,
        FechaRegistro
    )
    VALUES
    (
        @Modulo,
        @Accion,
        @Entidad,
        @IdEntidad,
        @UsuarioSistema,
        @Observacion,
        GETDATE()
    );

    SELECT SCOPE_IDENTITY() AS IdAuditoriaAdministrativa;
END;
GO
