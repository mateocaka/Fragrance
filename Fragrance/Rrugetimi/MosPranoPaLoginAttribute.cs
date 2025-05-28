using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fragrance.Rrugetimi
{
    public class MosPranoPaLoginAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var requestPath = context.HttpContext.Request.Path;
        
            if (requestPath.StartsWithSegments("/Identity/Account/Login"))
            {
                return; 
            }

          if(context.HttpContext.User.Identity.IsAuthenticated)
            {
            
                return;
            }
            else
            {
                
                string returnUrl = context.HttpContext.Request.Path;
                context.Result = new RedirectToActionResult("Login", "Account",
                    new { area = "Identity", returnUrl = returnUrl });
            }
        }
    }
}