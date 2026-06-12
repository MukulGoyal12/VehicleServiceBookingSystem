using Microsoft.AspNetCore.Mvc;
using ServiceCenterService.DTO;
using ServiceCenterService.Models;

namespace ServiceCenterService.Services
{
    public interface IServiceCenterService
    {
        Task<List<ServiceCenter>> GetByOwnerIdAsync(string ownerId);
        Task<ServiceCenter?> GetByIdForOwnerAsync(string id, string ownerId);
        Task<ServiceCenter?> CreateAsync(ServiceCenterDTO dto, string ownerId);
        Task<bool> UpdateForOwnerAsync(string id, ServiceCenterDTO dto, string ownerId);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteForOwnerAsync(string id, string ownerId);
        Task<bool> DeleteByOwnerAsync(string ownerId);
    }
}
