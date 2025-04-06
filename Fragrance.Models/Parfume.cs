using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fragrance.Models
{
    public class Parfume
    {
       
        public int ParfumeId { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Author is required.")]

        public string Author { get; set; }
        [Required(ErrorMessage = "Gender is required.")]
        [RegularExpression("Male|Female|Unisex", ErrorMessage = "Gender must be Male, Female, or Unisex.")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
     
        public string description { get; set; }
        [Required(ErrorMessage = "Top notes are required.")]
        [StringLength(200, ErrorMessage = "Top notes cannot exceed 200 characters.")]
        public string TopNotes { get; set; }

        [Required(ErrorMessage = "Middle notes are required.")]
        [StringLength(200, ErrorMessage = "Middle notes cannot exceed 200 characters.")]
        public string MiddleNotes { get; set; }
        [Required(ErrorMessage = "Base notes are required.")]
        [StringLength(200, ErrorMessage = "Base notes cannot exceed 200 characters.")]
        public string BaseNotes { get; set; }    

        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Price from 1-50")]
        [Range(1, 1000)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price for 50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }

        [Required]
        [Display(Name = "Price for 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }

        [ValidateNever]
        
        public string ImgUrl { get; set; }

        [Required]
        [ValidateNever]
        public int Size30 { get; set; }=30;
        [Required]
        [ValidateNever]
        public int Size50 { get; set; } = 50;
        [Required]
        [ValidateNever]
        public int Size100 { get; set; }= 100;
        [Required]
        public double Rating { get; set; }
        [Required]

        public int Quantity { get; set; } = 99;
    }

}
