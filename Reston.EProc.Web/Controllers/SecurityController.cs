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
            // GANTI: Gunakan CsrfHelper.GenerateToken() bukan Guid.NewGuid()
            var token = CsrfHelper.GenerateToken();  // <- INI YANG BENAR

            var cookie = new HttpCookie("XSRF-TOKEN", token)
            {
                HttpOnly = false,
                Expires = DateTime.Now.AddMinutes(30)
            };

            HttpContext.Current.Response.Cookies.Add(cookie);
            return Ok(new { csrfToken = token });
        }
    }
}
