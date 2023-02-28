namespace HappySalesApp.Models
{
    using Microsoft.AspNetCore.Identity;
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
            public decimal? Price { get; set; }


            //------- Datum ------------//

            [Display(Name = "Publicerad")]
            public DateTime? CreatedDate { get; } = DateTime.Now;

            [Display(Name = "Senast ändrad")]
            public DateTime? LastModifiedDate { get; } = DateTime.Now;



            //-------- Bild ---------//
            [Required(ErrorMessage = "Bild måste väljas")]
            [Display(Name = "Filnamn")]
            public string? FileName { get; set; }

            [Required(ErrorMessage = "Beskriv bilden kort")]
            [Display(Name = "Alt-text")]
            public string? AltText { get; set; }

            [NotMapped]
            [Required(ErrorMessage = "Fil måste väljas")]
            [Display(Name = "Bild")]
            public IFormFile? ImageFile { get; set; }



            //---------FK Kategori------------//
            public int CategoryId { get; set; }
            public Category? Category { get; set; }


            //---------FK Användare------------//
            [ForeignKey("User_Id")]
            public int User_Id { get; set; }
            public virtual IdentityUser IdentityUser { get; set; }

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
