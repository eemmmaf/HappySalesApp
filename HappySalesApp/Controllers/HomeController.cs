using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HappySalesApp.Data;
using HappySalesApp.Models.HappySales.Models;
using HappySalesApp.Models;
using System.Diagnostics;
using HappySalesApp.ViewModels;

namespace HappySalesApp.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            //Returnerar kategorierna på startsidan
            return _context.Categories != null ?
                        View(await _context.Categories.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        }

        public async Task<IActionResult> Search(string searchTerm)
        {

            var viewModel = new ProductsAndCategoriesViewModel
            {
                Products = await _context.Products.ToListAsync(),
                Categories = await _context.Categories.ToListAsync()
            };


            if (!string.IsNullOrEmpty(searchTerm))
            {


                viewModel.Products = await _context.Products
                    .Where(p => p.Name.Contains(searchTerm))
                    .ToListAsync();

            }
            

            return View("Search", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}