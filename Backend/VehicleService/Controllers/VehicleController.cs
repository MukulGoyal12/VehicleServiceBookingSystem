using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleService.DTO;
using VehicleService.Services;

namespace VehicleService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;
            if (int.TryParse(userIdClaim, out var userId))
                return userId;
            return null;
        }

        // GET api/Vehicle/user-vehicles
        [HttpGet("user-vehicles")]
        [Authorize]
        public async Task<IActionResult> GetMyVehicles()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("UserId claim missing or invalid.");

            var vehicles = await _vehicleService.GetMyVehicles(userId.Value);
            if (!vehicles.Any()) return NotFound("No vehicles found for this user.");

            return Ok(vehicles);
        }

        // GET api/Vehicle/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var vehicle = await _vehicleService.GetVehicleById(id, userId.Value);
            if (vehicle == null) return NotFound($"Vehicle with ID {id} not found.");

            return Ok(vehicle);
        }

        // POST api/Vehicle
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddVehicle([FromBody] VehicleDTO vehicleDto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("UserId claim missing or invalid.");

            var vehicle = await _vehicleService.AddVehicle(vehicleDto, userId.Value);
            return CreatedAtAction(nameof(Get), new { id = vehicle.VehicleId }, vehicle);
        }

        // PATCH api/Vehicle/5
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Patch(int id, [FromBody] VehicleDTO vehicleDto)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return Unauthorized("UserId claim missing or invalid.");

                var vehicle = await _vehicleService.PatchVehicle(id, vehicleDto, userId.Value);
                if (vehicle == null) return NotFound($"Vehicle with ID {id} not found or not yours.");

                return Ok(vehicle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE api/Vehicle/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("UserId claim missing or invalid.");

            var result = await _vehicleService.DeleteVehicle(id, userId.Value);
            if (!result) return NotFound($"Vehicle with ID {id} not found.");

            return NoContent();
        }
    }
}