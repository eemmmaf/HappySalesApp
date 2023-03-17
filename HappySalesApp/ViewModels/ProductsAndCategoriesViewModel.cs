using HappySalesApp.Models.HappySales.Models;

namespace HappySalesApp.ViewModels
{
    public class ProductsAndCategoriesViewModel
    {

        public Product? Product { get; set; }
        public IEnumerable<Product>? Products { get; set; }
        public IEnumerable<Category>? Categories { get; set; }

        public IEnumerable<Bid>? Bids { get; set; }
        public Dictionary<Category, int>? ProductCounts { get; internal set; }
    }
}
