using Microsoft.EntityFrameworkCore;

namespace sta.Models
{
    public class OrdersContext : DbContext
    {
        public OrdersContext(DbContextOptions<OrdersContext> options) : base(options)
        {
        }

        public DbSet<Orders> Orders { get; set; } = null!;
    }
}
