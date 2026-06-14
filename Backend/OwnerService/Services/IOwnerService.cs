using OwnerService.DTO;
using OwnerService.Models;
using ServiceCenterService.DTO;

namespace OwnerService.Services
{
    public interface IOwnerService
    {
        ApiResponse<IEnumerable<Owner>> GetAllOwners();
        ApiResponse<IEnumerable<Owner>> GetOwnersHistory(); 
        ApiResponse<Owner?> GetOwnerById(string id);
        ApiResponse<Owner> RegisterOwner(OwnerDTO owner);
        ApiResponse<string> Login(LoginModel login);
        ApiResponse<string> UpdateOwner(string id, OwnerDTO updatedOwner);
        Task<ApiResponse<string>> DeleteOwner(string id);
        ApiResponse<string> AddServiceCenter(AddServiceCenterDTO payload);
        ApiResponse<string> RemoveServiceCenter(AddServiceCenterDTO payload);
    }
}
