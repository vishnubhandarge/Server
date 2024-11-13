using System.ComponentModel.DataAnnotations;

namespace Server.Models.Netbanking.DTOs
{
    public class NetbankingLogoutDTO
    {
        [Required]
        public string UserId { get; set; }
    }
}