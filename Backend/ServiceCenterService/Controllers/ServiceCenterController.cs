using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceCenterService.DTO;
using ServiceCenterService.Services;

namespace ServiceCenterService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCenterController : ControllerBase
    {
        private readonly IServiceCenterService _service;
        private readonly IHttpClientFactory _httpClientFactory;

        public ServiceCenterController(IServiceCenterService service, IHttpClientFactory httpClientFactory)
        {
            _service = service;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var ownerId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized("OwnerId not found in token.");

            var centers = await _service.GetByOwnerIdAsync(ownerId);
            if (centers == null || centers.Count == 0)
                return NotFound("No service centers found for this user.");

            return Ok(centers);
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            var ownerId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized("OwnerId not found in token.");

            var serviceCenter = await _service.GetByIdForOwnerAsync(id, ownerId);
            if (serviceCenter == null)
                return NotFound("Service center not found for this user.");

            return Ok(serviceCenter);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] ServiceCenterDTO dto)
        {
            var ownerId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized("OwnerId not found in token.");

            var newServiceCenter = await _service.CreateAsync(dto, ownerId);

            // HttpClient call to OwnerService
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5242");

            var payload = new { OwnerId = ownerId, ServiceCenterId = newServiceCenter.ServiceCenterID };
            var response = await client.PostAsJsonAsync("/api/owner/addServiceCenter", payload);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to update OwnerService");

            return Ok(newServiceCenter);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Patch(string id, [FromBody] ServiceCenterDTO dto)
        {
            var ownerId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized("OwnerId not found in token.");

            var updated = await _service.UpdateForOwnerAsync(id, dto, ownerId);
            if (!updated) return NotFound("ServiceCenter not found for this user.");

            return Ok(new { Status = "Success", Message = "ServiceCenter updated successfully" });
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var ownerId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized("OwnerId not found in token.");

            var deleted = await _service.DeleteForOwnerAsync(id, ownerId);
            if (!deleted) return NotFound("ServiceCenter not found for this user.");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5242");

            var payload = new { OwnerId = ownerId, ServiceCenterId = id };
            var response = await client.PostAsJsonAsync("/api/owner/removeServiceCenter", payload);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to update OwnerService");

            return Ok(new { Status = "Success", Message = "ServiceCenter deleted and Owner updated successfully" });
        }


        [HttpDelete("deleteByOwner/{ownerId}")]
        public async Task<IActionResult> DeleteByOwner(string ownerId)
        {
            var deleted = await _service.DeleteByOwnerAsync(ownerId.ToString());
            if (!deleted) return NotFound("No ServiceCenters found for this Owner");
            return Ok(new { Status = "Success", Message = "All ServiceCenters deleted for Owner" });
        }
    }
}