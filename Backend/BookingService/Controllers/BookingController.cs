using BookingService.DTO;
using BookingService.Model;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("my-bookings")]
        [Authorize]
        public async Task<IActionResult> GetMyBookings()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized(new { message = "Invalid or missing user ID in token" });

                var bookings = await _bookingService.GetUserBookingsAsync(userId);

                if (bookings == null || !bookings.Any())
                    return NotFound(new { message = "No bookings found for this user" });

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching user bookings", error = ex.Message });
            }
        }


        [HttpPost("create-booking")]
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO bookingDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized(new { message = "Invalid or missing user ID in token" });

                

                var booking = await _bookingService.CreateBookingAsync(userId, bookingDto);

                return Ok(new
                {
                    message = "Booking created successfully",
                    bookingId = booking.BookingId,
                    userId = booking.UserId,
                    vehicleId = booking.VehicleId,
                    serviceCenterId = booking.ServiceCenterId,
                    status = booking.Status
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating booking", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookingById(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized(new { message = "Invalid or missing user ID in token" });

                var booking = await _bookingService.GetBookingByIdAsync(id, userId);
                if (booking == null)
                    return NotFound(new { message = "Booking not found" });

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching booking", error = ex.Message });
            }
        }

        [HttpPatch("update-booking/{id}")]
        [Authorize]
        public async Task<IActionResult> PatchBooking(int id, [FromBody] UpdateBookingDTO updateDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized(new { message = "Invalid or missing user ID in token" });

                var booking = await _bookingService.UpdateBookingAsync(id, userId, updateDto);
                if (booking == null)
                    return BadRequest(new { message = "Booking not found or cannot be updated" });

                return Ok(new { message = "Booking updated successfully", booking });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating booking", error = ex.Message });
            }
        }

        [HttpDelete("cancel-booking/{id}")]
        [Authorize]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized(new { message = "Invalid or missing user ID in token" });

                var cancelled = await _bookingService.CancelBookingAsync(id, userId);
                if (!cancelled)
                    return BadRequest(new { message = "Booking not found or cannot be cancelled" });

                return Ok(new { message = "Booking cancelled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error cancelling booking", error = ex.Message });
            }
        }
    }
}