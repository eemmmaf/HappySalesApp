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
using HappySalesApp.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;

namespace HappySalesApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;


        public ProductsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }


        // GET: Products
        [Route("/annonser")]
        public async Task<IActionResult> Index(string sortOrder)
        {
            var categories = await _context.Categories.ToListAsync();
            //Dictionary som räknar antalet produkter per kategori
            var productCounts = new Dictionary<Category, int>();

            foreach (var categoryItem in categories)
            {
                var count = await _context.Products.CountAsync(p => p.CategoryId == categoryItem.CategoryId);
                productCounts.Add(categoryItem, count);
            }


            //Viewbags för sortering
            ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.PriceSort = sortOrder == "Price" ? "price_desc" : "Price";

            var productsSort = from s in _context.Products
                               select s;

            var viewModel = new ProductsAndCategoriesViewModel
            {
                Products = await productsSort.ToListAsync(),
                Categories = await _context.Categories.ToListAsync(),
                ProductCounts = productCounts
            };


            // Sorteringsalternativ
            switch (sortOrder)
            {
                case "name_desc":
                    var productsList = await productsSort.ToListAsync();
                    productsList = productsList.OrderByDescending(p => p.Name).ToList();
                    viewModel.Products = productsList;
                    break;
                case "Price":
                    viewModel.Products = viewModel.Products.OrderBy(p => p.Price).ToList();
                    break;
                case "price_desc":
                    viewModel.Products = viewModel.Products.OrderByDescending(p => p.Price).ToList();
                    break;
                case "Date":
                    viewModel.Products = viewModel.Products.OrderBy(p => p.CreatedDate).ToList();
                    break;
                case "date_desc":
                    viewModel.Products = viewModel.Products.OrderByDescending(p => p.CreatedDate).ToList();
                    break;
                default:
                    productsList = await productsSort.ToListAsync();
                    productsList = productsList.OrderBy(p => p.Name).ToList();
                    viewModel.Products = productsList;
                    break;
            }



            return View(viewModel);
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {


            if (id == null)
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

            // Hämta alla bud för den här produkten
            var bids = await _context.Bid
                .Where(b => b.ProductId == product.Id)
                .OrderByDescending(b => b.Amount)
                .ToListAsync();

            // Lägg till produkten och buden till modellen för vyn
            var model = new ProductsAndCategoriesViewModel
            {
                Product = product,
                Bids = bids
            };

            return View(model);
        }

        //GET: Products/ProductsByCategory
        //Hämtar alla produkter utifrån ett visst id
        public async Task<IActionResult> ProductsByCategory(int? id, string sortOrder)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            //Sparar kategorinamn och beskrivning i viewbags
            ViewBag.CategoryName = category.CategoryName;
            ViewBag.CategoryDescription = category.CategoryDescription;

            //Hämtar alla produkter som matchar med kategorins id
            var products = await _context.Products.Where(p => p.CategoryId == id).ToListAsync();
            //Hämtar alla kategorier från databasen
            var categories = await _context.Categories.ToListAsync();

            //Dictionary som räknar antalet produkter per kategori
            var productCounts = new Dictionary<Category, int>();

            foreach (var categoryItem in categories)
            {
                var count = await _context.Products.CountAsync(p => p.CategoryId == categoryItem.CategoryId);
                productCounts.Add(categoryItem, count);
            }

            //Sortering
            ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.PriceSort = sortOrder == "Price" ? "price_desc" : "Price";

            var productsSort = from p in _context.Products
                           where p.CategoryId == id
                           select p;


            var viewModel = new ProductsAndCategoriesViewModel
            {
                Products = await productsSort.ToListAsync(),
                Categories = categories,
                ProductCounts = productCounts
            };


            // Sorteringsalternativ
            switch (sortOrder)
            {
                case "name_desc":
                    var productsList = await productsSort.ToListAsync();
                    productsList = productsList.OrderByDescending(p => p.Name).ToList();
                    viewModel.Products = productsList;
                    break;
                case "Price":
                    viewModel.Products = viewModel.Products.OrderBy(p => p.Price).ToList();
                    break;
                case "price_desc":
                    viewModel.Products = viewModel.Products.OrderByDescending(p => p.Price).ToList();
                    break;
                case "Date":
                    viewModel.Products = viewModel.Products.OrderBy(p => p.CreatedDate).ToList();
                    break;
                case "date_desc":
                    viewModel.Products = viewModel.Products.OrderByDescending(p => p.CreatedDate).ToList();
                    break;
                default:
                    productsList = await productsSort.ToListAsync();
                    productsList = productsList.OrderBy(p => p.Name).ToList();
                    viewModel.Products = productsList;
                    break;
            }

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
                }


                else
                {
                    product.ImageName = null;
                }


             
                // Hämta den inloggade användarens Id och lagrar
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                product.User_Id = userId;
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //Använder Authorize för att gömma formulär för att lägga till
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,ImageFile,ImageName,AltText,CategoryId,User_Id")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            //Definierar wwwRootPath
            string wwwRootPath = _hostEnvironment.WebRootPath;

            if (ModelState.IsValid)
            {
                try
                {
                    // Kontrollera om ImageFile är null
                    if (product.ImageFile == null)
                    {
                        // Hämta den befintliga bilden från databasen
                        var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                        product.ImageFile = null;
                        product.ImageName = existingProduct.ImageName;
                    }
                    else
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
                    }

                    // Hämta den inloggade användarens Id och lagrar
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    product.User_Id = userId;

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
                //Redirectar användare till vyn där alla produkter listas
                return RedirectToAction(nameof(ProductsByUser));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryName", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        //Använder Authorize för att gömma för oinloggade
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
            //Definierar wwwRootPath
            string wwwRootPath = _hostEnvironment.WebRootPath;

            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                //Tar bort bild
                var imagePath = Path.Combine(wwwRootPath + "/uploadedimages/" + product.ImageName);
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            //Redirectar till sidan där alla produkter finns
            return RedirectToAction(nameof(ProductsByUser));
        }


        [Authorize]
        public async Task<IActionResult> ProductsByUser()
        {
            // Hämta den inloggade användarens Id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Hämta produkter som är skapade av den inloggade användaren och sorterar efter skapat-datum
            var products = await _context.Products
                .Where(p => p.User_Id == userId)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            return View(products);
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
