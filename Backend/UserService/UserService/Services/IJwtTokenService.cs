using User_Management.Models;

namespace User_Management.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
