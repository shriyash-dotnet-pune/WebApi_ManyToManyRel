using WebApi_ManyToManyRel.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi_ManyToManyRel.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext>options) : base(options)
        {
            
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<BookOrder> BookOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<BookOrder>().HasKey(bo => new {bo.OrderId, bo.BookId});

                // configure relationships
                modelBuilder.Entity<BookOrder>()
                    .HasOne(bo => bo.Book)
                    .WithMany(b => b.BookOrders)
                    .HasForeignKey(bo => bo.BookId);

            modelBuilder.Entity<BookOrder>()
                .HasOne(bo => bo.Order)
                .WithMany(o => o.BookOrders)
                .HasForeignKey(bo => bo.OrderId);

        }
        

    }
}
