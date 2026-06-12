using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User_Management.Models;
using User_Management.Services;

namespace User_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse(false, "Invalid input data."));
            }

            var (success, message, user) = await _userService.RegisterUserAsync(registerDto);

            if (!success)
            {
                return BadRequest(new ApiResponse<LoginResponse>(false, message));
            }

            var token = _userService.GenerateTokenForUser(user!);
            var loginResponse = new LoginResponse
            {
                UserID = user!.UserID,
                Name = user.Name,
                Email = user.Email
                
            };

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserID }, 
                new ApiResponse<LoginResponse>(true, message, loginResponse));
        }

        /// <summary>
        /// Login user with email and password
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse(false, "Invalid input data."));
            }

            var (success, message, user) = await _userService.LoginUserAsync(loginDto);

            if (!success)
            {
                return Unauthorized(new ApiResponse<LoginResponse>(false, message));
            }

            var token = _userService.GenerateTokenForUser(user!);
            var loginResponse = new LoginResponse
            {
                UserID = user!.UserID,
                Name = user.Name,
                Email = user.Email,
                Token = token
            };

            return Ok(new ApiResponse<LoginResponse>(true, message, loginResponse));
        }


        /// <summary>
        /// Get user by ID (requires authentication)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<User>>> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<User>(false, "User not found."));
                }

                return Ok(new ApiResponse<User>(true, "User retrieved successfully.", user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ApiResponse(false, "An error occurred while retrieving the user."));
            }
        }

        /// <summary>
        /// Update user profile (partial update, requires authentication)
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<User>>> UpdateUser(int id, [FromBody] UpdateUserDto updatedUser)
        {
            if (updatedUser == null)
            {
                return BadRequest(new ApiResponse<User>(false, "Invalid request data."));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<User>(false, "Invalid input data."));
            }

            var (success, message, user) = await _userService.UpdateUserAsync(id, updatedUser);

            if (!success)
            {
                return NotFound(new ApiResponse<User>(false, message));
            }

            return Ok(new ApiResponse<User>(true, message, user));
        }

        /// <summary>
        /// Delete user (requires authentication)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> DeleteUser(int id)
        {
            try
            {
                var (success, message) = await _userService.DeleteUserAsync(id);

                if (!success)
                {
                    return NotFound(new ApiResponse(false, message));
                }

                return Ok(new ApiResponse(true, message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ApiResponse(false, "An error occurred while deleting the user."));
            }
        }
        [HttpPatch("{id}/change-password")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<User>>> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<User>(false, "Invalid input data."));

            var (success, message) = await _userService.ChangePasswordAsync(id, changePasswordDto);

            if (!success)
                return BadRequest(new ApiResponse<User>(false, message));

            return Ok(new ApiResponse<User>(true, message));
        }


    }
}
