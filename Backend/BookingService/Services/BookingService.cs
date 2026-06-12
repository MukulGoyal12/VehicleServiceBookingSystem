
using BookingService.DTO;
using BookingService.Model;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Services
{
    public class BookingServiceImp : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingServiceImp(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> CreateBookingAsync(int userId, CreateBookingDTO dto)
        {
            try
            {
                var booking = new Booking
                {
                    UserId = userId,
                    VehicleId = dto.VehicleId,
                    ServiceCenterId = dto.ServiceCenterId,
                    ServiceType = dto.ServiceType,
                    Date = dto.Date,
                    Status = BookingStatus.Pending.ToString()
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return booking;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating booking: {ex.Message}");
            }
        }

        public async Task<Booking?> GetBookingByIdAsync(int id, int userId)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null || booking.UserId != userId)
                    return null;
                return booking;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching booking: {ex.Message}");
            }
        }

        public async Task<Booking?> UpdateBookingAsync(int id, int userId, UpdateBookingDTO dto)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null || booking.UserId != userId)
                    return null;

                if (booking.Status != BookingStatus.Pending.ToString())
                    return null;

                if (!string.IsNullOrEmpty(dto.ServiceType))
                    booking.ServiceType = dto.ServiceType;

                if (dto.Date != default && dto.Date >= DateOnly.FromDateTime(DateTime.Today))
                    booking.Date = dto.Date;

                await _context.SaveChangesAsync();
                return booking;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating booking: {ex.Message}");
            }
        }

        public async Task<bool> CancelBookingAsync(int id, int userId)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null || booking.UserId != userId)
                    return false;

                if (booking.Status == BookingStatus.Completed.ToString() ||
                    booking.Status == BookingStatus.Cancelled.ToString())
                    return false;

                booking.Status = BookingStatus.Cancelled.ToString();
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error cancelling booking: {ex.Message}");
            }
        }

        public async Task<List<Booking>> GetUserBookingsAsync(int userId)
        {
            try
            {
                return await _context.Bookings
                    .Where(b => b.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching user bookings: {ex.Message}");
            }
        }

    }
}