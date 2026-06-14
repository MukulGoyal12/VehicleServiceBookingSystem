using Microsoft.AspNetCore.Mvc;
using OwnerService.DTO;
using OwnerService.Models;
using OwnerService.Services;
using ServiceCenterService.DTO;

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
        public IActionResult Get()
        {
            var response = _ownerService.GetAllOwners();
            return StatusCodeFromResponse(response);
        }

        [HttpGet("History")]
        public IActionResult GetOwnerHistory()
        {
            var response = _ownerService.GetOwnersHistory();
            return StatusCodeFromResponse(response);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] OwnerDTO owner)
        {
            var response = _ownerService.RegisterOwner(owner);
            return StatusCodeFromResponse(response);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var response = _ownerService.Login(login);
            return StatusCodeFromResponse(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var response = _ownerService.GetOwnerById(id);
            return StatusCodeFromResponse(response);
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(string id, [FromBody] OwnerDTO updatedOwner)
        {
            var response = _ownerService.UpdateOwner(id, updatedOwner);
            return StatusCodeFromResponse(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _ownerService.DeleteOwner(id);
            return StatusCodeFromResponse(response);
        }

        [HttpPost("addServiceCenter")]
        public IActionResult AddServiceCenter([FromBody] AddServiceCenterDTO payload)
        {
            var response = _ownerService.AddServiceCenter(payload);
            return StatusCodeFromResponse(response);
        }

        [HttpPost("removeServiceCenter")]
        public IActionResult RemoveServiceCenter([FromBody] AddServiceCenterDTO payload)
        {
            var response = _ownerService.RemoveServiceCenter(payload);
            return StatusCodeFromResponse(response);
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
    }
}
