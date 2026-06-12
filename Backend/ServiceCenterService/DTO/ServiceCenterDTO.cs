using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceCenterService.DTO
{
    public class ServiceCenterDTO
    {
        [StringLength(100, MinimumLength = 2)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? Location { get; set; } = string.Empty;

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact must be 10 digits.")]
        [Column(TypeName = "varchar(10)")]
        public string? Contact { get; set; }
        public string ServiceDescription { get; set; }
    }

}
