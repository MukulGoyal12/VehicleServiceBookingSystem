using System.ComponentModel.DataAnnotations;

namespace VehicleService.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }
        [Required]
        public int UserId {  get; set; }
        [Required(ErrorMessage = "Make is required.")]
        [StringLength(50)]
        public string Make { get; set; }
        [Required(ErrorMessage = "Model is required.")]
        [StringLength (50)]
        public string Model { get; set; }

        [Range(2000, int.MaxValue, ErrorMessage = "Year must be valid")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Registration number is required.")]
        [StringLength(15)]
        [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "Invalid registration format")]

        public string RegistrationNumber { get; set; }
    }
}
