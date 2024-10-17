using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.DTOs
{
    public class CustomerCreateAccountDTO
    {

        //Personal
        [Required]

        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]

        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        [Column(TypeName = "date")]
        public DateTime BirthDate { get; set; }
        [Required]
        [MaxLength(10)]
        [MinLength(10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid mobile number. It should be 10 digits.")]
        public string Mobile { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(100)]

        public string Email { get; set; }


        //Address
        [Required]

        public string HouseNo { get; set; } //User
        [Required]
        [MaxLength(100)]

        public string AddressLine1 { get; set; } //User
        [MaxLength(100)]
        public string? AddressLine2 { get; set; } //User
        [Required]
        [MaxLength(50)]
        public string Taluka { get; set; } //User
        [Required]
        [MaxLength(50)]
        public string City { get; set; } //User
        [Required]
        [MaxLength(50)]
        public string State { get; set; } //User
        [Required]
        [MaxLength(50)]
        public string Country { get; set; } //User
        [Required]
        [MinLength(6)]
        [MaxLength(10)]
        public int PinCode { get; set; }


        //Nominee details
        [Required]

        public string NomineeName { get; set; }
        //[Column("Active_Status")]
        [Required]

        public string RelationWithNominee { get; set; }
        [Required]
        [Column(TypeName = "date")]
        public DateTime NomineeDOB { get; set; }


        //Banking
        [Required]
        public string AccountType { get; set; }
    }
}
