using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Runner.IIS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // 🔒 KONFIGURASI: Mapping file HTML ke roles yang diizinkan
        private static readonly Dictionary<string, string[]> FileRoleMapping = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            // ========================================
            // ADMIN PAGES - Hanya untuk admin roles
            // ========================================
            { "/master.html", new[] { "eproc_superadmin", "eproc_admin" } },
            { "/master-penawaran.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager" } },
            { "/anggaran.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager" } },
            { "/budgeting.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager" } },
            { "/daftar-hitam.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head" } },
            { "/kebijakan-pendaftaran.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head" } },
            { "/edit-kebijakan.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head" } },
            { "/regulasi.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head" } },
            
            // ========================================
            // PROCUREMENT PAGES - Untuk procurement staff & above
            // ========================================
            { "/pengadaan-add.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/pengadaan_add_terbuka.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/pengadaan-detail.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/pengadaan-detail-edit.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/create-rks.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/create-pks.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/create-spk.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/create-spk-non-pks.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/create-po.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            
            // ========================================
            // VENDOR MANAGEMENT - Untuk procurement & admin
            // ========================================
            { "/rekanan-verifikasi.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/rekanan-verifikasi-v2.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/rekanan-sanksi.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager" } },
            { "/rekanan-sanksi-add.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager" } },
            { "/vendor-list.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            
            // ========================================
            // CATALOG MANAGEMENT - Untuk admin & procurement
            // ========================================
            { "/add-eCatalogue2.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager" } },
            { "/add-eCatalogue-Asuransi.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager" } },
            { "/edit-eCatalogue.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager" } },
            
            // ========================================
            // APPROVAL PAGES - Untuk approvers
            // ========================================
            { "/approval.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "end_user" } },
            { "/approval-detail.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "end_user" } },
            { "/approval-perencanaan-belum-disetujui.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "end_user" } },
            { "/approval-perencanaan-sudah-disetujui.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "end_user" } },
            
            // ========================================
            // EMEMO & ENOTA - Untuk internal users
            // ========================================
            { "/ememo-create.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff", "end_user" } },
            { "/ememo-list.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff", "end_user" } },
            { "/ememo-approval-list.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "end_user" } },
            { "/enota-create.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff", "end_user" } },
            { "/enota-list.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff", "end_user" } },
            { "/enota-approval-list.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "end_user" } },
            
            // ========================================
            // REPORT PAGES - Untuk admin & procurement
            // ========================================
            { "/report.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/report_penilaian_vendor.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager" } },
            
            // ========================================
            // MONITORING - Untuk admin & procurement
            // ========================================
            { "/sistem-monitoring.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/sistem-monitoring-detail.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            { "/monitoring-proyek-add.html", new[] { "eproc_superadmin", "eproc_admin", "procurement_head", "procurement_manager", "procurement_staff" } },
            
            // ========================================
            // VENDOR PAGES - Hanya untuk vendor
            // ========================================
            { "/menu-vendor.html", new[] { "rekanan_terdaftar" } },
            { "/vendor-registration.html", new[] { "rekanan_terdaftar" } },
            { "/vendor-registration-edit.html", new[] { "rekanan_terdaftar" } }
        };

        protected void Application_Start()
        {
            // 🔍 DEBUG: Log application start
            System.Diagnostics.Debug.WriteLine("========================================");
            System.Diagnostics.Debug.WriteLine("[APP] Application_Start called");
            System.Diagnostics.Debug.WriteLine("[APP] Authorization module initialized");
            System.Diagnostics.Debug.WriteLine($"[APP] Protected files count: {FileRoleMapping.Count}");
            System.Diagnostics.Debug.WriteLine("========================================");
        }

        /// <summary>
        /// 🔒 PERBAIKAN KEAMANAN: Authorization check untuk file HTML statis
        /// Dipanggil untuk setiap HTTP request
        /// </summary>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                // Get current request path
                string requestPath = Request.Url.AbsolutePath;
                
                // 🔍 DEBUG: Log EVERY request (untuk verify event berjalan)
                System.Diagnostics.Debug.WriteLine($"[REQUEST] {Request.HttpMethod} {requestPath}");

                // Skip non-HTML files untuk performance
                if (!requestPath.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                // 🔒 Check if file needs authorization
                if (FileRoleMapping.ContainsKey(requestPath))
                {
                    // Get required roles for this file
                    string[] requiredRoles = FileRoleMapping[requestPath];

                    // 🔍 DEBUG: Log request info
                    System.Diagnostics.Debug.WriteLine("========================================");
                    System.Diagnostics.Debug.WriteLine($"[AUTH] Protected file requested: {requestPath}");
                    System.Diagnostics.Debug.WriteLine($"[AUTH] User object: {(User == null ? "NULL" : "EXISTS")}");
                    System.Diagnostics.Debug.WriteLine($"[AUTH] User.Identity: {(User?.Identity == null ? "NULL" : "EXISTS")}");
                    System.Diagnostics.Debug.WriteLine($"[AUTH] User authenticated: {User?.Identity?.IsAuthenticated}");
                    System.Diagnostics.Debug.WriteLine($"[AUTH] User name: {User?.Identity?.Name ?? "NULL"}");

                    // Check if user is authenticated
                    if (User == null || User.Identity == null || !User.Identity.IsAuthenticated)
                    {
                        // ❌ User not authenticated - Redirect to login
                        System.Diagnostics.Debug.WriteLine($"[AUTH] ❌ BLOCKED: User not authenticated");
                        System.Diagnostics.Debug.WriteLine("========================================");
                        
                        Response.Clear();
                        Response.StatusCode = 401;
                        Response.StatusDescription = "Unauthorized";
                        Response.Redirect("/index.html?returnUrl=" + Server.UrlEncode(requestPath), true);
                        return;
                    }

                    // Check if user has required role
                    bool hasRequiredRole = false;
                    System.Diagnostics.Debug.WriteLine($"[AUTH] Required roles: {string.Join(", ", requiredRoles)}");
                    
                    // Get user's claims/roles
                    var identity = User.Identity as System.Security.Claims.ClaimsIdentity;
                    if (identity != null)
                    {
                        // Log all claims for debugging
                        System.Diagnostics.Debug.WriteLine($"[AUTH] Total claims: {identity.Claims.Count()}");
                        foreach (var claim in identity.Claims)
                        {
                            System.Diagnostics.Debug.WriteLine($"[AUTH]   Claim: {claim.Type} = {claim.Value}");
                        }
                        
                        var userRoles = identity.Claims
                            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role || 
                                       c.Type == "role" || 
                                       c.Type == "roles")
                            .Select(c => c.Value)
                            .ToList();
                        
                        System.Diagnostics.Debug.WriteLine($"[AUTH] User roles from claims: {string.Join(", ", userRoles)}");
                        
                        // Check if user has any of the required roles
                        foreach (var role in requiredRoles)
                        {
                            if (userRoles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase)))
                            {
                                hasRequiredRole = true;
                                System.Diagnostics.Debug.WriteLine($"[AUTH] ✅ MATCH: User has role '{role}'");
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Fallback to User.IsInRole() for non-claims based auth
                        System.Diagnostics.Debug.WriteLine($"[AUTH] Using User.IsInRole() fallback");
                        foreach (var role in requiredRoles)
                        {
                            if (User.IsInRole(role))
                            {
                                hasRequiredRole = true;
                                System.Diagnostics.Debug.WriteLine($"[AUTH] ✅ MATCH: User has role '{role}' (via IsInRole)");
                                break;
                            }
                        }
                    }

                    if (!hasRequiredRole)
                    {
                        // ❌ User doesn't have required role - Access Denied
                        System.Diagnostics.Debug.WriteLine($"[AUTH] ❌ BLOCKED: User does not have required role");
                        System.Diagnostics.Debug.WriteLine("========================================");
                        
                        Response.Clear();
                        Response.StatusCode = 403;
                        Response.StatusDescription = "Forbidden";
                        Response.ContentType = "text/html; charset=utf-8";
                        Response.Write(@"
<!DOCTYPE html>
<html>
<head>
    <title>Access Denied</title>
    <meta charset='utf-8'>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }
        .error-container {
            background: white;
            padding: 40px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            text-align: center;
            max-width: 500px;
        }
        .error-code {
            font-size: 72px;
            font-weight: bold;
            color: #dc3545;
            margin: 0;
        }
        .error-title {
            font-size: 24px;
            color: #333;
            margin: 20px 0 10px 0;
        }
        .error-message {
            color: #666;
            margin: 10px 0 30px 0;
            line-height: 1.6;
        }
        .btn {
            display: inline-block;
            padding: 10px 20px;
            background-color: #007bff;
            color: white;
            text-decoration: none;
            border-radius: 4px;
            transition: background-color 0.3s;
        }
        .btn:hover {
            background-color: #0056b3;
        }
    </style>
</head>
<body>
    <div class='error-container'>
        <h1 class='error-code'>403</h1>
        <h2 class='error-title'>Access Denied</h2>
        <p class='error-message'>
            Anda tidak memiliki akses ke halaman ini.<br>
            Silakan hubungi administrator jika Anda merasa ini adalah kesalahan.
        </p>
        <a href='/View/dashboard.html' class='btn'>Kembali ke Dashboard</a>
    </div>
</body>
</html>");
                        Response.End();
                        return;
                    }

                    // ✅ User has required role - Allow access
                    System.Diagnostics.Debug.WriteLine($"[AUTH] ✅ ALLOWED: User has required role");
                    System.Diagnostics.Debug.WriteLine("========================================");
                }

                // File tidak memerlukan authorization atau user sudah authorized
                // Continue normal request processing
            }
            catch (System.Threading.ThreadAbortException)
            {
                // Response.End() throws ThreadAbortException, this is expected
                throw;
            }
            catch (Exception ex)
            {
                // Log error tapi jangan block request
                System.Diagnostics.Debug.WriteLine($"[AUTH ERROR] {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[AUTH ERROR] {ex.StackTrace}");
                // Continue normal request processing
            }
        }
    }

}
