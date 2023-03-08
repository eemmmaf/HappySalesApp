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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Drawing;
using LazZiya.ImageResize;
using HappySalesApp.ViewModels;

namespace HappySalesApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        //Thumbnails
        private readonly int ThumbNailWidth = 350;
        private readonly int ThumbNailHeight = 350;

        public ProductsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }


        // GET: Products
        public async Task<IActionResult> Index()
        {
            var viewModel = new ProductsAndCategoriesViewModel
            {
                Products = await _context.Products.ToListAsync(),
                Categories = await _context.Categories.ToListAsync()
            };

            return View(viewModel);
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //GET: Products/ProductsByCategory
        //Hämtar alla produkter utifrån ett visst id
        public async Task<IActionResult> ProductsByCategory(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var products = await (from p in _context.Products
                                  join c in _context.Categories.Distinct() on p.CategoryId equals c.CategoryId
                                  where c.CategoryId == id
                                  select p).ToListAsync();

            var categoryName = await _context.Categories
                .Where(c => c.CategoryId == id)
                .Select(c => c.CategoryName)
                .FirstOrDefaultAsync();

            var categories = await _context.Categories.ToListAsync();

            var viewModel = new ProductsAndCategoriesViewModel
            {
                Products = products,
                Categories = categories
            };

            return View(viewModel);
        }

        // GET: Products/Create
        //Använder Authorize för att gömma formulär för att lägga till
        [Authorize]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "CategoryName");
            return View();
        }


        // POST: Products/Create
        //Använder Authorize för att gömma formulär för att lägga till
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,ImageFile,ImageName,AltText,CategoryId,User_Id")] Product product)
        {
            //Definierar wwwRootPath
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (ModelState.IsValid)
            {
                //Kontroll om bild finns eller inte
                if (product.ImageFile != null)
                {
                    //Sparar bilder till wwwroot och i katalogen uploadedimages
                    string fileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
                    string extension = Path.GetExtension(product.ImageFile.FileName);

                    //Tar bort mellanslag och lägger till datum så att bilderna har unika namn
                    product.ImageName = fileName = fileName.Replace(" ", String.Empty) + DateTime.Now.ToString("yymmssfff") + extension;

                    string path = Path.Combine(wwwRootPath + "/uploadedimages/", fileName);

                    //Lagrar filen
                    using var fileStream = new FileStream(path, FileMode.Create);
                    await product.ImageFile.CopyToAsync(fileStream);

                    //Skapar miniatyr
                    //CreateImages(fileName);
                }


                else
                {
                    product.ImageName = null;
                }



                var user = await _userManager.GetUserAsync(User);
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        //Använder Authorize för att gömma formulär för att lägga till
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //Använder Authorize för att gömma formulär för att lägga till
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,FileName,AltText,CategoryId,User_Id")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryName", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        //Använder Authorize för att gömma formulär för att lägga till
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        //Använder Authorize för att gömma formulär för att lägga till
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //Skapar miniatyr av bild
        private void CreateImages(string filename)
        {
            //Definierar wwwRootPath
            string wwwRootPath = _hostEnvironment.WebRootPath;
            using (var img = System.Drawing.Image.FromFile(Path.Combine(wwwRootPath + "/uploadedimages/", filename)))
            {
                img.Scale(ThumbNailWidth, ThumbNailHeight).SaveAs(Path.Combine(wwwRootPath + "/smallimages/", "thumb_" + filename));
            }
        }
    }
}
