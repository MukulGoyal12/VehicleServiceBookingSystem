using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceCenterService.DTO;
using ServiceCenterService.Helpers;
using ServiceCenterService.Models;

namespace ServiceCenterService.Services
{
    public class ServiceCenterServiceImpl : IServiceCenterService
    {
        private readonly ServiceCenterContext _context;
        private readonly ILogger<ServiceCenterServiceImpl> _logger;

        public ServiceCenterServiceImpl(ServiceCenterContext context, ILogger<ServiceCenterServiceImpl> logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task<ApiResponse<T>> ExecuteAsync<T>(Func<Task<ApiResponse<T>>> func, string errorMessage)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                return ResponseFactory.Error<T>($"{errorMessage}: {ex.Message}");
            }
        }

        public Task<ApiResponse<List<ServiceCenter>>> GetByOwnerIdAsync(string ownerId) =>
            ExecuteAsync(async () =>
            {
                var centers = await _context.ServiceCenters
                    .Where(sc => sc.OwnerId == ownerId && sc.Status == ServiceCenter.CenterStatus.Active)
                    .ToListAsync();

                if (!centers.Any())
                    return ResponseFactory.Failed<List<ServiceCenter>>("No active service centers found.");

                return ResponseFactory.Success("Active service centers fetched successfully.", centers);
            }, "Error fetching service centers");

        public Task<ApiResponse<ServiceCenter>> GetByIdForOwnerAsync(string id, string ownerId) =>
            ExecuteAsync(async () =>
            {
                var center = await _context.ServiceCenters
                    .FirstOrDefaultAsync(sc => sc.ServiceCenterID == id
                                            && sc.OwnerId == ownerId
                                            && sc.Status == ServiceCenter.CenterStatus.Active);

                if (center == null)
                    return ResponseFactory.Failed<ServiceCenter>("Service center not found or inactive.");

                return ResponseFactory.Success("Service center fetched successfully.", center);
            }, "Error fetching service center");

        public Task<ApiResponse<ServiceCenter>> CreateAsync(ServiceCenterDTO dto, string ownerId) =>
            ExecuteAsync(async () =>
            {
                var existingCenter = await _context.ServiceCenters.FirstOrDefaultAsync(sc => sc.OwnerId == ownerId
                    && sc.CenterName == dto.CenterName
                    && sc.Street == dto.Street
                    && sc.Pincode == dto.Pincode
                    && sc.Status == ServiceCenter.CenterStatus.Active);

                if (existingCenter != null)
                    return ResponseFactory.Failed<ServiceCenter>("Service center already exists at this address.");

                string IdPrefix = dto.City.Length >= 3 ? dto.City.Substring(0, 3).ToUpper() : dto.City.ToUpper();
                string ServiceCenterId = $"{IdPrefix}_{(_context.ServiceCenters.Count() + 1).ToString("D3")}";

                var newServiceCenter = new ServiceCenter
                {
                    ServiceCenterID = ServiceCenterId,
                    CenterName = dto.CenterName,
                    FlatNumber = dto.FlatNumber,
                    Street = dto.Street,
                    NearestLandmark = dto.NearestLandmark,
                    City = dto.City,
                    State = dto.State,
                    Pincode = dto.Pincode,
                    Contact = dto.Contact,
                    OwnerId = ownerId,
                    ServiceDescription = dto.ServiceDescription,
                    Capacity = dto.Capacity,
                    CreatedAt = DateTime.UtcNow,
                    Status = ServiceCenter.CenterStatus.Active
                };

                _context.ServiceCenters.Add(newServiceCenter);
                await _context.SaveChangesAsync();

                return ResponseFactory.Success("Service center created successfully.", newServiceCenter);
            }, "Error creating service center");

        public Task<ApiResponse<bool>> UpdateForOwnerAsync(string id, ServiceCenterDTO dto, string ownerId) =>
            ExecuteAsync(async () =>
            {
                var serviceCenter = await _context.ServiceCenters
                    .FirstOrDefaultAsync(sc => sc.ServiceCenterID == id
                                            && sc.OwnerId == ownerId
                                            && sc.Status == ServiceCenter.CenterStatus.Active);

                if (serviceCenter == null)
                    return ResponseFactory.Failed<bool>("Service center not found or inactive.");

                if (!string.IsNullOrEmpty(dto.CenterName)) serviceCenter.CenterName = dto.CenterName;
                if (!string.IsNullOrEmpty(dto.Street)) serviceCenter.Street = dto.Street;
                if (!string.IsNullOrEmpty(dto.Contact)) serviceCenter.Contact = dto.Contact;
                if (!string.IsNullOrEmpty(dto.ServiceDescription)) serviceCenter.ServiceDescription = dto.ServiceDescription;

                await _context.SaveChangesAsync();

                return ResponseFactory.Success("Service center updated successfully.", true);
            }, "Error updating service center");

        public Task<ApiResponse<bool>> DeleteAsync(string id) =>
            ExecuteAsync(async () =>
            {
                var serviceCenter = await _context.ServiceCenters
                    .FirstOrDefaultAsync(sc => sc.ServiceCenterID == id && sc.Status == ServiceCenter.CenterStatus.Active);

                if (serviceCenter == null)
                    return ResponseFactory.Failed<bool>("Service center not found or already inactive.");

                serviceCenter.Status = ServiceCenter.CenterStatus.Inactive;
                await _context.SaveChangesAsync();

                return ResponseFactory.Success("Service center marked as inactive.", true);
            }, "Error marking service center inactive");

        public Task<ApiResponse<bool>> DeleteForOwnerAsync(string id, string ownerId) =>
            ExecuteAsync(async () =>
            {
                var serviceCenter = await _context.ServiceCenters
                    .FirstOrDefaultAsync(sc => sc.ServiceCenterID == id
                                            && sc.OwnerId == ownerId
                                            && sc.Status == ServiceCenter.CenterStatus.Active);

                if (serviceCenter == null)
                    return ResponseFactory.Failed<bool>("Service center not found or already inactive.");

                serviceCenter.Status = ServiceCenter.CenterStatus.Inactive;
                await _context.SaveChangesAsync();

                return ResponseFactory.Success("Service center marked inactive.", true);
            }, "Error marking service center inactive");

        public Task<ApiResponse<bool>> DeleteByOwnerAsync(string ownerId) =>
            ExecuteAsync(async () =>
            {
                var serviceCenters = _context.ServiceCenters
                    .Where(sc => sc.OwnerId == ownerId && sc.Status == ServiceCenter.CenterStatus.Active)
                    .ToList();

                if (!serviceCenters.Any())
                    return ResponseFactory.Failed<bool>("No active service centers found for owner.");

                foreach (var sc in serviceCenters)
                {
                    sc.Status = ServiceCenter.CenterStatus.Inactive;
                }

                await _context.SaveChangesAsync();

                return ResponseFactory.Success("All service centers marked inactive for owner.", true);
            }, "Error marking service centers inactive");
    }
}
