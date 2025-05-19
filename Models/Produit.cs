using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppNGS.Models
{
    public class Produit
    {
        public int Id { get; set; }

        public required string Nom { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Prix { get; set; }
    }
}
