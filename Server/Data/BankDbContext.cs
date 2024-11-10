using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
    public class BankDbContext(DbContextOptions<BankDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Card> Cards { get; set; }
    }
   
}
