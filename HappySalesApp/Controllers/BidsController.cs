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

                // Hämta annonsens ägare
                var productOwnerId = await _context.Products
                    .Where(p => p.Id == bid.ProductId)
                    .Select(p => p.User_Id)
                    .FirstOrDefaultAsync();

                // Jämför produktens ägare med den inloggade användaren
                if (productOwnerId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return BadRequest("Du kan inte lägga till bud på din egen annons.");
                }

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


        //Metod för att acceptera bud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptBid(int bidId)
        {
            var bid = await _context.Bid.FindAsync(bidId);
            if (bid == null)
            {
                return NotFound();
            }

            bid.IsApproved = true;
            await _context.SaveChangesAsync();

            var productId = bid.ProductId;

            // Hämta produkten som är kopplad till budet
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                // Sätt IsSold till true 
                product.IsSold = true;
                // Uppdatera priset till värdet från det godkända budet
                product.Price = bid.Amount;
                await _context.SaveChangesAsync();
            }

            // Redirectar till produkt-sidan
            return RedirectToAction("Details", "Products", new { id = product.Id });
        }



        private bool BidExists(int id)
        {
            return (_context.Bid?.Any(e => e.BidId == id)).GetValueOrDefault();
        }
    }
}
