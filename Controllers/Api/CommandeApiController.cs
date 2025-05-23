﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppNGS.Data;
using WebAppNGS.Models;

namespace WebAppNGS.Controllers.Api
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CommandeApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommandeApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Commande
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Commande>>> GetCommandes()
        {
            return await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Produit)
                .ToListAsync();
        }

        // GET: api/Commande/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Commande>> GetCommande(int id)
        {
            var commande = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.Produit)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (commande == null)
                return NotFound();

            return commande;
        }

        // POST: api/Commande
        [HttpPost]
        public async Task<ActionResult<Commande>> PostCommande(Commande commande)
        {
            commande.DateCommande = DateTime.Now;
            _context.Commandes.Add(commande);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommande), new { id = commande.Id }, commande);
        }

        // PUT: api/Commande/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommande(int id, Commande commande)
        {
            if (id != commande.Id)
                return BadRequest();

            _context.Entry(commande).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Commandes.Any(e => e.Id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/Commande/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommande(int id)
        {
            var commande = await _context.Commandes.FindAsync(id);
            if (commande == null)
                return NotFound();

            _context.Commandes.Remove(commande);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
