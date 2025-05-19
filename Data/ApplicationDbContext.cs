using Microsoft.EntityFrameworkCore;
using WebAppNGS.Models;

namespace WebAppNGS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<Facturation> Facturations { get; set; }
        public DbSet<Stock> Stocks { get; set; }


    }
}
