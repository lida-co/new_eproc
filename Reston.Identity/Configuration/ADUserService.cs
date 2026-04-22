using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using Reston.Identity.Helper;
using Reston.Identity.Repository.Identity;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Extensions;
using IdentityModel;
using IdentityServer3.Core;
using System.Net.Http;
using Newtonsoft.Json;
using Reston.Helper;
using NLog;


namespace IdLdap.Configuration
{
    public class ADUserService : UserServiceBase
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ILdapRepository _LdapRepository;
        private readonly UserManager _UserManager;
        public bool EnableSecurityStamp { get; set; }

        public ADUserService()
        {
            _UserManager = new UserManager(new UserStore(new IdentityContext()));
            EnableSecurityStamp = true;
        }

        public ADUserService(ILdapRepository LdapRepository)
        {
            _LdapRepository = LdapRepository;
            _UserManager = new UserManager(new UserStore(new IdentityContext()));
            EnableSecurityStamp = true;
        }

        public int validasiCaptcha(Guid guid, string answer)
        {
            var client = new HttpClient();
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //HttpResponseMessage reply = await client.GetAsync(
            //       string.Format("{0}/{1}", IdLdapConstants.Proc.Url, "api/Registrasi/validasiCaptcha?guid="+guid+"&answer="+answer));

            //if (reply.IsSuccessStatusCode)
            //{
            //    string captca = await reply.Content.ReadAsStringAsync();
            //    var result = JsonConvert.DeserializeObject<int>(captca);

            //    return result;
            //}
            //else
            //{
            //    return 0;
            //}
            CaptchaHelper captcha = new CaptchaHelper();
            var validasi = captcha.Verify(guid, answer);
            if (validasi) return 1;
            else return 0;

        }

        public override async Task AuthenticateLocalAsync(IdentityServer3.Core.Models.LocalAuthenticationContext context)
        {
            try
            {
                var splitData = context.UserName.Split('#');
                if (splitData.Count() < 3)
                {
                    _log.Warn("AuthenticateLocalAsync: Format username tidak valid. Diterima: '{UserNameRaw}'. Format yang dibutuhkan: 'username#captcha#guid'", context.UserName);
                    context.AuthenticateResult = new AuthenticateResult("Username atau password tidak valid");
                    return;
                }
                var username = splitData[0];
                var answerCaptcha = splitData[1];
                Guid guid = new Guid(splitData[2]);

                if (IdLdapConstants.AppConfiguration.IsAntiBruteForceEnabled)
                {
                    int valid = validasiCaptcha(guid, answerCaptcha);
                    if (valid == 0)
                    {
                        // human verification code failed
                        _log.Debug("Anti brute force code verification failed. AntiBruteForceCodeId: {AntiBruteForceCodeId}, UserEntered: {AntiBruteForceUserInput}", guid, answerCaptcha);
                        context.AuthenticateResult = new AuthenticateResult("Please enter the correct verification code");
                        return;
                    }
                }

                // var username = context.UserName;
                var password = context.Password;
                var message = context.SignInMessage;

                context.AuthenticateResult = null;

                //System.IO.File.AppendAllText(path, Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine);

                //_log.Debug(path);

                //System.IO.File.AppendAllText(path, Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine);

                //bool UseAppDir = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["LDAP_APPDIR"]);

                //var userLdap = UseAppDir ? _LdapRepository.FindUser2(username) : _LdapRepository.FindUser(username);

                //var userLdap = _LdapRepository.FindUser2(username);
                //System.IO.File.AppendAllText(path, "cekuserldap " + userLdap.DisplayName + Environment.NewLine);
                // var userIdentity = await FindUserAsync(username);

                //System.IO.File.AppendAllText(path, "cekuserldap " + userIdentity.DisplayName + Environment.NewLine);
                //_log.Debug("cekuserldap " + userIdentity.DisplayName);
                var userAccount = await _UserManager.FindByNameAsync(username);
                if (userAccount == null && _LdapRepository != null)
                {
                    _log.Debug("Local account not found for '{UserName}'. Trying LDAP auto-link.", username);
                    userAccount = await TryAutoLinkLdapAccountAsync(username);
                    if (userAccount == null && ValidateLdapCredentialsWithFallback(username, username, password))
                    {
                        _log.Warn("LDAP lookup failed for '{UserName}', but credential bind succeeded. Creating minimal linked local account.", username);
                        userAccount = await CreateMinimalLdapLinkedAccountAsync(username);
                    }
                }
                if (userAccount != null)
                {
                    _log.Debug("Account found. Id: {AccountId}, UserName: {AccountUserName}", userAccount.Id, userAccount.UserName);
                    if (_UserManager.SupportsUserLockout && await _UserManager.IsLockedOutAsync(userAccount.Id))
                    {
                        _log.Debug("Account is locked. Id: {AccountId}, UserName: {AccountUserName}", userAccount.Id, userAccount.UserName);
                        context.AuthenticateResult = new AuthenticateResult("Account is locked. Please contact your system administrator");
                        return;
                    }

                    if (userAccount.IsLdapUser)
                    {
                        _log.Debug("Account is LDAP-linked. Using LDAP Store to verify password. Id: {AccountId}, UserName: {AccountUserName}", userAccount.Id, userAccount.UserName);
                        if (_LdapRepository == null)
                        {
                            _log.Warn("LDAP repository is null while trying to authenticate LDAP user '{UserName}'.", userAccount.UserName);
                            context.AuthenticateResult = new AuthenticateResult("The system is experiencing problem. Please contact system administrator");
                            return;
                        }

                        bool pwdIsValid = ValidateLdapCredentialsWithFallback(username, userAccount.UserName, password);
                        if (pwdIsValid)
                        {
                            _log.Debug("Password verification SUCCESSFUL. Id: {AccountId}, UserName: {AccountUserName}", userAccount.Id, userAccount.UserName);
                            if (_UserManager.SupportsUserLockout)
                            {
                                _log.Debug("Resetting failed login attempt count. Id: {AccountId}, UserName: {AccountUserName}", userAccount.Id, userAccount.UserName);
                                await _UserManager.ResetAccessFailedCountAsync(userAccount.Id);
                            }
                            var claims = await GetClaimsForAuthenticateResult(userAccount);
                            var result = new AuthenticateResult(userAccount.Id, userAccount.UserName, claims);
                            context.AuthenticateResult = result;
                            return;
                        }
                        else
                        {
                            _log.Debug("Password verification FAILED. Id: {AccountId}, UserName: {AccountUserName}", userAccount.Id, userAccount.UserName);
                            if (_UserManager.SupportsUserLockout)
                            {
                                _log.Debug("Incrementing failed login attempt count. Id: {AccountId}, UserName: {AccountUserName}", userAccount.Id, userAccount.UserName);
                                await _UserManager.AccessFailedAsync(userAccount.Id);
                            }
                            context.AuthenticateResult = new AuthenticateResult("Username or password is invalid");
                            return;
                        }
                        //context.AuthenticateResult = new AuthenticateResult("Account is LDAP-linked. LDAP authentication is not supported at this moment.");
                    }
                    else
                    {
                        _log.Debug("Account is a Local Account. Using Local Credential Store to verify password. Id: {AccountId}, UserName: {AccountUserName}", userAccount.Id, userAccount.UserName);
                        bool pwdIsValid = await _UserManager.CheckPasswordAsync(userAccount, password);
                        if (pwdIsValid)
                        {
                            _log.Debug("Password verification SUCCESSFUL. Id: {AccountId}, UserName: {AccountUserName}", userAccount.Id, userAccount.UserName);
                            if (_UserManager.SupportsUserLockout)
                            {
                                _log.Debug("Resetting failed login attempt count. Id: {AccountId}, UserName: {AccountUserName}", userAccount.Id, userAccount.UserName);
                                await _UserManager.ResetAccessFailedCountAsync(userAccount.Id);
                            }
                            var claims = await GetClaimsForAuthenticateResult(userAccount);
                            var result = new AuthenticateResult(userAccount.Id, userAccount.UserName, claims);
                            context.AuthenticateResult = result;
                            return;
                        }
                        else
                        {
                            _log.Debug("Password verification FAILED. Id: {AccountId}, UserName: {AccountUserName}", userAccount.Id, userAccount.UserName);
                            if (_UserManager.SupportsUserLockout)
                            {
                                _log.Debug("Incrementing failed login attempt count. Id: {AccountId}, UserName: {AccountUserName}", userAccount.Id, userAccount.UserName);
                                await _UserManager.AccessFailedAsync(userAccount.Id);
                            }
                            context.AuthenticateResult = new AuthenticateResult("Username or password is invalid");
                            return;
                        }
                    }
                }
                else
                {
                    _log.Debug("User {0} not found", username);
                    context.AuthenticateResult = new AuthenticateResult("Username or password is invalid");
                    return;

                }
            }
            catch (Exception ex)
            {
                //System.IO.File.AppendAllText(path, Environment.NewLine + DateTime.Now.ToString()+": "+ex.ToString() + Environment.NewLine);
                _log.Debug(ex, "The system is experiencing problem. Please contact system administrator");
                context.AuthenticateResult = new AuthenticateResult("The system is experiencing problem. Please contact system administrator");
                return;
            }
        }

        protected async virtual Task<User> FindUserAsync(string username)
        {
            return await _UserManager.FindByNameAsync(username);
        }

        private async Task<User> TryAutoLinkLdapAccountAsync(string username)
        {
            try
            {
                var userLdap = FindUserFromLdapWithFallback(username);
                if (userLdap == null)
                {
                    _log.Debug("LDAP user not found for '{UserName}'. Auto-link skipped.", username);
                    return null;
                }

                var resolvedUsername = string.IsNullOrWhiteSpace(userLdap.SamAccountName) ? username : userLdap.SamAccountName;
                var existingByResolvedUsername = await _UserManager.FindByNameAsync(resolvedUsername);
                if (existingByResolvedUsername != null)
                {
                    _log.Debug("Found existing local account with LDAP resolved username '{UserName}'.", resolvedUsername);
                    return existingByResolvedUsername;
                }

                var newUser = new User
                {
                    IsLdapUser = true,
                    UserName = resolvedUsername,
                    DisplayName = userLdap.DisplayName,
                    Email = userLdap.EmailAddress
                };
                if (userLdap.Guid.HasValue)
                {
                    newUser.Id = userLdap.Guid.Value.ToString();
                }

                var createResult = await _UserManager.CreateAsync(newUser, "P@ssw0rd!");
                if (!createResult.Succeeded)
                {
                    _log.Warn("Failed creating auto-linked LDAP user '{UserName}'. Errors: {Errors}",
                        resolvedUsername, string.Join(", ", createResult.Errors));
                    return null;
                }

                await _UserManager.AddClaimAsync(newUser.Id,
                    new Claim(IdLdapConstants.Claims.Role, IdLdapConstants.AppConfiguration.IdLdapUserRole));

                _log.Info("LDAP user '{UserName}' auto-linked successfully with id '{UserId}'.", resolvedUsername, newUser.Id);
                return newUser;
            }
            catch (Exception ex)
            {
                _log.Warn(ex, "LDAP auto-link failed for '{UserName}'.", username);
                return null;
            }
        }

        private UserPrincipal FindUserFromLdapWithFallback(string username)
        {
            try
            {
                bool useAppDir = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["LDAP_APPDIR"]);
                UserPrincipal userLdap = useAppDir ? _LdapRepository.FindUser2(username) : _LdapRepository.FindUser(username);
                if (userLdap != null)
                {
                    return userLdap;
                }

                // Fallback: coba metode kebalikan karena beberapa user disimpan dengan format identitas berbeda.
                userLdap = useAppDir ? _LdapRepository.FindUser(username) : _LdapRepository.FindUser2(username);
                if (userLdap != null)
                {
                    _log.Debug("LDAP user found via fallback identity lookup for '{UserName}'.", username);
                }
                return userLdap;
            }
            catch (Exception ex)
            {
                _log.Warn(ex, "LDAP lookup fallback failed for '{UserName}'.", username);
                return null;
            }
        }

        private bool ValidateLdapCredentialsWithFallback(string inputUsername, string storedUsername, string password)
        {
            var usernamesToTry = new List<string>();
            if (!string.IsNullOrWhiteSpace(storedUsername)) usernamesToTry.Add(storedUsername);
            if (!string.IsNullOrWhiteSpace(inputUsername)) usernamesToTry.Add(inputUsername);

            var domainHost = IdLdapConstants.LdapConfiguration.Host;
            if (!string.IsNullOrWhiteSpace(domainHost))
            {
                var shortDomain = domainHost.Split('.').FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(shortDomain))
                {
                    usernamesToTry.Add(shortDomain + "\\" + storedUsername);
                    usernamesToTry.Add(shortDomain + "\\" + inputUsername);
                }

                usernamesToTry.Add(storedUsername + "@" + domainHost);
                usernamesToTry.Add(inputUsername + "@" + domainHost);
            }

            foreach (var candidate in usernamesToTry.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                _log.Debug("Attempting LDAP credential validation using candidate username '{CandidateUserName}'.", candidate);
                if (_LdapRepository.ValidateCredentials(candidate, password))
                {
                    _log.Debug("LDAP credential validation succeeded using candidate username '{CandidateUserName}'.", candidate);
                    return true;
                }
            }

            return false;
        }

        private async Task<User> CreateMinimalLdapLinkedAccountAsync(string username)
        {
            try
            {
                var existing = await _UserManager.FindByNameAsync(username);
                if (existing != null) return existing;

                var newUser = new User
                {
                    IsLdapUser = true,
                    UserName = username,
                    DisplayName = username
                };

                var createResult = await _UserManager.CreateAsync(newUser, "P@ssw0rd!");
                if (!createResult.Succeeded)
                {
                    _log.Warn("Failed creating minimal auto-linked LDAP user '{UserName}'. Errors: {Errors}",
                        username, string.Join(", ", createResult.Errors));
                    return null;
                }

                await _UserManager.AddClaimAsync(newUser.Id,
                    new Claim(IdLdapConstants.Claims.Role, IdLdapConstants.AppConfiguration.IdLdapUserRole));

                _log.Info("Minimal LDAP user '{UserName}' auto-linked successfully with id '{UserId}'.", username, newUser.Id);
                return newUser;
            }
            catch (Exception ex)
            {
                _log.Warn(ex, "Failed to create minimal LDAP-linked user for '{UserName}'.", username);
                return null;
            }
        }

        public virtual async Task<IEnumerable<Claim>> GetClaimsForAuthenticateResult(User userIdentity)
        {
            List<Claim> claims = new List<Claim>();
            if (EnableSecurityStamp && _UserManager.SupportsUserSecurityStamp)
            {
                var stamp = await _UserManager.GetSecurityStampAsync(userIdentity.Id);
                if (!String.IsNullOrWhiteSpace(stamp))
                {
                    claims.Add(new Claim("security_stamp", stamp));
                }
            }

            return claims;
        }

        public override async Task GetProfileDataAsync(IdentityServer3.Core.Models.ProfileDataRequestContext context)
        {
            var subject = context.Subject;
            var requestedClaimTypes = context.RequestedClaimTypes;

            if (subject == null) throw new ArgumentNullException("subject");

            String key = subject.GetSubjectId();

            var userIdentity = await _UserManager.FindByIdAsync(key);

            if (userIdentity == null)
            {
                throw new ArgumentException("Invalid subject identifier");
            }

            _log.Debug("Obtaining claims. {AccountId}, {AccountUserName}, {IsDirectoryUser}", userIdentity.Id, userIdentity.UserName, userIdentity.IsLdapUser);
            IEnumerable<Claim> claims = await GetClaimsFromAccount(userIdentity);
            if (userIdentity.IsLdapUser)
            {
                _log.Debug("Account is directory-mapped. Overriding claims with those obtained from directory. {AccountId}, {AccountUserName}, {IsDirectoryUser}", userIdentity.Id, userIdentity.UserName, userIdentity.IsLdapUser);
                if (_LdapRepository != null)
                {
                    try
                    {
                        // Cari user LDAP berdasarkan SamAccountName dulu, lalu fallback ke UserPrincipalName.
                        // Tidak bisa pakai GetUserByGuid(key) karena 'key' adalah database ID,
                        // bukan LDAP GUID — keduanya hampir selalu berbeda.
                        var userLdap = _LdapRepository.FindUser(userIdentity.UserName);
                        if (userLdap == null)
                        {
                            _log.Debug("FindUser (SamAccountName) tidak menemukan user, mencoba FindUser2 (UserPrincipalName)...");
                            userLdap = _LdapRepository.FindUser2(userIdentity.UserName);
                        }

                        if (userLdap != null)
                        {
                            claims = await GetClaimsFromAccount(userLdap, userIdentity);
                            _log.Debug("Account is directory-mapped. Overriding claims successful. {AccountId}, {AccountUserName}, {IsDirectoryUser}", userIdentity.Id, userIdentity.UserName, userIdentity.IsLdapUser);
                        }
                        else
                        {
                            _log.Warn("LDAP user tidak ditemukan untuk username '{UserName}'. Menggunakan local claims sebagai fallback.", userIdentity.UserName);
                            // Tetap pakai local claims yang sudah di-set di atas
                        }
                    }
                    catch (Exception ex)
                    {
                        // Jangan biarkan exception di sini memutus alur /connect/authorize (error generik di UI).
                        _log.Error(ex, "Gagal mengambil/memap claim LDAP untuk user DB '{UserName}' (subject {Subject}). Fallback ke claim dari database saja.", userIdentity.UserName, key);
                    }
                }
                else
                {
                    _log.Warn("Account is directory-mapped. Overriding claims from directory FAILED. No directory is currently in use. {AccountId}, {AccountUserName}, {IsDirectoryUser}", userIdentity.Id, userIdentity.UserName, userIdentity.IsLdapUser);
                }
            }

            //if (requestedClaimTypes != null && requestedClaimTypes.Any())
            //{
            //    claims = claims.Where(x => requestedClaimTypes.Contains(x.Type));
            //}

            context.IssuedClaims = claims;

        }

        public virtual async System.Threading.Tasks.Task<IEnumerable<Claim>> GetClaimsFromAccount(User userIdentity)
        {
            var claims = new List<Claim>{
                new Claim(Constants.ClaimTypes.Subject, userIdentity.Id.ToString()),
                new Claim(Constants.ClaimTypes.PreferredUserName, userIdentity.UserName),
            };

            if (_UserManager.SupportsUserClaim)
            {
                claims.AddRange(await _UserManager.GetClaimsAsync(userIdentity.Id.ToString()));
            }

            if (_UserManager.SupportsUserRole)
            {
                var roleClaims =
                    from role in await _UserManager.GetRolesAsync(userIdentity.Id.ToString())
                    select new Claim(Constants.ClaimTypes.Role, role);
                claims.AddRange(roleClaims);
            }

            return claims;
        }

        public virtual async System.Threading.Tasks.Task<IEnumerable<Claim>> GetClaimsFromAccount(UserPrincipal userLdap, User userIdentity)
        {
            // Subject HARUS menggunakan userIdentity.Id (database ID), bukan userLdap.Guid.
            // IsActiveAsync() memanggil FindByIdAsync(subject) untuk validasi session —
            // jika subject berisi LDAP GUID (bukan database ID), user tidak akan ditemukan
            // dan sesi dianggap tidak aktif → redirect ke login terus.
            var preferredUserName = userIdentity.UserName;
            if (userLdap != null)
            {
                try
                {
                    var upn = userLdap.UserPrincipalName;
                    if (!string.IsNullOrWhiteSpace(upn))
                        preferredUserName = upn;
                }
                catch (Exception ex)
                {
                    _log.Warn(ex, "Tidak bisa membaca UserPrincipalName dari LDAP; memakai UserName database.");
                }
            }

            var claims = new List<Claim>
            {
                new Claim(Constants.ClaimTypes.Subject, userIdentity.Id.ToString()),
                new Claim(Constants.ClaimTypes.PreferredUserName, preferredUserName),
            };

            // Gunakan userIdentity.Id untuk lookup ke database (bukan userLdap.Guid)
            if (_UserManager.SupportsUserClaim)
            {
                claims.AddRange(await _UserManager.GetClaimsAsync(userIdentity.Id.ToString()));
            }

            if (_UserManager.SupportsUserRole)
            {
                var roleClaims =
                    from role in await _UserManager.GetRolesAsync(userIdentity.Id.ToString())
                    select new Claim(Constants.ClaimTypes.Role, role);
                claims.AddRange(roleClaims);
            }

            return claims;
        }

        public override async System.Threading.Tasks.Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject;

            if (subject == null) throw new ArgumentNullException("subject");

            String key = subject.GetSubjectId();
            var acct = await _UserManager.FindByIdAsync(key);

            context.IsActive = false;

            if (acct != null)
            {
                if (EnableSecurityStamp && _UserManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(x => x.Type == "security_stamp").Select(x => x.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _UserManager.GetSecurityStampAsync(key);
                        if (db_security_stamp != security_stamp)
                        {
                            return;
                        }
                    }
                }

                context.IsActive = true;
            }
        }


    }
}
