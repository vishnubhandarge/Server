using System.ComponentModel.DataAnnotations;

namespace Server.Models.Netbanking.DTOs
{
    public class NetbankingRegistrationDTO
    {
        [Required]
        public long CRN { get; set; }
        [Required]
        public long AccountNumber { get; set; }
        [Required]
        public long CardNumber { get; set; }
        [Required]
        public int Cvv { get; set; }
        [Required]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
