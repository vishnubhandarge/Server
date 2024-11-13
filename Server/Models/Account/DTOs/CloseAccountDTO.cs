using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Server.Models.Account.DTOs
{
    public class CloseAccountDTO
    {
        [Required]
        public long AccountNumber { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [Required]
        public long CRN { get; set; }
    }
}