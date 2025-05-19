// Models/Stock.cs
namespace WebAppNGS.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public string NomProduit { get; set; } = string.Empty;
        public int Quantite { get; set; }
    }
}
