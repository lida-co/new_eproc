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
                else {
                    _log.Debug("FindUser - userPrincipal not found! Searched using IdentityType.SamAccountName, search term {SearchTerm} ", searchterm);
                }
                return userprincipl;
            }
            catch (Exception ex)
            {
                _log.Debug(ex, "FindUser - Problem Occured. See stack trace for details");
                return new UserPrincipal(_AuthLdapConnect);
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
                return new UserPrincipal(_AuthLdapConnect);
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

        public IdLdap.Models.GridUserItem GetUsersByUsername(string searchterm, int page = 0, int limit = int.MaxValue )
        {
            UserPrincipal userSearch =
                new UserPrincipal(_AuthLdapConnect);
            PrincipalSearcher searcher = new PrincipalSearcher();
            
            
            //userSearch.SamAccountName = searchterm;
            //userSearch.Name = searchterm;
            //userSearch.UserPrincipalName = searchterm;
            //userSearch.GivenName = searchterm;
            //userSearch.SamAccountName = searchterm;

            if (_AuthLdapConnect.ContextType == ContextType.Machine)
            {
                userSearch.SamAccountName = searchterm;
            }
            else
            {
                userSearch.UserPrincipalName = searchterm;
            }

            searcher.QueryFilter = userSearch;
            int ct = searcher.FindAll().Count();
            
            using (searcher)
            {
                try
                {
                    ((DirectorySearcher)searcher.GetUnderlyingSearcher()).SizeLimit = limit;
                    ((DirectorySearcher)searcher.GetUnderlyingSearcher()).PageSize = page;
                }
                catch
                { 
                }

                PrincipalSearchResult<Principal> results =
                    searcher.FindAll();

                IdLdap.Models.GridUserItem gr = new IdLdap.Models.GridUserItem()
                {
                    Length = ct,
                    Users = results.Skip(page * limit).Take(limit).Select(principal => principal as UserPrincipal)
                };
                return gr;
            }
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
