using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Netbanking
{
    public class User
    {
        public int UserId { get; set; }
        public long CRN { get; set; }
        public  long AccountNumber { get; set; }
        public long CardNumber { get; set; }
        public byte IsRegistered { get; set; }
        public bool IsNetbankingBlocked { get; set; } = false;
        [Required]
        [Column("PasswordSalt")]
        public required byte[] PasswordSalt { get; set; }
        [Column("PasswordHash")]
        [Required]
        public required byte[] PasswordHash { get; set; }

    }
}
