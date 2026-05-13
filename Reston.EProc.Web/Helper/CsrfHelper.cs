using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace Reston.EProc.Web.Helper
{
    /// <summary>
    /// CSRF Helper menggunakan in-memory token store.
    /// Simple dan reliable - token disimpan di memory server.
    /// </summary>
    public static class CsrfHelper
    {
        private static readonly ConcurrentDictionary<string, DateTime> _tokens 
            = new ConcurrentDictionary<string, DateTime>();

        private static readonly object _lock = new object();

        public static string GenerateToken()
        {
            var token = Guid.NewGuid().ToString("N"); // 32 hex chars, no dashes, no special chars
            var expiry = DateTime.UtcNow.AddMinutes(120); // 2 jam

            lock (_lock)
            {
                // Bersihkan token expired
                var expired = _tokens.Where(x => x.Value < DateTime.UtcNow)
                                     .Select(x => x.Key).ToList();
                foreach (var key in expired)
                    _tokens.TryRemove(key, out _);

                _tokens[token] = expiry;
            }

            System.Diagnostics.Debug.WriteLine($"[CSRF] Generated token: {token}, total tokens: {_tokens.Count}");
            return token;
        }

        public static bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;

            lock (_lock)
            {
                System.Diagnostics.Debug.WriteLine($"[CSRF] Validating: {token}, total tokens: {_tokens.Count}");

                if (_tokens.TryGetValue(token, out DateTime expiry))
                {
                    if (expiry >= DateTime.UtcNow)
                    {
                        System.Diagnostics.Debug.WriteLine($"[CSRF] Valid!");
                        return true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[CSRF] Expired!");
                        _tokens.TryRemove(token, out _);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[CSRF] Not found in memory!");
                }
                return false;
            }
        }
    }
}
