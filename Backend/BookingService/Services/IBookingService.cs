using BookingService.DTO;
using BookingService.Model;

namespace BookingService.Services
{
    public interface IBookingService
    {
        Task<Booking> CreateBookingAsync(int userId, CreateBookingDTO dto);
        Task<Booking?> GetBookingByIdAsync(int id, int userId);
        Task<Booking?> UpdateBookingAsync(int id, int userId, UpdateBookingDTO dto);
        Task<bool> CancelBookingAsync(int id, int userId);
        Task<List<Booking>> GetUserBookingsAsync(int userId);

    }
}
