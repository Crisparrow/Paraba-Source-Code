using Microsoft.Data.SqlClient;
using Paraba.DAL.Repositories;
using Paraba.ENTITY.Models;

namespace Paraba.BLL.Services;

public sealed class DriverProfileService
{
    private static readonly HashSet<string> TiposDocumentoPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "CedulaIdentidad",
        "LicenciaConducir",
        "FotoVerificacion",
        "DocumentoVehiculo",
        "RUAT",
        "SOAT",
        "DocumentoMicrobus"
    };

    private readonly ConductorService conductorService = new();
    private readonly VehiculoRepository vehiculoRepository = new();
    private readonly DocumentoConductorRepository documentoRepository = new();
    private readonly TipoServicioService tipoServicioService = new();
    private readonly PerfilConductorRepository perfilRepository = new();

    public List<Vehiculo> ListarVehiculos(int idConductor)
    {
        ValidarConductor(idConductor);
        return vehiculoRepository.Listar()
            .Where(item => item.IdConductor == idConductor && item.Estado)
            .OrderByDescending(item => item.FechaRegistro)
            .ToList();
    }

    public int RegistrarVehiculo(Vehiculo vehiculo)
    {
        ValidarConductor(vehiculo.IdConductor);

        TipoServicio? tipoServicio = tipoServicioService.ListarTiposServicio()
            .FirstOrDefault(item => item.IdTipoServicio == vehiculo.IdTipoServicio && item.Estado);

        if (tipoServicio == null)
        {
            throw new ArgumentException("Debe seleccionar un tipo de servicio activo.");
        }

        vehiculo.Placa = Limpiar(vehiculo.Placa).ToUpperInvariant();
        vehiculo.Marca = Limpiar(vehiculo.Marca);
        vehiculo.Modelo = Limpiar(vehiculo.Modelo);
        vehiculo.Color = Limpiar(vehiculo.Color);

        if (vehiculo.Placa.Length < 4 || vehiculo.Marca.Length < 2 || vehiculo.Modelo.Length < 1 || vehiculo.Color.Length < 2)
        {
            throw new ArgumentException("Complete placa, marca, modelo y color del vehiculo.");
        }

        if (vehiculo.Anio < 1980 || vehiculo.Anio > DateTime.Today.Year + 1)
        {
            throw new ArgumentException("El año del vehiculo no es valido.");
        }

        try
        {
            return vehiculoRepository.Crear(vehiculo);
        }
        catch (SqlException ex)
        {
            throw new ArgumentException(ex.Message, ex);
        }
    }

    public List<DocumentoConductor> ListarDocumentos(int idConductor, bool soloVigentes = true)
    {
        ValidarConductor(idConductor);
        IEnumerable<DocumentoConductor> documentos = documentoRepository.Listar()
            .Where(item => item.IdConductor == idConductor);

        if (soloVigentes)
        {
            documentos = documentos.Where(item => item.EsVigente);
        }

        return documentos.OrderBy(item => item.TipoDocumento).ToList();
    }

    public int RegistrarDocumento(DocumentoConductor documento)
    {
        ValidarConductor(documento.IdConductor);
        documento.TipoDocumento = Limpiar(documento.TipoDocumento);
        documento.NumeroDocumento = Limpiar(documento.NumeroDocumento);

        if (!TiposDocumentoPermitidos.Contains(documento.TipoDocumento))
        {
            throw new ArgumentException("El tipo de documento no esta permitido.");
        }

        if (string.IsNullOrWhiteSpace(documento.UrlArchivo))
        {
            throw new ArgumentException("Debe adjuntar el archivo del documento.");
        }

        if (!string.Equals(documento.TipoDocumento, "FotoVerificacion", StringComparison.OrdinalIgnoreCase) &&
            documento.NumeroDocumento.Length < 3)
        {
            throw new ArgumentException("Debe ingresar un numero de documento valido.");
        }

        if (documento.FechaVencimiento != null && documento.FechaVencimiento.Value.Date <= DateTime.Today)
        {
            throw new ArgumentException("La fecha de vencimiento debe ser futura.");
        }

        try
        {
            return documentoRepository.Crear(documento);
        }
        catch (SqlException ex)
        {
            throw new ArgumentException(ex.Message, ex);
        }
    }

    public bool RecalcularAprobacion(int idConductor)
    {
        ValidarConductor(idConductor);
        return perfilRepository.RecalcularAprobacion(idConductor);
    }

    private void ValidarConductor(int idConductor)
    {
        if (idConductor <= 0 || !conductorService.ListarConductores().Any(item => item.IdConductor == idConductor && item.Estado))
        {
            throw new ArgumentException("Conductor no encontrado o inactivo.");
        }
    }

    private static string Limpiar(string? value) => value?.Trim() ?? string.Empty;
}
