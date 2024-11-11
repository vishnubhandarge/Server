using System.ComponentModel.DataAnnotations;


namespace Server.Models.Account.DTOs
{
    public class MinAgeAttribute : ValidationAttribute
    {
        private readonly int _minAge;

        public MinAgeAttribute(int minAge)
        {
            _minAge = minAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime birthDate)
            {
                var today = DateTime.Today;
                var age = today.Year - birthDate.Year;
                if (birthDate.Date > today.AddYears(-age)) age--;
                if (age >= _minAge)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult($"Age must be at least {_minAge} years.");
                }
            }
            return new ValidationResult("Invalid birth date.");
        }
    }

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
        [MinAge(15)]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [Required]
        [MaxLength(10)]
        [MinLength(10)]
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

        //Nominee details
        [Required]
        public string NomineeName { get; set; }
        //[Column("Active_Status")]

        [Required]
        public string RelationWithNominee { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NomineeDOB { get; set; }
        //Banking
        [Required]
        public string AccountType { get; set; }
    }
}
