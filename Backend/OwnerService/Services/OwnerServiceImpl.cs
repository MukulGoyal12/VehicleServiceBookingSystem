using OwnerService.DTO;
using OwnerService.Models;
using OwnerService.Helpers;
using ServiceCenterService.DTO;
using Microsoft.Extensions.Logging;

namespace OwnerService.Services
{
    public class OwnerServiceImpl : IOwnerService
    {
        private readonly OwnerContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OwnerServiceImpl> _logger;

        public OwnerServiceImpl(OwnerContext context, IHttpClientFactory httpClientFactory, ILogger<OwnerServiceImpl> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public IEnumerable<Owner> GetAllOwners()
        {
            try
            {
                return _context.Owners.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllOwners at {time}", DateTime.UtcNow);
                return new List<Owner>();
            }
        }

        public Owner? GetOwnerById(string id)
        {
            try
            {
                return _context.Owners.FirstOrDefault(o => o.OwnerId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOwnerById for Id {id}", id);
                return null;
            }
        }

        public string RegisterOwner(OwnerDTO owner)
        {
            try
            {
                var found = _context.Owners.FirstOrDefault(o => o.Email == owner.Email);
                if (found != null) return "Owner Already Exists";

                owner.Password = BCrypt.Net.BCrypt.HashPassword(owner.Password);

                string IdPrefix = owner.Name.Substring(0, 3);
                string OwnerId = $"{IdPrefix}_00{_context.Owners.Count() + 1}";

                Owner newOwner = new Owner
                {
                    OwnerId = OwnerId,
                    Name = owner.Name,
                    Email = owner.Email,
                    Phone = owner.Phone,
                    Password = owner.Password,
                    ServiceCenterIds = []
                };

                _context.Owners.Add(newOwner);
                _context.SaveChanges();

                _logger.LogInformation("Owner {email} registered successfully at {time}", owner.Email, DateTime.UtcNow);
                return "Owner Register Successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterOwner for Email {email}", owner.Email);
                return "Something went wrong while registering owner";
            }
        }

        public string Login(LoginModel login, out string token)
        {
            token = string.Empty;
            try
            {
                var found = _context.Owners.FirstOrDefault(o => o.Email == login.Email);
                if (found == null) return "Invalid credentials";

                bool isValid = BCrypt.Net.BCrypt.Verify(login.Password, found.Password);
                if (!isValid) return "Invalid credentials";

                token = JWTHelper.GenerateToken(found);
                _logger.LogInformation("Owner {email} logged in successfully at {time}", login.Email, DateTime.UtcNow);
                return "Success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login for Email {email}", login.Email);
                return "Something went wrong while logging in";
            }
        }

        public string UpdateOwner(string id, OwnerDTO updatedOwner)
        {
            try
            {
                var owner = _context.Owners.FirstOrDefault(o => o.OwnerId == id);
                if (owner == null) return "Owner not found";

                if (!string.IsNullOrEmpty(updatedOwner.Name)) owner.Name = updatedOwner.Name;
                if (!string.IsNullOrEmpty(updatedOwner.Email)) owner.Email = updatedOwner.Email;
                if (!string.IsNullOrEmpty(updatedOwner.Phone)) owner.Phone = updatedOwner.Phone;

                if (!string.IsNullOrEmpty(updatedOwner.Password))
                    owner.Password = BCrypt.Net.BCrypt.HashPassword(updatedOwner.Password);

                _context.SaveChanges();

                _logger.LogInformation("Owner {id} updated successfully at {time}", id, DateTime.UtcNow);
                return "Owner updated successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateOwner for Id {id}", id);
                return "Something went wrong while updating owner";
            }
        }

        public async Task<string> DeleteOwner(string id)
        {
            try
            {
                var owner = _context.Owners.FirstOrDefault(o => o.OwnerId == id);
                if (owner == null) return "Owner not found";

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("http://localhost:5268");

                var response = await client.DeleteAsync($"/api/servicecenter/deleteByOwner/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to delete ServiceCenters for Owner {id}", id);
                    return "Failed to delete ServiceCenters";
                }

                _context.Owners.Remove(owner);
                _context.SaveChanges();

                _logger.LogInformation("Owner {id} and linked ServiceCenters deleted successfully at {time}", id, DateTime.UtcNow);
                return "Owner and linked ServiceCenters deleted successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteOwner for Id {id}", id);
                return "Something went wrong while deleting owner";
            }
        }

        public string AddServiceCenter(AddServiceCenterDTO payload)
        {
            try
            {
                var owner = _context.Owners.FirstOrDefault(o => o.OwnerId == payload.OwnerId);
                if (owner == null) return "Owner not found";

                if (!owner.ServiceCenterIds.Contains(payload.ServiceCenterId))
                {
                    owner.ServiceCenterIds.Add(payload.ServiceCenterId);
                    _context.SaveChanges();
                }

                _logger.LogInformation("ServiceCenter {scId} linked to Owner {ownerId}", payload.ServiceCenterId, payload.OwnerId);
                return "ServiceCenter linked to Owner successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddServiceCenter for Owner {ownerId}", payload.OwnerId);
                return "Something went wrong while linking ServiceCenter";
            }
        }

        public string RemoveServiceCenter(AddServiceCenterDTO payload)
        {
            try
            {
                var owner = _context.Owners.FirstOrDefault(o => o.OwnerId == payload.OwnerId);
                if (owner == null) return "Owner not found";

                if (owner.ServiceCenterIds.Contains(payload.ServiceCenterId))
                {
                    owner.ServiceCenterIds.Remove(payload.ServiceCenterId);
                    _context.SaveChanges();
                }

                _logger.LogInformation("ServiceCenter {scId} removed from Owner {ownerId}", payload.ServiceCenterId, payload.OwnerId);
                return "ServiceCenter removed from Owner successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RemoveServiceCenter for Owner {ownerId}", payload.OwnerId);
                return "Something went wrong while removing ServiceCenter";
            }
        }
    }
}
