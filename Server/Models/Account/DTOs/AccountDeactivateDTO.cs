using System.ComponentModel.DataAnnotations;

namespace Server.Models.Account.DTOs
{
    public class AccountDeactivateDTO
    {
        [Required]
        public required long CRN { get; set; }
        [Required]
        public long AccountNumber { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}
