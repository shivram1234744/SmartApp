using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrustPlus.Models
{
   
    public class EmployeeMst
    {
        [Key]   // Primary Key
        public int EmployeeId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Address { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter valid 10 digit mobile number")]
        public string MobileNo { get; set; }

    }
}
