using Microsoft.EntityFrameworkCore;

namespace sta.Models
{
    public class ProdutosContext : DbContext
    {
        public ProdutosContext(DbContextOptions<ProdutosContext> options) : base(options)
        {
        }
        public DbSet<TodosProdutos> TodoProdutos { get; set; } = null!;
    }
}
