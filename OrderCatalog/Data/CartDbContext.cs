using CartService.Model;
using Microsoft.EntityFrameworkCore;
using OrderCatalog.Model;

namespace CartService.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options)
        { }

        public DbSet<Cart> Cart { get; set; }

        public DbSet<Order> Order { get; set; }
        public DbSet<ProductList> ProductList{ get; set; }
    }
}
