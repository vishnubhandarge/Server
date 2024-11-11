namespace Server.Models.Account
{
    public class Card
    {
        public int CardId { get; set; }
        public long CardNumber { get; set; }
        public string CardIssuer { get; set; } // VISA/ MASTERCARD/ RUPAY...

        //PLATINUM/ MONEYBACK/ MILLENIA/ BUSINESS....
        public string CardType { get; set; }
        // MM/YY
        public string ExpiryDate { get; set; }
        public int Cvv { get; set; }
        public string NameOnCard { get; set; }
        public bool IsActive { get; set; } = true;

        //many to one relation
        public long AccountNumber { get; set; }
        public Customer customer { get; set; }
    }
}
