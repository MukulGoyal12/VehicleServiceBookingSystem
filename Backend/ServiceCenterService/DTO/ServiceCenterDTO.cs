using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceCenterService.DTO
{
    public class ServiceCenterDTO
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string CenterName { get; set; } = string.Empty;

        [Required]
        public string FlatNumber { get; set; } = string.Empty;

        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string NearestLandmark { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be 6 digits.")]
        public string Pincode { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact must be 10 digits.")]
        public string Contact { get; set; } = string.Empty;

        [Required]
        [StringLength(1100, ErrorMessage = "Description cannot exceed 1100 characters.")]
        public string ServiceDescription { get; set; } = string.Empty;

        [Range(1, 500)]
        public int Capacity { get; set; }
    }


}
