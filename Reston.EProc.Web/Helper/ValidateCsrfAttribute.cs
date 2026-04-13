using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Reston.EProc.Web.Helper
{
    public class ValidateCsrfAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var path = request.RequestUri.AbsolutePath.ToLower();

            // Skip validation for certain paths
            if (!path.StartsWith("/api/"))
                return true;

            if (path.Contains("/identity") ||
                path.Contains("/connect") ||
                path.Contains("/signin") ||
                path.Contains("/login") ||
                path.Contains("/list") ||
                path.Contains("/api/header/ceklogin") ||
                path.Contains("/api/header/cekrole") ||
                path.Contains("/api/header/signout") ||
                path.Contains("/api/security/csrf-token"))  // TAMBAHKAN INI
            {
                return true;
            }

            if (request.Method == HttpMethod.Get)
                return true;

            // Check both header and cookie
            string token = null;

            // 1. Check header first
            if (request.Headers.Contains("X-CSRF-TOKEN"))
            {
                token = request.Headers.GetValues("X-CSRF-TOKEN").FirstOrDefault();
            }
            // 2. Fallback to cookie
            else if (request.Headers.TryGetValues("Cookie", out var cookieValues))
            {
                var cookies = cookieValues.FirstOrDefault();
                if (cookies != null)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(
                        cookies, "XSRF-TOKEN=([^;]+)");
                    if (match.Success)
                    {
                        token = match.Groups[1].Value;
                    }
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("CSRF token missing")
                };
                return false;
            }

            return CsrfHelper.ValidateToken(token);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            // Return 403 instead of 401 to prevent redirect to login
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Invalid or missing CSRF token")
            };
        }
    }
}
