using Microsoft.AspNetCore.Mvc;
using Paraba.API.Services;

namespace Paraba.API.Controllers;

[ApiController]
[Route("api/archivos-conductor")]
public sealed class DriverFilesController : ControllerBase
{
    private readonly DriverFileStorage storage;

    public DriverFilesController(DriverFileStorage storage)
    {
        this.storage = storage;
    }

    [HttpGet("{token}")]
    public async Task<IActionResult> Get(string token)
    {
        StoredDriverFile? file = await storage.OpenAsync(token);
        return file == null ? NotFound() : File(file.Content, file.ContentType);
    }
}
