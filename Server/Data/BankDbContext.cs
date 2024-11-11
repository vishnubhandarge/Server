using Microsoft.EntityFrameworkCore;
using Server.Models.Account;
using Server.Models.Netbanking;

namespace Server.Data
{
    public class BankDbContext(DbContextOptions<BankDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<User> Users { get; set; }
    }  
}
