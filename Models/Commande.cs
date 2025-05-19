namespace WebAppNGS.Models
{
    public class Commande
    {
        public int Id { get; set; }
        public DateTime DateCommande { get; set; }

        // FK vers Client
        public int ClientId { get; set; }
        public  Client? Client { get; set; }

        // FK vers Produit
        public int ProduitId { get; set; }
        public  Produit? Produit { get; set; }
    }
}
