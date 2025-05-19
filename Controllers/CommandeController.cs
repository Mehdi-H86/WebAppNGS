using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppNGS.Data;
using WebAppNGS.Models;

namespace WebAppNGS.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class CommandeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommandeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var commandes = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Produit)
                .ToListAsync();

            return View(commandes);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Clients = _context.Clients.ToList();
            ViewBag.Produits = _context.Produits.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Commande commande)
        {
            if (ModelState.IsValid)
            {
                commande.DateCommande = DateTime.Now;
                _context.Add(commande);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Commande créée avec succès.";
                return RedirectToAction(nameof(Index));
            }

            // Si erreur, recharger les listes
            ViewBag.Clients = _context.Clients.ToList();
            ViewBag.Produits = _context.Produits.ToList();
            return View(commande);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var commande = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Produit)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (commande == null)
                return NotFound();

            return View(commande);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var commande = await _context.Commandes.FindAsync(id);
            if (commande != null)
            {
                _context.Commandes.Remove(commande);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Commande supprimée avec succès.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DebugAdd()
        {
            var client = _context.Clients.First();
            var produit = _context.Produits.First();

            var commande = new Commande
            {
                ClientId = client.Id,
                Client = client,
                ProduitId = produit.Id,
                Produit = produit,
                DateCommande = DateTime.Now
            };

            _context.Add(commande);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Commande ajoutée manuellement.";
            return RedirectToAction(nameof(Index));
        }

    }
}
