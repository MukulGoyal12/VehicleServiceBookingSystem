using User_Management.Models;

namespace User_Management.Services
{
    public interface IUserService
    {
        Task<(bool Success, string Message, User? User)> RegisterUserAsync(RegisterDto registerDto);
        Task<(bool Success, string Message, User? User)> LoginUserAsync(LoginDto loginDto);
        Task<User?> GetUserByIdAsync(int id);
        Task<(bool Success, string Message, User? User)> UpdateUserAsync(int id, UpdateUserDto updatedUser);
        Task<(bool Success, string Message)> DeleteUserAsync(int id);
        string GenerateTokenForUser(User user);
        Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    }
}