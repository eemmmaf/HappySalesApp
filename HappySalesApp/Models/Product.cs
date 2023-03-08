namespace HappySalesApp.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace HappySales.Models
    {
        public class Product
        {
            public int Id { get; set; } //PK

            [Required(ErrorMessage = "Namn måste fyllas i")]
            [Display(Name = "Namn")]
            public string? Name { get; set; }

            [Required(ErrorMessage = "Beskrivning måste fyllas i")]
            [Display(Name = "Beskrivning")]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Pris måste fyllas i")]
            [Display(Name = "Pris")]
            [DisplayFormat(DataFormatString = "{0:n2}")]
            public decimal? Price { get; set; }


            //------- Datum ------------//

            [Display(Name = "Publicerad")]
            public DateTime? CreatedDate { get; } = DateTime.Now;

            [Display(Name = "Senast ändrad")]
            public DateTime? LastModifiedDate { get; } = DateTime.Now;



            //-------- Bild ---------//

            [Display(Name = "Filnamn - Bild")]
            public string? ImageName { get; set; }

            [Display(Name = "Alt-text")]
            public string? AltText { get; set; }

            [NotMapped]
            [DisplayName("Ladda upp fil")]
            public IFormFile? ImageFile { get; set; }



            //---------FK Kategori------------//
            public int CategoryId { get; set; }
            public Category? Category { get; set; }


            //---------FK Användare------------//
            [ForeignKey("User")]
            public string? User_Id { get; set; }
            public IdentityUser? User { get; set; }

        }

        //Kategori
        public class Category
        {
            //PK
            public int CategoryId { get; set; }
            public string? CategoryName { get; set; }
            public string? CategoryDescription { get; set; }

            //Navigation property
            public List<Product>? Product { get; set; }

        }
    }

}
