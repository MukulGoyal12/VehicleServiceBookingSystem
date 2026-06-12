namespace BookingService.DTO
{
    public class CreateBookingDTO
    {
        public int VehicleId { get; set; }
        public int ServiceCenterId { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public DateOnly Date { get; set; }

    }
}
