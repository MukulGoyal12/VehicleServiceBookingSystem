using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.Marshalling;

namespace ServiceCenterService.Models
{
    public class ServiceCenter
    {
        [Column(TypeName = "varchar(20)")]
        public string ServiceCenterID { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact must be 10 digits.")]
        [Column(TypeName = "varchar(10)")]
        public string Contact { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public string ServiceDescription { get; set; } = string.Empty;

    }
}
