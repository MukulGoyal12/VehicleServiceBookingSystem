using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceCenterService.Models
{
    public class ServiceCenter
    {
        [Key]
        [Column(TypeName = "varchar(20)")]
        public string ServiceCenterID { get; set; } = string.Empty;

        [Required]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Center name must be between 2 and 150 characters.")]
        public string CenterName { get; set; } = string.Empty;

        // Address fields
        [Required]
        [StringLength(50)]
        public string FlatNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string NearestLandmark { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be exactly 6 digits.")]
        [Column(TypeName = "varchar(6)")]
        public string Pincode { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact must be exactly 10 digits.")]
        [Column(TypeName = "varchar(10)")]
        public string Contact { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string OwnerId { get; set; } = string.Empty;

        [Required]
        [StringLength(1100, ErrorMessage = "Description cannot exceed 1100 characters.")]
        public string ServiceDescription { get; set; } = string.Empty;

        [Range(1, 500, ErrorMessage = "Capacity must be between 1 and 500.")]
        public int Capacity { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public enum CenterStatus { Inactive = 0, Active = 1 }

        [Required]
        public CenterStatus Status { get; set; } = CenterStatus.Active;

        [NotMapped]
        public string FullAddress => $"{FlatNumber}, {Street}, Near {NearestLandmark}, {City}, {State} - {Pincode}";
    }
}
