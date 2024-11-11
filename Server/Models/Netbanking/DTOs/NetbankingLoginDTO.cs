namespace Server.Models.Netbanking.DTOs
{
    public class NetbankingLoginDTO
    {
        public long CRN { get; set; }
        public required string Password { get; set; }
    }
}