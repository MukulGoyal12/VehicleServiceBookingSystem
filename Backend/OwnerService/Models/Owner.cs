using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OwnerService.Models
{
    public class Owner
    {
        [Column(TypeName = "varchar(20)")]
        public string OwnerId { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be exactly 10 digits.")]
        [Column(TypeName = "varchar(10)")]
        public string Phone { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "Password hash length exceeded.")]
        public string Password { get; set; }
        public IList<string> ServiceCenterIds { get; set; } = new List<string>();
    }
}