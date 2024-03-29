﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HappySalesApp.Models
{
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
            public int Price { get; set; }


            //------- Datum ------------//

            [Display(Name = "Publicerad")]
            public DateTime? CreatedDate { get; set; } = DateTime.Now;


            //-------- Bild ---------//

            [Display(Name = "Filnamn - Bild")]
            public string? ImageName { get; set; }

            [Display(Name = "Bildbeskrivning")]
            [Required(ErrorMessage = "Bilden behöver en kort beskrivning")]
            public string? AltText { get; set; }

            [NotMapped]
            [DisplayName("Fil - Bild")]
            public IFormFile? ImageFile { get; set; }



            //---------FK Kategori------------//
            [DisplayName("Kategori")]
            public int CategoryId { get; set; }
            [Display(Name = "Kategori")]
            public Category? Category { get; set; }


            //---------FK Användare------------//
            [ForeignKey("User")]
            public string? User_Id { get; set; }
            public IdentityUser? User { get; set; }


            // Property som har antalet produkter i samma kategori
            [NotMapped]
            public int ProductCount { get; set; }

            // Navigation property till Bids
            public List<Bid>? Bids { get; set; }

            //Bool för att se om produkten är såld eller inte
            public bool IsSold { get; set; }

        }

        //Kategori
        public class Category
        {
            //PK
            public int CategoryId { get; set; }
            [Display(Name = "Kategorins namn")]
            public string? CategoryName { get; set; }
            public string? CategoryDescription { get; set; }

            //Navigation property
            public List<Product>? Product { get; set; }

        }


        public class Bid
        {
            [Key]
            public int BidId { get; set; } // PK


            [DisplayName("Bud")]
            [Required(ErrorMessage = "Bud måste fyllas i")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Budet måste vara mer än 0")]
            public int Amount { get; set; }

            [DisplayName("Bud lagt")]
            public DateTime CreatedDate { get; set; } 

            //Bool för att bestämma om budet är godkänt eller inte
            public bool IsApproved { get; set; }

            public string? UserId { get; set; }
            public IdentityUser? User { get; set; }


            //--------- FK till produkt -------//
            public int ProductId { get; set; } // FK

            [DisplayName("Annons")]
            public Product? Product { get; set; } // Navigation property
        }
    }

}
