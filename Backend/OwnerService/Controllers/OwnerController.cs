using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OwnerService.DTO;
using OwnerService.Helpers;
using OwnerService.Models;
using OwnerService.Services;
using ServiceCenterService.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OwnerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _ownerService;

        public OwnerController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_ownerService.GetAllOwners());

        [HttpPost("register")]
        public IActionResult Register([FromBody] OwnerDTO owner)
        {
            var result = _ownerService.RegisterOwner(owner);
            return Ok(new { Status = "Success", Message = result });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var result = _ownerService.Login(login, out string token);
            if (result != "Success") return Unauthorized(new { Status = "Failed", Message = result });
            return Ok(new { Status = "Success", Token = token });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var owner = _ownerService.GetOwnerById(id);
            if (owner == null) return NotFound("Owner not found");
            return Ok(owner);
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(string id, [FromBody] OwnerDTO updatedOwner)
        {
            var result = _ownerService.UpdateOwner(id, updatedOwner);
            if (result == "Owner not found") return NotFound(result);
            if (result.StartsWith("Something went wrong")) return StatusCode(500, result);

            return Ok(new { Status = "Success", Message = result });
        } 

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _ownerService.DeleteOwner(id);
            if (result == "Owner not found") return NotFound(result);
            return Ok(new { Status = "Success", Message = result });
        }

        [HttpPost("addServiceCenter")]
        public IActionResult AddServiceCenter([FromBody] AddServiceCenterDTO payload)
        {
            var result = _ownerService.AddServiceCenter(payload);
            return Ok(new { Status = "Success", Message = result });
        }

        [HttpPost("removeServiceCenter")]
        public IActionResult RemoveServiceCenter([FromBody] AddServiceCenterDTO payload)
        {
            var result = _ownerService.RemoveServiceCenter(payload);
            return Ok(new { Status = "Success", Message = result });
        }
    }
}
