using System.ComponentModel.DataAnnotations;

namespace BookingService.DTO
{
    public class UpdateBookingDTO
    {
        [Required]
        public string ServiceType { get; set; } = string.Empty;

        [Required]
        public DateOnly Date { get; set; }
    }
}
