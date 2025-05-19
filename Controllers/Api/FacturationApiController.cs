using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppNGS.Data;
using WebAppNGS.Models;

namespace WebAppNGS.Controllers.Api
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class FacturationApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FacturationApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Facturation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Facturation>>> GetFacturations()
        {
            return await _context.Facturations
                .Include(f => f.Commande)
                    .ThenInclude(c => c.Client)
                .Include(f => f.Commande)
                    .ThenInclude(c => c.Produit)
                .Where(f => f.Commande != null)
                .ToListAsync();
        }

        // GET: api/Facturation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Facturation>> GetFacturation(int id)
        {
            var facture = await _context.Facturations
                .Include(f => f.Commande)
                    .ThenInclude(c => c.Client)
                .Include(f => f.Commande)
                    .ThenInclude(c => c.Produit)
                .Where(f => f.Commande != null)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (facture == null)
                return NotFound();

            return facture;
        }

        // POST: api/Facturation
        [HttpPost]
        public async Task<ActionResult<Facturation>> PostFacturation(Facturation facture)
        {
            facture.DateFacture = DateTime.Now;
            _context.Facturations.Add(facture);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFacturation), new { id = facture.Id }, facture);
        }

        // DELETE: api/Facturation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFacturation(int id)
        {
            var facture = await _context.Facturations.FindAsync(id);
            if (facture == null)
                return NotFound();

            _context.Facturations.Remove(facture);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
