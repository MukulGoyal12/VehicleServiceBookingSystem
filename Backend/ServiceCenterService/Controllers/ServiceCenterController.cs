using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceCenterService.DTO;
using ServiceCenterService.Helpers;
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

        private IActionResult StatusCodeFromResponse<T>(ApiResponse<T> response)
        {
            return response.Status switch
            {
                "Success" => Ok(response),
                "Failed" => BadRequest(response),
                "Error" => StatusCode(500, response),
                _ => StatusCode(500, response)
            };
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var ownerId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized(ResponseFactory.Failed<string>("OwnerId not found in token."));

            var response = await _service.GetByOwnerIdAsync(ownerId);
            return StatusCodeFromResponse(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            var ownerId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized(ResponseFactory.Failed<string>("OwnerId not found in token."));

            var response = await _service.GetByIdForOwnerAsync(id, ownerId);
            return StatusCodeFromResponse(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] ServiceCenterDTO dto)
        {
            var ownerId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized(ResponseFactory.Failed<string>("OwnerId not found in token."));

            var response = await _service.CreateAsync(dto, ownerId);
            if (response.Status != "Success")
                return StatusCodeFromResponse(response);

            // HttpClient call to OwnerService
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5242");

            var payload = new { OwnerId = ownerId, ServiceCenterId = response.Data!.ServiceCenterID };
            var ownerResponse = await client.PostAsJsonAsync("/api/owner/addServiceCenter", payload);

            if (!ownerResponse.IsSuccessStatusCode)
                return StatusCode((int)ownerResponse.StatusCode, ResponseFactory.Failed<string>("Failed to update OwnerService"));

            return StatusCodeFromResponse(response);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Patch(string id, [FromBody] ServiceCenterDTO dto)
        {
            var ownerId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized(ResponseFactory.Failed<string>("OwnerId not found in token."));

            var response = await _service.UpdateForOwnerAsync(id, dto, ownerId);
            return StatusCodeFromResponse(response);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var ownerId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized(ResponseFactory.Failed<string>("OwnerId not found in token."));

            var response = await _service.DeleteForOwnerAsync(id, ownerId);
            if (response.Status != "Success")
                return StatusCodeFromResponse(response);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5242");

            var payload = new { OwnerId = ownerId, ServiceCenterId = id };
            var ownerResponse = await client.PostAsJsonAsync("/api/owner/removeServiceCenter", payload);

            if (!ownerResponse.IsSuccessStatusCode)
                return StatusCode((int)ownerResponse.StatusCode, ResponseFactory.Failed<string>("Failed to update OwnerService"));

            return StatusCodeFromResponse(ResponseFactory.Success("ServiceCenter deleted and Owner updated successfully", id));
        }

        [HttpDelete("deleteByOwner/{ownerId}")]
        public async Task<IActionResult> DeleteByOwner(string ownerId)
        {
            var response = await _service.DeleteByOwnerAsync(ownerId);
            return StatusCodeFromResponse(response);
        }
    }
}
