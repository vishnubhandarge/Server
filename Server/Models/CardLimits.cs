namespace Server.Models
{
    public class CardLimits : Card
    {
        public int CardLimitId { get; set; }
        public decimal WithdwaralLimit { get; set; }
        public decimal POSLimit { get; set; }
        public decimal OnlineLimit { get; set; }
        public decimal ContactlessLimit { get; set; }
    }
}
