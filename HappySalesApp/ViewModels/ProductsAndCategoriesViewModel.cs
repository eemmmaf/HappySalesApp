using HappySalesApp.Models.HappySales.Models;

namespace HappySalesApp.ViewModels
{
    public class ProductsAndCategoriesViewModel
    {
        public IEnumerable<Product>? Products { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
    }
}
