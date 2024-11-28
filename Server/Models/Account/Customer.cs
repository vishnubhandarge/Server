using Server.Models.Netbanking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Account
{
    public class Customer
    {
        //Personal details
        public int CustomerId { get; set; }

        [Required]
        [Column("FirstName")]
        [MaxLength(50)]
        public string FirstName { get; set; } //User

        [Required]
        [Column("LastName")]
        [MaxLength(50)]
        public string LastName { get; set; } //User

        [Column("BirthDate", TypeName = "date")]
        public DateTime BirthDate { get; set; } //User 

        [MaxLength(10)]
        [MinLength(10)]
        [Required]
        public string Mobile { get; set; } //User

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        [Column("EmailAddress")]
        public string Email { get; set; } //User

        [Column("NormalizedEmail")]
        public string NormalizedEmail { get; set; } //User

        //Address
        [Required]
        [Column("House/Flat_No.")]
        [MaxLength(100)]
        public string HouseNo { get; set; } //User

        [Required]
        [MaxLength(100)]
        [Column("AddressLine1")]
        public string AddressLine1 { get; set; } //User

        [Column("AddressLine2")]
        [MaxLength(100)]
        public string? AddressLine2 { get; set; } //User

        [Required]
        [MaxLength(50)]
        public string City { get; set; } //User

        [Required]
        [MaxLength(50)]
        public string District { get; set; } //User

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
        [Column("AccountNumber")]
        [ForeignKey("AccountNumber")]
        public long AccountNumber { get; set; }
        [Column("CRN")]
        public long CRN { get; set; }
        [Required]
        [Column("AccountType")]
        public string AccountType { get; set; } //SAVING/ SALARY/ CURRENT
        [Column("BranchName")]
        public string Branch { get; set; }
        [Column("IfscCode")]
        public string IfscCode { get; set; }
        [Column("Openingdate")]
        [DataType(DataType.Date)]
        public DateTime OpeningDate { get; set; }
        [Column("AccountBalance")]
        public decimal AccountBalance { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public bool IsClosed { get; set; } = false;
        public bool IsEmailVerified { get; set; } = false;
        public bool IsNumberVerified { get; set; } = false;


        //Nominee details
        [Required]
        [Column("NomineeName")]
        public string NomineeName { get; set; }
        //[Column("Active_Status")]
        [Required]
        [Column("RelationWithNominee")]
        public string RelationWithNominee { get; set; }
        [Required]
        [Column("NomineeBirthDate", TypeName = "date")]
        public DateTime NomineeDOB { get; set; }

        ////one to many
        [InverseProperty("Customer")]
        public ICollection<Card> Cards { get; set; } = new List<Card>();
    }
}
