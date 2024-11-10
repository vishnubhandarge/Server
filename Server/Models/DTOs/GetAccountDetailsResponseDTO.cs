namespace Server.Models.DTOs
{
    public class GetAccountDetailsResponseDTO
    {
        //Bank details
        public long AccountNumber { get; set; }
        public string Branch { get; set; }
        public string IfscCode { get; set; }
        public long CRN { get; set; }
        public string AccountType { get; set; }
        public DateTime OpeningDate { get; set; }
        public decimal AccountBalance { get; set; } = 0;

        //Nominee Details
        public string NomineeName { get; set; }
        public string RelationWithNominee { get; set; }
        public DateTime NomineeDOB { get; set; }
    }
}
