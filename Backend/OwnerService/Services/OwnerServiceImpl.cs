using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OwnerService.DTO;
using OwnerService.Helpers;
using OwnerService.Models;
using ServiceCenterService.DTO;

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

        private Owner? GetActiveOwner(string id)
        {
            return _context.Owners.FirstOrDefault(o => o.OwnerId == id && o.Status == Owner.OwnerStatus.Active);
        }

        private ApiResponse<T> Execute<T>(Func<ApiResponse<T>> func, string errorMessage)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                return ResponseFactory.Error<T>($"{errorMessage}: {ex.Message}");
            }
        }

        public ApiResponse<IEnumerable<Owner>> GetOwnersHistory()
        {
            return Execute(() =>
            {
                IEnumerable<Owner> owners = _context.Owners.ToList();
                return ResponseFactory.Success("Owners fetched successfully", owners);
            }, "Error in GetAllOwners");
        }

        public ApiResponse<IEnumerable<Owner>> GetAllOwners()
        {
            return Execute(() =>
            {
                IEnumerable<Owner> owners = _context.Owners.Where(o => o.Status == Owner.OwnerStatus.Active).ToList();
                return ResponseFactory.Success("Owners fetched successfully", owners);
            }, "Error in GetAllOwners");
        }


        public ApiResponse<Owner> GetOwnerById(string id)
        {
            return Execute(() =>
            {
                var owner = GetActiveOwner(id);
                if (owner == null)
                    return ResponseFactory.Failed<Owner>("Owner not found or inactive");

                return ResponseFactory.Success("Owner fetched successfully", owner);
            }, "Error in GetOwnerById");
        }


        public ApiResponse<Owner> RegisterOwner(OwnerDTO owner)
        {
            return Execute(() =>
            {
                if (string.IsNullOrEmpty(owner.Name) || string.IsNullOrEmpty(owner.Password))
                    return ResponseFactory.Failed<Owner>("Name and Password are required");

                var found = _context.Owners.FirstOrDefault(o =>o.Email == owner.Email && o.Status == Owner.OwnerStatus.Active); 
                if (found != null)
                    return ResponseFactory.Failed<Owner>("Owner Already Exists");

                owner.Password = BCrypt.Net.BCrypt.HashPassword(owner.Password);

                string IdPrefix = owner.Name.Length >= 3 ? owner.Name.Substring(0, 3) : owner.Name;
                int nextNumber = _context.Owners.Count() + 1;
                string OwnerId = $"{IdPrefix}_{nextNumber.ToString("D3")}";

                Owner newOwner = new Owner
                {
                    OwnerId = OwnerId,
                    Name = owner.Name,
                    Email = owner.Email,
                    Phone = owner.Phone,
                    Password = owner.Password,
                    ServiceCenterIds = new List<string>(),
                    Status = Owner.OwnerStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Owners.Add(newOwner);
                _context.SaveChanges();

                _logger.LogInformation("Owner {email} registered successfully at {time}", owner.Email, DateTime.UtcNow);
                return ResponseFactory.Success("Owner Register Successfully", newOwner);
            }, "Error in RegisterOwner");
        }


        public ApiResponse<string> Login(LoginModel login)
        {
            return Execute(() =>
            {
                var found = _context.Owners.FirstOrDefault(o => o.Email == login.Email && o.Status == Owner.OwnerStatus.Active);
                if (found == null)
                    return ResponseFactory.Failed<string>("Invalid credentials");

                bool isValid = BCrypt.Net.BCrypt.Verify(login.Password, found.Password);
                if (!isValid)
                    return ResponseFactory.Failed<string>("Invalid credentials");

                string token = JWTHelper.GenerateToken(found);
                _logger.LogInformation("Owner {email} logged in successfully at {time}", login.Email, DateTime.UtcNow);
                return ResponseFactory.Success("Login successful", token);
            }, "Error in Login");
        }


        public ApiResponse<string> UpdateOwner(string id, OwnerDTO updatedOwner)
        {
            return Execute(() =>
            {
                var owner = GetActiveOwner(id);
                if (owner == null)
                    return ResponseFactory.Failed<string>("Owner not found");

                if (!string.IsNullOrEmpty(updatedOwner.Name)) owner.Name = updatedOwner.Name;
                if (!string.IsNullOrEmpty(updatedOwner.Email)) owner.Email = updatedOwner.Email;
                if (!string.IsNullOrEmpty(updatedOwner.Phone)) owner.Phone = updatedOwner.Phone;
                if (!string.IsNullOrEmpty(updatedOwner.Password))
                    owner.Password = BCrypt.Net.BCrypt.HashPassword(updatedOwner.Password);

                _context.SaveChanges();
                _logger.LogInformation("Owner {id} updated successfully at {time}", id, DateTime.UtcNow);
                return ResponseFactory.Success("Owner updated successfully",id);
            }, "Error in UpdateOwner");
        }

        public async Task<ApiResponse<string>> DeleteOwner(string id)
        {
            try
            {
                var owner = GetActiveOwner(id);
                if (owner == null)
                    return ResponseFactory.Failed<string>("Owner not found");

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("http://localhost:5268");

                var response = await client.DeleteAsync($"/api/servicecenter/deleteByOwner/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to delete ServiceCenters for Owner {id}", id);
                    return ResponseFactory.Failed<string>("Failed to delete ServiceCenters");
                }

                owner.Status = Owner.OwnerStatus.Inactive;
                _context.SaveChanges();

                _logger.LogInformation("Owner {id} and linked ServiceCenters deleted successfully at {time}", id, DateTime.UtcNow);
                return ResponseFactory.Success("Owner and linked ServiceCenters deleted successfully", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteOwner for Id {id}", id);
                return ResponseFactory.Error<string>("Something went wrong while deleting owner");
            }
        }

        public ApiResponse<string> AddServiceCenter(AddServiceCenterDTO payload)
        {
            return Execute(() =>
            {
                var owner = GetActiveOwner(payload.OwnerId);
                if (owner == null)
                    return ResponseFactory.Failed<string>("Owner not found");

                if (!owner.ServiceCenterIds.Contains(payload.ServiceCenterId))
                {
                    owner.ServiceCenterIds.Add(payload.ServiceCenterId);
                    _context.SaveChanges();
                }

                _logger.LogInformation("ServiceCenter {scId} linked to Owner {ownerId}", payload.ServiceCenterId, payload.OwnerId);
                return ResponseFactory.Success("ServiceCenter linked to Owner successfully", payload.ServiceCenterId);
            }, "Error in AddServiceCenter");
        }

        public ApiResponse<string> RemoveServiceCenter(AddServiceCenterDTO payload)
        {
            return Execute(() =>
            {
                var owner = GetActiveOwner(payload.OwnerId);
                if (owner == null)
                    return ResponseFactory.Failed<string>("Owner not found");

                if (owner.ServiceCenterIds.Contains(payload.ServiceCenterId))
                {
                    owner.ServiceCenterIds.Remove(payload.ServiceCenterId);
                    _context.SaveChanges();
                }

                _logger.LogInformation("ServiceCenter {scId} removed from Owner {ownerId}", payload.ServiceCenterId, payload.OwnerId);
                return ResponseFactory.Success("ServiceCenter removed from Owner successfully", payload.ServiceCenterId);
            }, "Error in RemoveServiceCenter");
        }
    }
}
