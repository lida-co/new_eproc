using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdLdap.Controllers
{
    [AllowAnonymous]
    public class HandleExceptionController : Controller
    {
        // GET: HandleException
        public ActionResult Index()
        {
            // 🔒 PERBAIKAN XSS: Default message aman
            string safeMessage = "SOMETHING WENT WRONG.";
            
            if (TempData["RedirectReason"] != null)
            {
                string rawMessage = TempData["RedirectReason"].ToString();
                
                // 🔒 PERBAIKAN XSS: Sanitasi dan encode message
                if (!string.IsNullOrEmpty(rawMessage))
                {
                    // Validasi panjang
                    if (rawMessage.Length > 500)
                    {
                        rawMessage = rawMessage.Substring(0, 500);
                    }
                    
                    // Encode HTML untuk mencegah XSS
                    safeMessage = HttpUtility.HtmlEncode(rawMessage);
                }
            }
            
            ViewBag.RedirectReason = safeMessage;

            return View();
        }
    }
}
