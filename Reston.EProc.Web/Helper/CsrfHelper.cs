using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Reston.EProc.Web.Helper
{
    public static class CsrfHelper
    {
        private static readonly ConcurrentDictionary<string, DateTime> Tokens = new ConcurrentDictionary<string, DateTime>();

        private static readonly object _lock = new object();

        public static string GenerateToken()
        {
            var token = Guid.NewGuid().ToString();
            var expiry = DateTime.Now.AddMinutes(30);

            lock (_lock)
            {
                // Clean expired tokens
                var expiredTokens = Tokens.Where(kvp => kvp.Value < DateTime.Now)
                                         .Select(kvp => kvp.Key)
                                         .ToList();
                foreach (var expired in expiredTokens)
                {
                    Tokens.TryRemove(expired, out _);
                }

                Tokens[token] = expiry;
            }

            return token;
        }

        public static bool ValidateToken(string token)
        {
            lock (_lock)
            {
                if (Tokens.TryGetValue(token, out DateTime expiry))
                {
                    if (expiry >= DateTime.Now)
                    {
                        // Remove after use (optional)
                        //Tokens.TryRemove(token, out _);
                        return true;
                    }
                    else
                    {
                        // Remove expired token
                        Tokens.TryRemove(token, out _);
                    }
                }
                return false;
            }
        }
    }
}
