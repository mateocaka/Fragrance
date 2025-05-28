using Fragrance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fragrance.Models
{
    public class ShoppingCart
    {
        
        public int ShoppingCartId { get; set; }
     
        public int ParfumeId { get; set; }
        [ForeignKey("ParfumeId")]

        [ValidateNever]
        public Parfume Parfume { get; set; }
        [Range(1, 1000, ErrorMessage = "Please enter a number bettwen 1 to 1000")]
        public int Count { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [NotMapped]
        public double Price { get; set; }
       
        public int Size { get; set; }
    }
}