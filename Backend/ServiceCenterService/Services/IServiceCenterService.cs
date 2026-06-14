using ServiceCenterService.DTO;
using ServiceCenterService.Models;
using ServiceCenterService.Helpers;

namespace ServiceCenterService.Services
{
    public interface IServiceCenterService
    {
        Task<ApiResponse<List<ServiceCenter>>> GetByOwnerIdAsync(string ownerId);
        Task<ApiResponse<ServiceCenter>> GetByIdForOwnerAsync(string id, string ownerId);
        Task<ApiResponse<ServiceCenter>> CreateAsync(ServiceCenterDTO dto, string ownerId);
        Task<ApiResponse<bool>> UpdateForOwnerAsync(string id, ServiceCenterDTO dto, string ownerId);
        Task<ApiResponse<bool>> DeleteAsync(string id);
        Task<ApiResponse<bool>> DeleteForOwnerAsync(string id, string ownerId);
        Task<ApiResponse<bool>> DeleteByOwnerAsync(string ownerId);
    }
}
