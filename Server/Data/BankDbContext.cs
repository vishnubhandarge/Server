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
//using Microsoft.EntityFrameworkCore;
//using Server.Models;

//namespace Server.Data
//{
//    public class BankDbContext : DbContext
//    {
//        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
//        {
//        }

//        public DbSet<Customer> Customers { get; set; }
//        public DbSet<Card> Cards { get; set; }

//        protected override void OnModelCreating(ModelBuilder modelBuilder) 
//        {
//            modelBuilder.Entity<Customer>().HasMany(c => c.Cards)
//                .WithOne(o => o.customer)
//                .HasForeignKey(o => o.AccountNumber); 
//            base.OnModelCreating(modelBuilder);
//        }
//    }
//}