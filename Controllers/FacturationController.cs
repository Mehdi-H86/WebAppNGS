using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppNGS.Data;
using WebAppNGS.Models;

namespace WebAppNGS.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class FacturationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacturationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var factures = _context.Facturations
                .Include(f => f.Commande)
                .ThenInclude(c => c.Client);

            return View(await factures.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Clients = _context.Clients.ToList();
            ViewBag.Commandes = _context.Commandes
                .Include(c => c.Produit)
                .ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Facturation facture)
        {
            if (ModelState.IsValid)
            {
                facture.DateFacture = DateTime.Now;
                _context.Add(facture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clients = _context.Clients.ToList();
            ViewBag.Commandes = _context.Commandes.Include(c => c.Produit).ToList();
            return View(facture);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var facture = await _context.Facturations
                .Include(f => f.Commande)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (facture == null) return NotFound();
            return View(facture);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facture = await _context.Facturations.FindAsync(id);
            if (facture == null)
            {
                TempData["Error"] = "Facture introuvable ou déjà supprimée.";
                return RedirectToAction(nameof(Index));
            }

            _context.Facturations.Remove(facture);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Facture supprimée avec succès.";
            return RedirectToAction(nameof(Index));
       
        }
    }
}
