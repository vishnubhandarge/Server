namespace Server.Models.DTOs
{
    public class DeactivateAccountDTO : id
    {
        public long AccountNumber { get; set; }
        public string AccountType { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
