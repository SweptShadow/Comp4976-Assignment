using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObituaryApp.Services;

namespace ObituaryApp.Controllers
{
    [Route("api/Upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IBlobService _blobService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IBlobService blobService, IWebHostEnvironment env, ILogger<UploadController> logger)
        {
            _blobService = blobService;
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Upload a photo file and return a URL path.
        /// Tries Azure Blob first if configured; falls back to local wwwroot/uploads.
        /// </summary>
        [HttpPost("photo")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [RequestSizeLimit(20_000_000)] // 20 MB
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new { message = "No file provided" });

            string? url = null;
            try
            {
                url = await _blobService.UploadFileAsync(file);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Blob upload failed. Falling back to local storage.");
            }

            if (string.IsNullOrEmpty(url))
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
                var fullPath = Path.Combine(uploadsFolder, uniqueFileName);
                await using (var stream = System.IO.File.Create(fullPath))
                {
                    await file.CopyToAsync(stream);
                }

                url = Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
                // For local, return absolute path so the WASM can display it
                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                url = url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? url : $"{baseUrl}/{url}";
            }

            return Ok(new { url });
        }

        // Simple health check to verify routing under different hosts
        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping() => Ok(new { status = "ok" });
    }
}
