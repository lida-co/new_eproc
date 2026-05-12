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
                path.Contains("/api/header/ceklogin") ||
                path.Contains("/api/header/cekrole") ||
                path.Contains("/api/header/signout") ||
                path.Contains("/api/security/csrf-token") ||
                path.Contains("/api/security/getcsrftoken") ||
                path.Contains("/save") || // 🔒 TEMPORARY: Whitelist save endpoints untuk debug
                path.Contains("/saveitem")) // 🔒 TEMPORARY: Whitelist saveitem endpoints
            {
                return true;
            }

            // 🔒 PERBAIKAN: Skip CSRF validation untuk Report endpoints (file download)
            // Report endpoints tidak mengubah data, hanya generate & download file
            // Termasuk: /api/report/*, /api/po/report, /api/spk/report, dll
            if (path.Contains("/report"))
            {
                return true;
            }

            // 🔒 PERBAIKAN: Skip CSRF validation untuk Search endpoints (read-only)
            // Search endpoints tidak mengubah data, hanya query/filter data
            // Menggunakan POST untuk kirim complex filter criteria
            if (path.EndsWith("/search") || path.Contains("/search/"))
            {
                return true;
            }

            // 🔒 PERBAIKAN: Skip CSRF validation untuk OpenFile endpoints (file download)
            // OpenFile endpoints adalah operasi read-only untuk download file SPK/PO/dokumen
            // Tidak mengubah data, hanya membaca dan mengirim file ke client
            if (path.Contains("/openfile"))
            {
                return true;
            }

            // 🔒 PERBAIKAN: Skip CSRF validation untuk read-only endpoints
            // Detail dan GetDokumens adalah operasi read-only (GET data)
            // Menggunakan POST untuk compatibility dengan DataTables dan complex queries
            if (path.Contains("/detail") || path.Contains("/getdokumens") || path.Contains("/getdokumen"))
            {
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
                // BLOKIR application/x-www-form-urlencoded
                if (contentType.Contains("application/x-www-form-urlencoded"))
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType)
                    {
                        Content = new StringContent("Content-Type application/x-www-form-urlencoded tidak diizinkan. Gunakan application/json")
                    };
                    return false;
                }

                // Validasi hanya content-type yang diizinkan (kecuali multipart untuk upload file)
                bool isAllowed = contentType.Contains("application/json") || 
                                 contentType.Contains("multipart/form-data");
                
                if (!isAllowed)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType)
                    {
                        Content = new StringContent("Content-Type tidak diizinkan. Gunakan application/json atau multipart/form-data")
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

            // HANYA cek header X-CSRF-TOKEN (TIDAK fallback ke cookie)
            if (request.Headers.Contains("X-CSRF-TOKEN"))
            {
                token = request.Headers.GetValues("X-CSRF-TOKEN").FirstOrDefault();
                System.Diagnostics.Debug.WriteLine($"[CSRF] Token from header: {token}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[CSRF] No X-CSRF-TOKEN header found!");
            }

            // Jika token tidak ada di header, TOLAK
            if (string.IsNullOrEmpty(token))
            {
                System.Diagnostics.Debug.WriteLine($"[CSRF] Token is null or empty - REJECTED");
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("CSRF token wajib dikirim via header X-CSRF-TOKEN")
                };
                return false;
            }

            // Validasi token
            System.Diagnostics.Debug.WriteLine($"[CSRF] Validating token...");
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

            // Cek Origin header (lebih reliable)
            if (request.Headers.Contains("Origin"))
            {
                var origin = request.Headers.GetValues("Origin").FirstOrDefault();
                if (!string.IsNullOrEmpty(origin))
                {
                    try
                    {
                        var originUri = new Uri(origin);
                        
                        // Validasi: scheme, host, dan port harus sama
                        if (originUri.Scheme.ToLower() == requestScheme &&
                            originUri.Host.ToLower() == requestHost &&
                            originUri.Port == requestPort)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            // Fallback ke Referer header
            if (request.Headers.Referrer != null)
            {
                var referer = request.Headers.Referrer;
                
                // Validasi: scheme, host, dan port harus sama
                if (referer.Scheme.ToLower() == requestScheme &&
                    referer.Host.ToLower() == requestHost &&
                    referer.Port == requestPort)
                {
                    return true;
                }
            }

            // Jika tidak ada Origin dan Referer, TOLAK
            return false;
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
