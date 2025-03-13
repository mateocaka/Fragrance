using Fragrance.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fragrance.Models.ViewModels
{
    public class ParfumeVM
    {
        public Parfume Parfum{ get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ParfumeList { get; set; }
        
    }
}
