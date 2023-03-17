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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        public IActionResult Create(int? id)
        {
            // Hämtar användarens ID
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //ViewBag med användarens ID och produktens ID
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


                // Kontrollera att id är en INT
                if (int.TryParse(Request.Form["ProductId"], out int productId))
                {
                    // Tilldela produktens id
                    bid.ProductId = productId;
                }

                // Sätt CreatedDate till nuvarande tidpunkt
                bid.CreatedDate = DateTime.Now;

                // Lägg till budet i databasen och redirectar tillbaka
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> AcceptBid(int bidId)
        {
            //Hämtar bud utifrån id
            var bid = await _context.Bid.FindAsync(bidId);

            if (bid == null)
            {
                return NotFound();
            }

            //Sätter IsApproved till true om budet accepteras
            bid.IsApproved = true;
            await _context.SaveChangesAsync();

            //Produktens id
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
            return RedirectToAction("Details", "Products", new { id = product?.Id });
        }



        private bool BidExists(int id)
        {
            return (_context.Bid?.Any(e => e.BidId == id)).GetValueOrDefault();
        }
    }
}
