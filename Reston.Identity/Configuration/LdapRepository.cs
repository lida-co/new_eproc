using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Reston.Identity.Helper;

namespace IdLdap.Configuration
{
    public class LdapRepository : ILdapRepository, IDisposable
    {

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        PrincipalContext _AuthLdapConnect;

        public LdapRepository(PrincipalContext AuthLdapConnect)
        {
            _AuthLdapConnect = AuthLdapConnect;
        }


        public UserPrincipal FindUser(string searchterm)
        {
            _log.Debug("FindUser -> {UserName}", _AuthLdapConnect.UserName);
            try
            {
                var userprincipl = UserPrincipal.FindByIdentity(_AuthLdapConnect, IdentityType.SamAccountName, searchterm);
                if (userprincipl != null)
                {
                    _log.Debug("FindUser - userprincipl.SamAccountName = {SamAccountName}", userprincipl.SamAccountName);
                }
                else
                {
                    _log.Debug("FindUser - userPrincipal not found! Searched using IdentityType.SamAccountName, search term {SearchTerm} ", searchterm);
                }
                return userprincipl;
            }
            catch (Exception ex)
            {
                _log.Debug(ex, "FindUser - Problem Occured. See stack trace for details");
                return null;
            }
        }


        public UserPrincipal FindUser2(string searchterm)
        {
            _log.Debug("FindUser2 -> {UserName}", _AuthLdapConnect.UserName);
            try
            {
                var userprincipl = UserPrincipal.FindByIdentity(_AuthLdapConnect, IdentityType.UserPrincipalName, searchterm);
                if (userprincipl != null)
                {
                    _log.Debug("FindUser2 - userprincipl.UserPrincipalName = {UserPrincipalName}", userprincipl.UserPrincipalName);
                }
                else
                {
                    _log.Debug("FindUser2 - userPrincipal not found! Searched using IdentityType.UserPrincipalName, search term {SearchTerm} ", searchterm);
                }
                return userprincipl;
            }
            catch (Exception ex)
            {
                _log.Debug(ex, "FindUser2 - Problem Occured. See stack trace for details");
                return null;
            }
        }

        public GroupPrincipal FindGroup(string searchterm)
        {
            return GroupPrincipal.FindByIdentity(_AuthLdapConnect, searchterm);
        }

        public bool ValidateCredentials(string username, string password)
        {
            _log.Debug("ValidateCredentials - Username: {UserName}, Password: ***", username);

            var authenticated = false;
            var message = "";
            try
            {
                authenticated = _AuthLdapConnect.ValidateCredentials(username, password, ContextOptions.SimpleBind);
                if (!authenticated)
                {
                    authenticated = _AuthLdapConnect.ValidateCredentials(username, password, ContextOptions.Negotiate);

                    if (!authenticated)

                        throw new AuthenticationException();
                }
            }
            catch (Exception ex)
            {
                _log.Debug(ex, "ValidateCredentials - Problem Occured. See stack trace for details");
                message = ex.ToString();
            }
            _log.Debug("ValidateCredentials - authentication result Username: {Username}, Authenticated: {Authenticated}, Authentication Message: {AuthMessage}", username, authenticated, message);
            return authenticated;
        }

        public IdLdap.Models.GridUserItem GetUsersByUsername(string searchterm, int page = 0, int limit = int.MaxValue)
        {
            var normalizedTerm = (searchterm ?? string.Empty).Trim();
            var wildcardChars = new[] { '*', '%' };
            normalizedTerm = normalizedTerm.Trim(wildcardChars).Trim();

            var users = new List<UserPrincipal>();
            if (string.IsNullOrWhiteSpace(normalizedTerm))
            {
                // Saat filter kosong, kembalikan seluruh user agar halaman Link LDAP tidak terlihat kosong.
                users.AddRange(FindAllUsers());
            }
            else
            {
                users.AddRange(FindUsersByIdentity(IdentityType.UserPrincipalName, normalizedTerm));
                users.AddRange(FindUsersByIdentity(IdentityType.SamAccountName, normalizedTerm));
                users.AddRange(FindUsersByIdentity(IdentityType.Name, normalizedTerm));

                // Beberapa directory provider hanya match saat wildcard di-set eksplisit.
                var wildcardTerm = "*" + normalizedTerm + "*";
                users.AddRange(FindUsersByIdentity(IdentityType.UserPrincipalName, wildcardTerm));
                users.AddRange(FindUsersByIdentity(IdentityType.SamAccountName, wildcardTerm));
                users.AddRange(FindUsersByIdentity(IdentityType.Name, wildcardTerm));
            }

            var dedupedUsers = users
                .Where(x => x != null)
                .GroupBy(x => x.Guid.HasValue ? x.Guid.Value.ToString() : (x.DistinguishedName ?? x.Sid?.ToString() ?? x.Name ?? string.Empty), StringComparer.OrdinalIgnoreCase)
                .Select(x => x.First())
                .ToList();

            if (!dedupedUsers.Any())
            {
                _log.Warn("GetUsersByUsername - UserPrincipal query returned 0 result. Fallback to DirectorySearcher. SearchTerm: {SearchTerm}", normalizedTerm);
                var fallbackUsers = SearchUsersWithDirectorySearcher(normalizedTerm);
                dedupedUsers = fallbackUsers
                    .Where(x => x != null)
                    .GroupBy(x => x.Guid.HasValue ? x.Guid.Value.ToString() : (x.DistinguishedName ?? x.Sid?.ToString() ?? x.Name ?? string.Empty), StringComparer.OrdinalIgnoreCase)
                    .Select(x => x.First())
                    .ToList();
                _log.Info("GetUsersByUsername - DirectorySearcher fallback found {UserCount} users.", dedupedUsers.Count);
            }

            var total = dedupedUsers.Count;
            var pageIndex = page < 0 ? 0 : page;
            var rowLimit = limit <= 0 ? int.MaxValue : limit;

            return new IdLdap.Models.GridUserItem()
            {
                Length = total,
                Users = dedupedUsers.Skip(pageIndex * rowLimit).Take(rowLimit)
            };
        }

        private IEnumerable<UserPrincipal> FindAllUsers()
        {
            try
            {
                using (var userSearch = new UserPrincipal(_AuthLdapConnect))
                using (var searcher = new PrincipalSearcher(userSearch))
                {
                    return searcher.FindAll().Select(principal => principal as UserPrincipal).Where(x => x != null).ToList();
                }
            }
            catch (Exception ex)
            {
                _log.Debug(ex, "GetUsersByUsername - failed querying all users.");
                return Enumerable.Empty<UserPrincipal>();
            }
        }

        private IEnumerable<UserPrincipal> FindUsersByIdentity(IdentityType identityType, string searchterm)
        {
            try
            {
                var userSearch = new UserPrincipal(_AuthLdapConnect);
                switch (identityType)
                {
                    case IdentityType.UserPrincipalName:
                        userSearch.UserPrincipalName = searchterm;
                        break;
                    case IdentityType.SamAccountName:
                        userSearch.SamAccountName = searchterm;
                        break;
                    case IdentityType.Name:
                        userSearch.Name = searchterm;
                        break;
                    default:
                        return Enumerable.Empty<UserPrincipal>();
                }

                using (var searcher = new PrincipalSearcher(userSearch))
                {
                    return searcher.FindAll().Select(principal => principal as UserPrincipal).Where(x => x != null).ToList();
                }
            }
            catch (Exception ex)
            {
                _log.Debug(ex, "GetUsersByUsername - failed querying identity type {IdentityType} with term {SearchTerm}", identityType, searchterm);
                return Enumerable.Empty<UserPrincipal>();
            }
        }

        private IEnumerable<UserPrincipal> SearchUsersWithDirectorySearcher(string searchterm)
        {
            try
            {
                var host = IdLdapConstants.LdapConfiguration.Host;
                var container = IdLdapConstants.LdapConfiguration.ContextNaming;
                var username = IdLdapConstants.LdapConfiguration.Username;
                var password = IdLdapConstants.LdapConfiguration.Password;

                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(container))
                {
                    _log.Warn("DirectorySearcher fallback skipped because LDAP host/container is empty.");
                    return Enumerable.Empty<UserPrincipal>();
                }

                var escapedTerm = EscapeLdapSearchFilter(searchterm);
                var filter = string.IsNullOrWhiteSpace(escapedTerm)
                    ? "(&(objectClass=user))"
                    : string.Format("(&(objectClass=user)(|(sAMAccountName=*{0}*)(userPrincipalName=*{0}*)(cn=*{0}*)(displayName=*{0}*)))", escapedTerm);

                var users = new List<UserPrincipal>();
                var ldapPath = "LDAP://" + host + "/" + container;
                var authType = AuthenticationTypes.None;
                if (IdLdapConstants.LdapConfiguration.UsingSSL)
                {
                    authType = AuthenticationTypes.SecureSocketsLayer;
                }
                using (var root = new DirectoryEntry(ldapPath, username, password, authType))
                using (var searcher = new DirectorySearcher(root))
                {
                    searcher.Filter = filter;
                    searcher.PageSize = 500;
                    searcher.PropertiesToLoad.Add("distinguishedName");

                    var results = searcher.FindAll();
                    foreach (SearchResult result in results)
                    {
                        var dn = result.Properties["distinguishedName"]?.Count > 0
                            ? result.Properties["distinguishedName"][0]?.ToString()
                            : null;
                        if (string.IsNullOrWhiteSpace(dn)) continue;

                        var principal = UserPrincipal.FindByIdentity(_AuthLdapConnect, IdentityType.DistinguishedName, dn);
                        if (principal != null)
                        {
                            users.Add(principal);
                        }
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "DirectorySearcher fallback failed.");
                return Enumerable.Empty<UserPrincipal>();
            }
        }

        private static string EscapeLdapSearchFilter(string value)
        {
            if (value == null) return string.Empty;
            return value
                .Replace("\\", "\\5c")
                .Replace("*", "\\2a")
                .Replace("(", "\\28")
                .Replace(")", "\\29")
                .Replace("\0", "\\00");
        }

        public PrincipalSearchResult<Principal> GetGroups(string searchterm)
        {
            GroupPrincipal userSearch =
                new GroupPrincipal(_AuthLdapConnect);
            userSearch.SamAccountName = searchterm;


            PrincipalSearcher searcher = new PrincipalSearcher();
            searcher.QueryFilter = userSearch;

            using (searcher)
            {
                PrincipalSearchResult<Principal> results =
                    searcher.FindAll();

                return results;
            }
        }


        public UserPrincipal GetUserByGuid(String guid)
        {
            var userPrincipal = UserPrincipal.FindByIdentity(_AuthLdapConnect, IdentityType.Guid, guid);
            return userPrincipal;
        }


        public void Dispose()
        {
            _AuthLdapConnect.Dispose();
        }





    }
}
