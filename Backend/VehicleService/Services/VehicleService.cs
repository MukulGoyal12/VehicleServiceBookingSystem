using Microsoft.EntityFrameworkCore;
using VehicleService.DTO;
using VehicleService.Models;

namespace VehicleService.Services
{
    public class VehicleServiceImpl : IVehicleService
    {
        private readonly VehicleContext _context;

        public VehicleServiceImpl(VehicleContext context)
        {
            _context = context;
        }

        public async Task<List<Vehicle>> GetMyVehicles(int userId)
        {
            return await _context.Vehicles
                .Where(v => v.UserId == userId)
                .ToListAsync();
        }

        public async Task<Vehicle?> GetVehicleById(int id, int userId)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == id && v.UserId == userId);
        }

        public async Task<Vehicle> AddVehicle(VehicleDTO vehicleDto, int userId)
        {
            var vehicle = new Vehicle
            {
                UserId = userId,
                Make = vehicleDto.Make,
                Model = vehicleDto.Model,
                Year = vehicleDto.Year,
                RegistrationNumber = vehicleDto.RegistrationNumber
            };

            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<Vehicle?> PatchVehicle(int id, VehicleDTO vehicleDto, int userId)
        {
            var existingVehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == id && v.UserId == userId);

            if (existingVehicle == null)
                return null;

            if (vehicleDto.Make != null)
                existingVehicle.Make = vehicleDto.Make;

            if (vehicleDto.Model != null)
                existingVehicle.Model = vehicleDto.Model;

            if (vehicleDto.Year != 0)
                existingVehicle.Year = vehicleDto.Year;

            if (vehicleDto.RegistrationNumber != null)
                existingVehicle.RegistrationNumber = vehicleDto.RegistrationNumber;

            await _context.SaveChangesAsync();
            return existingVehicle;
        }

        public async Task<bool> DeleteVehicle(int id, int userId)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return false;

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}