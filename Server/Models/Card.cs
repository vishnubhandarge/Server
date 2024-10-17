namespace Server.Models
{
    public class Card
    {
        public int CardId { get; set; }
        public string CardNumber { get; set; }


        public string CardIssuer { get; set; } // VISA/ MASTERCARD/ RUPAY...

        //PLATINUM/ MONEYBACK/ MILLENIA/ BUSINESS....
        public string CardType { get; set; }
        // MM/YY
        public DateTime ExpiryDate { get; set; }
        public int Cvv { get; set; }
        public string NameOnCard { get; set; }
        public bool IsActive { get; set; } = true;

        //many to one relation
        public int CustomerId { get; set; }
        public Customer customer { get; set; }
    }
}
