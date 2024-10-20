using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Customer
    {
        //Personal details
        public int CustomerId { get; set; }
        [Required]
        [Column("First Name")]
        [MaxLength(50)]
        public string FirstName { get; set; } //User
        [Required]
        [Column("Last Name")]
        [MaxLength(50)]
        public string LastName { get; set; } //User
        [Column("Date Of Birth", TypeName = "date")]
        public DateTime BirthDate { get; set; } //User 
        [MaxLength(10)]
        [MinLength(10)]
        [Required]

        public string Mobile { get; set; } //User

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        [Column("Email Address")]
        public string Email { get; set; } //User

        [Column("Normalized Email")]
        public string NormalizedEmail { get; set; } //User

        //Address
        [Required]
        [Column("House/Flat No.")]
        [MaxLength(100)]
        public string HouseNo { get; set; } //User


        [Required]
        [MaxLength(100)]
        [Column("Address Line 1")]
        public string AddressLine1 { get; set; } //User


        [Column("Address Line 2")]
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
        public string PinCode { get; set; }

        //Banking detailes
        [Column("Account Number")]
        public long AccountNumber { get; set; }
        [Column("CRN")]
        public long CRN { get; set; }
        [Required]
        [Column("Account Type")]
        public string AccountType { get; set; } //SAVING/ SALARY/ CURRENT
        [Column("Branch Name")]
        public string Branch { get; set; }
        [Column("Ifsc Code")]
        public string IfscCode { get; set; }
        [Column("Opening date")]
        [DataType(DataType.Date)]
        public DateTime OpeningDate { get; set; }
        [Column("Account Balance")]
        public decimal AccountBalance { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public bool IsClosed { get; set; } = false;

        //Nominee details
        [Required]
        [Column("Nominee Name")]
        public string NomineeName { get; set; }
        //[Column("Active_Status")]
        [Required]
        [Column("Relation with nominee")]
        public string RelationWithNominee { get; set; }
        [Required]
        [Column("Nominee birth date", TypeName = "date")]
        public DateTime NomineeDOB { get; set; }

        //one to many
        public ICollection<Card> Cards { get; set; }
    }
}
