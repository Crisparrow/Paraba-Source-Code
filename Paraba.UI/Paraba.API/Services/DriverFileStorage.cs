using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Paraba.API.Services;

public sealed class DriverFileStorage
{
    private readonly string connectionString;
    private readonly string containerName;
    private readonly IWebHostEnvironment environment;
    private readonly string publicApiBaseUrl;

    public DriverFileStorage(IConfiguration configuration, IWebHostEnvironment environment)
    {
        connectionString = Environment.GetEnvironmentVariable("PARABA_AZURE_BLOB_CONNECTION_STRING")
            ?? configuration["AzureBlobStorage:ConnectionString"]
            ?? string.Empty;
        containerName = Environment.GetEnvironmentVariable("PARABA_AZURE_BLOB_CONTAINER")
            ?? configuration["AzureBlobStorage:ContainerName"]
            ?? "driver-documents";
        publicApiBaseUrl = Environment.GetEnvironmentVariable("PARABA_API_PUBLIC_BASE_URL")
            ?? configuration["AzureBlobStorage:PublicApiBaseUrl"]
            ?? "http://127.0.0.1:5183";
        this.environment = environment;
    }

    public bool UsesAzure => !string.IsNullOrWhiteSpace(connectionString);

    public async Task<string> SaveAsync(int idConductor, string documentType, IFormFile file)
    {
        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        string safeType = new string(documentType.Where(char.IsLetterOrDigit).ToArray());
        string blobName = $"conductores/{idConductor}/{safeType}/{Guid.NewGuid():N}{extension}";

        if (UsesAzure)
        {
            BlobContainerClient container = new(connectionString, containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.None);
            BlobClient blob = container.GetBlobClient(blobName);
            await using Stream stream = file.OpenReadStream();
            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            return BuildDownloadUrl(blobName);
        }

        string webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        string relativePath = blobName.Replace('/', Path.DirectorySeparatorChar);
        string absolutePath = Path.Combine(webRoot, "uploads", relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
        await using FileStream output = File.Create(absolutePath);
        await file.CopyToAsync(output);
        return BuildDownloadUrl(blobName);
    }

    public async Task<StoredDriverFile?> OpenAsync(string token)
    {
        string? blobName = DecodeToken(token);
        if (string.IsNullOrWhiteSpace(blobName) || !blobName.StartsWith("conductores/", StringComparison.Ordinal) || blobName.Contains(".."))
        {
            return null;
        }

        if (UsesAzure)
        {
            BlobContainerClient container = new(connectionString, containerName);
            BlobClient blob = container.GetBlobClient(blobName);
            if (!await blob.ExistsAsync()) return null;
            BlobDownloadStreamingResult download = await blob.DownloadStreamingAsync();
            return new StoredDriverFile(download.Content, download.Details.ContentType ?? "application/octet-stream");
        }

        string webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        string absolutePath = Path.GetFullPath(Path.Combine(webRoot, "uploads", blobName.Replace('/', Path.DirectorySeparatorChar)));
        string allowedRoot = Path.GetFullPath(Path.Combine(webRoot, "uploads", "conductores"));
        if (!absolutePath.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase) || !File.Exists(absolutePath)) return null;
        return new StoredDriverFile(File.OpenRead(absolutePath), GetContentType(absolutePath));
    }

    private string BuildDownloadUrl(string blobName)
    {
        string token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(blobName))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        return $"{publicApiBaseUrl.TrimEnd('/')}/api/archivos-conductor/{token}";
    }

    private static string? DecodeToken(string token)
    {
        try
        {
            string value = token.Replace('-', '+').Replace('_', '/');
            value = value.PadRight(value.Length + (4 - value.Length % 4) % 4, '=');
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private static string GetContentType(string path) => Path.GetExtension(path).ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".pdf" => "application/pdf",
        _ => "application/octet-stream"
    };
}

public sealed record StoredDriverFile(Stream Content, string ContentType);
