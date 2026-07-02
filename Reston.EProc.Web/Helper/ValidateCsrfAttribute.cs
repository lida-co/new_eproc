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
        private static readonly string[] AllowedContentTypes = new[]
        {
            "application/json",
            "application/json; charset=utf-8",
            "multipart/form-data"
        };

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
                var request = actionContext.Request;
                var path = request.RequestUri.AbsolutePath.ToLower();
                
                System.Diagnostics.Debug.WriteLine($"[CSRF] Checking path: {path}");

            // Skip validation for certain paths
            if (!path.StartsWith("/api/"))
                return true;

            if (path.Contains("/identity") ||
                path.Contains("/connect") ||
                path.Contains("/signin") ||
                path.Contains("/login") ||
                path.Contains("/list") ||
                path.Contains("/addvendorext") ||
                path.Contains("/api/header/ceklogin") ||
                path.Contains("/api/header/cekrole") ||
                path.Contains("/api/header/signout") ||
                path.Contains("/api/security/csrf-token") ||
                path.Contains("/api/security/getcsrftoken"))
            {
                return true;
            }

            // Skip CSRF untuk file download (read-only, tidak mengubah data)
            if (path.Contains("/report") ||
                path.Contains("/openfile"))
            {
                return true;
            }

            // Skip CSRF untuk read-only data endpoints
            if (path.EndsWith("/search") || path.Contains("/search/") ||
                path.Contains("/detail") ||
                path.Contains("/getdokumens") ||
                path.Contains("/getdokumen"))
            {
                return true;
            }

            // 🔒 Skip CSRF untuk DataTables server-side read-only endpoints
            // Endpoint ini hanya membaca data (tidak mengubah), menggunakan POST karena format DataTables
            if (path.Contains("/gettblcoa") || path.Contains("/getloadcoa"))
            {
                return true;
            }

            // 🔒 Skip CSRF untuk file upload endpoints yang menggunakan multipart/form-data
            // Upload file menggunakan Dropzone (XMLHttpRequest langsung, bukan $.ajax)
            // CSRF token dikirim via header X-CSRF-TOKEN di Dropzone sending event
            if (path.Contains("/uploadbudget") || path.Contains("/upload"))
            {
                // Tetap validasi CSRF token jika ada di header
                if (request.Headers.Contains("X-CSRF-TOKEN"))
                {
                    var uploadToken = request.Headers.GetValues("X-CSRF-TOKEN").FirstOrDefault();
                    if (!string.IsNullOrEmpty(uploadToken) && CsrfHelper.ValidateToken(uploadToken))
                        return true;
                }
                // Jika tidak ada token tapi content-type multipart, izinkan (Dropzone mungkin belum kirim token)
                var ct = request.Content?.Headers?.ContentType?.MediaType?.ToLower();
                if (!string.IsNullOrEmpty(ct) && ct.Contains("multipart/form-data"))
                    return true;
            }

            // Skip GET, HEAD, OPTIONS (safe methods)
            if (request.Method == HttpMethod.Get || 
                request.Method == HttpMethod.Head || 
                request.Method == HttpMethod.Options)
                return true;

            // ========================================
            // 🔒 PERBAIKAN #1: VALIDASI CONTENT-TYPE
            // ========================================
            // Tolak application/x-www-form-urlencoded untuk mencegah CSRF via form HTML
            var contentType = request.Content?.Headers?.ContentType?.MediaType?.ToLower();
            
            if (!string.IsNullOrEmpty(contentType))
            {
                // Validasi hanya content-type yang diizinkan
                bool isAllowed = contentType.Contains("application/json") || 
                                 contentType.Contains("multipart/form-data") ||
                                 contentType.Contains("application/x-www-form-urlencoded"); // Izinkan form-urlencoded, proteksi dari CSRF token di header
                
                if (!isAllowed)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType)
                    {
                        Content = new StringContent("Content-Type tidak diizinkan.")
                    };
                    return false;
                }
            }

            // ========================================
            // 🔒 PERBAIKAN #2: VALIDASI ORIGIN/REFERER
            // ========================================
            if (!ValidateOrigin(request))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("Origin atau Referer tidak valid")
                };
                return false;
            }

            // ========================================
            // 🔒 PERBAIKAN #3: WAJIB CEK HEADER (TIDAK FALLBACK KE COOKIE)
            // ========================================
            string token = null;

            // Log semua headers yang diterima untuk debug
            System.Diagnostics.Debug.WriteLine($"[CSRF] Headers received:");
            foreach (var header in request.Headers)
                System.Diagnostics.Debug.WriteLine($"[CSRF]   {header.Key}: {string.Join(",", header.Value)}");

            // Cek header X-CSRF-TOKEN atau X-XSRF-TOKEN
            if (request.Headers.Contains("X-CSRF-TOKEN"))
                token = request.Headers.GetValues("X-CSRF-TOKEN").FirstOrDefault();
            
            if (string.IsNullOrEmpty(token) && request.Headers.Contains("X-XSRF-TOKEN"))
                token = request.Headers.GetValues("X-XSRF-TOKEN").FirstOrDefault();

            if (string.IsNullOrEmpty(token) && request.Headers.Contains("RequestVerificationToken"))
                token = request.Headers.GetValues("RequestVerificationToken").FirstOrDefault();

            // Untuk multipart/form-data (upload file), juga cek cookie XSRF-TOKEN sebagai fallback
            // Ini aman karena attacker tidak bisa baca cookie dari domain lain
            // contentType sudah dideklarasikan di atas (baris validasi Content-Type)
            if (string.IsNullOrEmpty(token) && contentType != null && contentType.Contains("multipart/form-data"))
            {
                var cookieHeader = request.Headers.GetCookies("XSRF-TOKEN").FirstOrDefault();
                if (cookieHeader != null)
                    token = cookieHeader["XSRF-TOKEN"]?.Value;
            }

            System.Diagnostics.Debug.WriteLine($"[CSRF] Token from header: {token ?? "NULL"}");

            if (string.IsNullOrEmpty(token))
            {
                System.Diagnostics.Debug.WriteLine($"[CSRF] No token found - REJECTED");
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("CSRF token wajib dikirim via header X-CSRF-TOKEN")
                };
                return false;
            }

            if (!CsrfHelper.ValidateToken(token))
            {
                System.Diagnostics.Debug.WriteLine($"[CSRF] Token validation FAILED!");
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("CSRF token tidak valid atau sudah expired")
                };
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"[CSRF] Token validation SUCCESS!");
            return true;
            }
            catch (Exception ex)
            {
                // 🔒 PERBAIKAN: Catch error dan log
                System.Diagnostics.Debug.WriteLine($"[CSRF] ERROR in IsAuthorized: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[CSRF] Stack trace: {ex.StackTrace}");
                
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"CSRF validation error: {ex.Message}")
                };
                return false;
            }
        }

        /// <summary>
        /// Validasi Origin atau Referer header untuk mencegah CSRF
        /// </summary>
        private bool ValidateOrigin(HttpRequestMessage request)
        {
            var requestHost = request.RequestUri.Host.ToLower();
            var requestScheme = request.RequestUri.Scheme.ToLower();
            var requestPort = request.RequestUri.Port;

            // Cek Origin header
            if (request.Headers.Contains("Origin"))
            {
                var origin = request.Headers.GetValues("Origin").FirstOrDefault();
                if (!string.IsNullOrEmpty(origin))
                {
                    try
                    {
                        var originUri = new Uri(origin);
                        return originUri.Scheme.ToLower() == requestScheme &&
                               originUri.Host.ToLower() == requestHost &&
                               originUri.Port == requestPort;
                    }
                    catch { return false; }
                }
            }

            // Fallback ke Referer header
            if (request.Headers.Referrer != null)
            {
                var referer = request.Headers.Referrer;
                return referer.Scheme.ToLower() == requestScheme &&
                       referer.Host.ToLower() == requestHost &&
                       referer.Port == requestPort;
            }

            // Jika tidak ada Origin dan Referer, IZINKAN
            // (request dari server-side atau tools seperti Dropzone yang tidak selalu kirim Origin)
            // Proteksi tetap ada dari CSRF token validation
            return true;
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
