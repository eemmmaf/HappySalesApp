using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HappySalesApp.Data;
using HappySalesApp.Models.HappySales.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;

namespace HappySalesApp.Controllers
{
    public class BidsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BidsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Bids
        public async Task<IActionResult> Index()
        {
            // Hämta den inloggade användarens Id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Hämta alla bud som har samma användar-id som den inloggade användaren
            var bids = await _context.Bid
                .Include(b => b.Product)
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return View(bids);
        }

        // GET: Bids/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bid == null)
            {
                return NotFound();
            }

            var bid = await _context.Bid
                .Include(b => b.Product)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BidId == id);
            if (bid == null)
            {
                return NotFound();
            }

            return View(bid);
        }

        // GET: Bids/Create
        public IActionResult Create(int? id)
        {
            // Här kan du hämta användarens ID från din autentiseringslogik
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sätt ViewBag med användarens ID och produktens ID
            ViewBag.UserId = userId;
            ViewBag.ProductId = id;

            return View();
        }

        // POST: Bids/Create
        // Metod för att lägga till bud

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bid bid)
        {
            if (ModelState.IsValid)
            {
                // Lägg till produktens ID och användarens ID till budet
                bid.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Kontrollera att värdet för "id" är en giltig heltalssträng
                if (int.TryParse(Request.Form["ProductId"], out int productId))
                {
                    // Tilldela produkt-ID:t till budet
                    bid.ProductId = productId;
                }

                // Sätt CreatedDate till nuvarande tidpunkt
                bid.CreatedDate = DateTime.Now;

                // Lägg till budet i databasen och redirectar tillbaka användaren
                _context.Add(bid);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Products", new { id = bid.ProductId });
            }
            else
            {
                // Hantera felaktig productId i Query-stringen
                return BadRequest("Invalid productId");
            }

        }

        // GET: Bids/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bid == null)
            {
                return NotFound();
            }

            var bid = await _context.Bid.FindAsync(id);
            if (bid == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", bid.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", bid.UserId);
            return View(bid);
        }

        // POST: Bids/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,UserId,ProductId")] Bid bid)
        {
            if (id != bid.BidId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bid);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BidExists(bid.BidId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", bid.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", bid.UserId);
            return View(bid);
        }

        // GET: Bids/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bid == null)
            {
                return NotFound();
            }

            var bid = await _context.Bid
                .Include(b => b.Product)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BidId == id);
            if (bid == null)
            {
                return NotFound();
            }

            return View(bid);
        }

        // POST: Bids/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bid == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Bid'  is null.");
            }
            var bid = await _context.Bid.FindAsync(id);
            if (bid != null)
            {
                _context.Bid.Remove(bid);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BidExists(int id)
        {
            return (_context.Bid?.Any(e => e.BidId == id)).GetValueOrDefault();
        }
    }
}
