using VehicleService.DTO;
using VehicleService.Models;

namespace VehicleService.Services
{
    public interface IVehicleService
    {
        Task<List<Vehicle>> GetMyVehicles(int userId);
        Task<Vehicle?> GetVehicleById(int id, int userId);
        Task<Vehicle> AddVehicle(VehicleDTO vehicleDto, int userId);
        Task<Vehicle?> PatchVehicle(int id, VehicleDTO vehicleDto, int userId);
        Task<bool> DeleteVehicle(int id, int userId);
    }
}
