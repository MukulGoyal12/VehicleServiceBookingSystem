using System.Security.Cryptography;
using System.Text;
using User_Management.Models;
using User_Management.Repository;

namespace User_Management.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IJwtTokenService jwtTokenService, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, User? User)> RegisterUserAsync(RegisterDto registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Email) ||
                string.IsNullOrWhiteSpace(registerDto.Password) ||
                string.IsNullOrWhiteSpace(registerDto.Name))
            {
                return (false, "Email, password, and name are required.", null);
            }

            var existingUser = await _userRepository.GetUserByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return (false, "Email is already registered.", null);
            }

            var passwordHash = HashPassword(registerDto.Password);

            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Phone = registerDto.Phone ?? string.Empty,
                Address = registerDto.Address ?? string.Empty,
                PasswordHash = passwordHash
            };

            var createdUser = await _userRepository.CreateUserAsync(user);
            _logger.LogInformation($"User registered successfully: {createdUser.Email}");

            return (true, "User registered successfully.", createdUser);
        }

        public async Task<(bool Success, string Message, User? User)> LoginUserAsync(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return (false, "Email and password are required.", null);
            }

            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return (false, "Invalid email or password.", null);
            }

            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return (false, "Invalid email or password.", null);
            }

            _logger.LogInformation($"User logged in successfully: {user.Email}");
            return (true, "Login successful.", user);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<(bool Success, string Message, User? User)> UpdateUserAsync(int id, UpdateUserDto updatedUser)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return (false, "User not found.", null);
            }

            user.Name = updatedUser.Name ?? user.Name;
            user.Phone = updatedUser.Phone ?? user.Phone;
            user.Address = updatedUser.Address ?? user.Address;

            var result = await _userRepository.UpdateUserAsync(user);
            _logger.LogInformation($"User updated successfully: {user.Email}");

            return (true, "User updated successfully.", result);
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
        {
            var success = await _userRepository.DeleteUserAsync(id);
            if (!success)
            {
                return (false, "User not found.");
            }

            _logger.LogInformation($"User deleted successfully: ID {id}");
            return (true, "User deleted successfully.");
        }

     
        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            var currentHash = HashPassword(dto.CurrentPassword);
            if (user.PasswordHash != currentHash)
                return (false, "Current password is incorrect.");

            user.PasswordHash = HashPassword(dto.NewPassword);
            await _userRepository.UpdateUserAsync(user);

            _logger.LogInformation($"Password changed for user: ID {userId}");
            return (true, "Password changed successfully.");
        }
        public string GenerateTokenForUser(User user)
        {
            return _jwtTokenService.GenerateToken(user);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password).Equals(hash);
        }
    }
}
