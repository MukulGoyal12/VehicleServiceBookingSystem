using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceCenterService.DTO;
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

        public async Task<List<ServiceCenter>> GetByOwnerIdAsync(string ownerId)
        {
            try
            {
                return await _context.ServiceCenters
                                     .Where(sc => sc.OwnerId == ownerId)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching service centers for OwnerId {ownerId}", ownerId);
                return new List<ServiceCenter>();
            }
        }

        public async Task<ServiceCenter?> GetByIdForOwnerAsync(string id, string ownerId)
        {
            try
            {
                return await _context.ServiceCenters
                                     .FirstOrDefaultAsync(sc => sc.ServiceCenterID == id && sc.OwnerId == ownerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching service center {id} for OwnerId {ownerId}", id, ownerId);
                return null;
            }
        }

        public async Task<ServiceCenter?> CreateAsync(ServiceCenterDTO dto, string ownerId)
        {
            try
            {
                string IdPrefix = dto.Location.Substring(0, 3);
                string ServiceCenterId = $"{IdPrefix}_00{_context.ServiceCenters.Count() + 1}";
                var newServiceCenter = new ServiceCenter
                {
                    ServiceCenterID = ServiceCenterId,
                    Name = dto.Name,
                    Location = dto.Location,
                    Contact = dto.Contact,
                    OwnerId = ownerId,
                    ServiceDescription = dto.ServiceDescription
                };

                _context.ServiceCenters.Add(newServiceCenter);
                await _context.SaveChangesAsync();

                _logger.LogInformation("ServiceCenter {name} created for OwnerId {ownerId}", dto.Name, ownerId);
                return newServiceCenter;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service center for OwnerId {ownerId}", ownerId);
                return null;
            }
        }

        public async Task<bool> UpdateForOwnerAsync(string id, ServiceCenterDTO dto, string ownerId)
        {
            try
            {
                var serviceCenter = await _context.ServiceCenters
                                                  .FirstOrDefaultAsync(sc => sc.ServiceCenterID == id && sc.OwnerId == ownerId);
                if (serviceCenter == null) return false;

                if (!string.IsNullOrEmpty(dto.Name)) serviceCenter.Name = dto.Name;
                if (!string.IsNullOrEmpty(dto.Location)) serviceCenter.Location = dto.Location;
                if (!string.IsNullOrEmpty(dto.Contact)) serviceCenter.Contact = dto.Contact;
                if (!string.IsNullOrEmpty(dto.ServiceDescription)) serviceCenter.ServiceDescription = dto.ServiceDescription;

                await _context.SaveChangesAsync();

                _logger.LogInformation("ServiceCenter {id} updated for OwnerId {ownerId}", id, ownerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service center {id} for OwnerId {ownerId}", id, ownerId);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var serviceCenter = await _context.ServiceCenters.FindAsync(id);
                if (serviceCenter == null) return false;

                _context.ServiceCenters.Remove(serviceCenter);
                await _context.SaveChangesAsync();

                _logger.LogInformation("ServiceCenter {id} deleted", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service center {id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteForOwnerAsync(string id, string ownerId)
        {
            try
            {
                var serviceCenter = await _context.ServiceCenters
                                                  .FirstOrDefaultAsync(sc => sc.ServiceCenterID == id && sc.OwnerId == ownerId);
                if (serviceCenter == null) return false;

                _context.ServiceCenters.Remove(serviceCenter);
                await _context.SaveChangesAsync();

                _logger.LogInformation("ServiceCenter {id} deleted for OwnerId {ownerId}", id, ownerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service center {id} for OwnerId {ownerId}", id, ownerId);
                return false;
            }
        }

        public async Task<bool> DeleteByOwnerAsync(string ownerId)
        {
            try
            {
                var serviceCenters = _context.ServiceCenters
                    .Where(sc => sc.OwnerId == ownerId)
                    .ToList();

                if (!serviceCenters.Any()) return false;

                _context.ServiceCenters.RemoveRange(serviceCenters);
                await _context.SaveChangesAsync();

                _logger.LogInformation("All service centers deleted for OwnerId {ownerId}", ownerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service centers for OwnerId {ownerId}", ownerId);
                return false;
            }
        }
    }
}
