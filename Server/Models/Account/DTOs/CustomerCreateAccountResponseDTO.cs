namespace Server.Models.Account.DTOs
{
    public class CustomerCreateAccountResponseDTO
    {
        public long AccountNumber { get; set; }
        public string IfscCode { get; set; }
        public long CRN { get; set; }
        public string Branch { get; set; }
        public string AccountType { get; set; }

        //Card details
        public long CardNumber { get; set; }
        public string CardIssuer { get; set; } // VISA/ MASTERCARD/ RUPAY...

        //PLATINUM/ MONEYBACK/ MILLENIA/ BUSINESS....
        public string CardType { get; set; }
        // MM/YY
        public string ExpiryDate { get; set; }
        public int Cvv { get; set; }
        public string NameOnCard { get; set; }
    }
}
