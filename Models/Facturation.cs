using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppNGS.Models
{
    public class Facturation
    {
        public int Id { get; set; }

        public DateTime DateFacture { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Montant { get; set; }

        public int CommandeId { get; set; }

        public Commande? Commande { get; set; }

        public Client? Client => Commande?.Client;

        public int? ClientId => Commande?.ClientId;
    }
}
