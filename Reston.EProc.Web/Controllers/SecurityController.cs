using Reston.EProc.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Reston.EProc.Web.Controllers
{
    //[RoutePrefix("api/security")]
    public class SecurityController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetCsrfToken()
        {
            // Generate token menggunakan CsrfHelper
            var token = CsrfHelper.GenerateToken();

            // 🔒 PERBAIKAN: Tambahkan SameSite=Strict untuk mencegah CSRF
            var cookie = new HttpCookie("XSRF-TOKEN", token)
            {
                HttpOnly = false,  // Harus false agar JavaScript bisa baca (untuk kirim via header)
                Secure = true,     // 🔒 WAJIB HTTPS
                SameSite = SameSiteMode.Strict,  // 🔒 PERBAIKAN: Tambahkan SameSite
                Expires = DateTime.Now.AddMinutes(30),
                Path = "/"
            };

            HttpContext.Current.Response.Cookies.Add(cookie);
            
            return Ok(new { csrfToken = token });
        }
    }
}
