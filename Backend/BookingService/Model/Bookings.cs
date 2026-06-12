using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingService.Model
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }

    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public int ServiceCenterId { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        [Required]
        public DateOnly Date { get; set; }
        public string Status { get; set; } = BookingStatus.Pending.ToString();

    }
}
