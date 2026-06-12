using BookingService.Model;

namespace BookingService.DTO
{
    public class GetBookingDTO
    {
        public int BookingId { get; set; }
        public int ServiceCenterId { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
