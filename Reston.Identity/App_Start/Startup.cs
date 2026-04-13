using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Security.Cryptography.X509Certificates;
using IdentityServer3.Core.Configuration;
using Reston.Identity.Helper;
using IdLdap.Configuration;
using IdentityServer3.Core.Services;
using System.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Web.Helpers;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using IdentityModel.Tokens;
using IdentityModel.Client;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using Microsoft.Owin.Security;
using Microsoft.IdentityModel.Protocols;
using Reston.Identity.Helper.Util;
using System.DirectoryServices.AccountManagement;
using Microsoft.Owin.Security.Jwt;
using System.Web.Http;
using NLog;

[assembly: OwinStartup(typeof(IdLdap.Startup))]

namespace IdLdap
{
    public partial class Startup
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        public void Configuration(IAppBuilder app)
        {
            //
            // IdentityServer CORS Policy seems to not work resulting in all CORS preflight
            // returning failure response. 
            //
            // This middleware forces CORS preflight request to return proper responses
            //
            app.Use(async (context, next) =>
            {
                _log.Debug("CORS middleware starting...");
                context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Append("Access-Control-Allow-Headers",
                    "Accept-Encoding, Content-Type, X-CSRF-Token, X-Requested-With, "
                    + "Accept, Accept-Version, Content-Length, Content-MD5, Date, "
                    + "X-Api-Version, X-File-Name, Origin, Referer, User-Agent");
                context.Response.Headers.Append("Access-Control-Expose-Headers", "x-powered-by");
                //context.Response.Headers.Append("Access-Control-Allow-Headers", "Accept-Encoding, Accept, ");
                context.Response.Headers.Append("Access-Control-Allow-Methods", "POST,GET,PUT,PATCH,DELETE,OPTIONS");
                context.Response.Headers.Append("Access-Control-Allow-Credential", "true");
                await next();
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 200;
                }
                _log.Debug("CORS middleware completed.");

            });


            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>
            {
                {"role", System.Security.Claims.ClaimTypes.Role}
            };

            AntiForgeryConfig.UniqueClaimTypeIdentifier = IdLdapConstants.Claims.UniqueUserKey;

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                SlidingExpiration = true,
                ExpireTimeSpan = System.TimeSpan.FromMinutes(500)

            });


            app.Map("/identity", idsrvApp =>
            {

                var idSvrFactory = IdFactory.Configure(IdLdapConstants.Configuration.IdLdapConnectionString);

                _log.Info("Registering required services...");

                var contextType = IdLdapConstants.LdapConfiguration.ContextType;
                if (IdLdapConstants.LdapContextType.Machine.Equals(contextType, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Register a 'Machine' Principal context.
                    // This will allow you to map application accounts to local windows accounts
                    // Suitable for development
                    idSvrFactory.Register(new Registration<PrincipalContext>(
                        resolver => new PrincipalContext(ContextType.Machine)
                    ));

                    idSvrFactory.Register(new Registration<ILdapRepository, LdapRepository>());
                    _log.Info("Registering Machine-backed User Service.");
                    idSvrFactory.UserService = new Registration<IUserService>(
                        resolver => new ADUserService(resolver.Resolve<ILdapRepository>())
                    );
                }
                else if (IdLdapConstants.LdapContextType.Domain.Equals(contextType, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Register a 'Domain' Principal context.
                    // This will allow you to map application accounts to Active Directory accounts
                    idSvrFactory.Register(new Registration<PrincipalContext>(
                        resolver => new PrincipalContext(
                                        ContextType.Domain,
                                        IdLdapConstants.LdapConfiguration.Host,
                                        IdLdapConstants.LdapConfiguration.ContextNaming,
                                        ContextOptions.SimpleBind,
                                        IdLdapConstants.LdapConfiguration.Username,
                                        IdLdapConstants.LdapConfiguration.Password)
                    ));

                    idSvrFactory.Register(new Registration<ILdapRepository, LdapRepository>());
                    _log.Info("Registering Domain-backed User Service");
                    idSvrFactory.UserService = new Registration<IUserService>(
                        resolver => new ADUserService(resolver.Resolve<ILdapRepository>())
                    );
                }
                else if (IdLdapConstants.LdapContextType.ApplicationDirectory.Equals(contextType, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Register a 'ApplicationDirectory' Principal context.
                    // This will allow you to map application accounts to Active Directory LDS (ADAM) accounts
                    // Suitable for development if you have AD-LDS as directory server
                    idSvrFactory.Register(new Registration<PrincipalContext>(
                        resolver => new PrincipalContext(
                                         ContextType.ApplicationDirectory,
                                         IdLdapConstants.LdapConfiguration.Host,
                                         IdLdapConstants.LdapConfiguration.ContextNaming,
                                         ContextOptions.SimpleBind,
                                         IdLdapConstants.LdapConfiguration.Username,
                                         IdLdapConstants.LdapConfiguration.Password)
                    ));

                    idSvrFactory.Register(new Registration<ILdapRepository, LdapRepository>());
                    idSvrFactory.UserService = new Registration<IUserService, ADUserService>();
                    _log.Info("Registering Application Directory -backed User Service");
                    idSvrFactory.UserService = new Registration<IUserService>(
                        resolver => new ADUserService(resolver.Resolve<ILdapRepository>())
                    );

                }
                else
                {
                    _log.Warn("'LDAP_CONTEXT_TYPE' is not set or set to unknown value. Skipping LDAP repository creation. You will only be able to use local accounts.");
                    _log.Info("Registering 'local' User Service. This User Service can only manage 'local' accounts...");
                    idSvrFactory.UserService = new Registration<IUserService>(
                        resolver => new ADUserService()
                    );
                }


                //idSvrFactory.ConfigureUserService(IdLdapConstants.Configuration.EConnectionString);

                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = IdLdapConstants.AppConfiguration.IdentitySiteName,
                    IssuerUri = IdLdapConstants.AppConfiguration.IdentitySiteIssueruri,
                    SigningCertificate = LoadCertificate(),
                    RequireSsl = IdLdapConstants.AppConfiguration.IdentitySiteSLL,
                    Factory = idSvrFactory,
                    LoggingOptions = new LoggingOptions
                    { 
                        EnableHttpLogging = true,
                        EnableWebApiDiagnostics = true,
                        EnableKatanaLogging = true,
                        WebApiDiagnosticsIsVerbose = true,
                    }
                });
            });

            app.Map("/api", api =>
            {
                //api.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions {
                //    Authority = Client.Config.Url,
                //    RequiredScopes = new[] {
                //        Client.Config.Scope.DirectoryAPI
                //    }
                //});

                api.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
                {
                    AllowedAudiences = new string[] {
                        IdLdapConstants.Id.Url + "resources",
                    },
                    AuthenticationType = "Bearer",

                    IssuerSecurityTokenProviders = new[] { 
                        new X509CertificateSecurityTokenProvider(IdLdapConstants.Id.Url, LoadCertificate())
                    },

                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                });

                var config = new HttpConfiguration();
                config.MapHttpAttributeRoutes();
                //                config.Filters.Add(new AuthorizeAttribute() { Roles=RepoConfig.TempConfiguration.AdminRole });
                api.UseWebApi(config);
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = IdLdapConstants.IDM.ClientId,
                Authority = IdLdapConstants.Id.Url,
                RedirectUri = IdLdapConstants.IDM.Url,
                PostLogoutRedirectUri = IdLdapConstants.IDM.Url,
                SignInAsAuthenticationType = IdLdapConstants.SignInAsAuthenticationType.Cookies,
                UseTokenLifetime = false,

                ResponseType = string.Format("{0} {1} {2}", IdLdapConstants.ResponseType.Code,
                                            IdLdapConstants.ResponseType.IdToken,
                                            IdLdapConstants.ResponseType.Token),

                Scope = string.Format("{0} {1} {2} {3} {4}",
                                       IdLdapConstants.Scope.Openid, IdLdapConstants.Scope.Profile,
                                       IdLdapConstants.Scope.Roles, IdLdapConstants.Scope.OfflineAccess, IdLdapConstants.Scope.IdentityManager),

                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    MessageReceived = async n =>
                    {
                        EndpointAndTokenHelper.DecodeAndWrite(n.ProtocolMessage.IdToken);
                        EndpointAndTokenHelper.DecodeAndWrite(n.ProtocolMessage.AccessToken);
                    },

                    AuthorizationCodeReceived = async n =>
                    {
                        // use the code to get the access and refresh token
                        var tokenClient = new TokenClient(
                            IdLdapConstants.Id.TokenEndpoint,
                            IdLdapConstants.IDM.ClientId,
                            IdLdapConstants.IDM.FirstSecret);

                        var response = await tokenClient.RequestAuthorizationCodeAsync(n.Code, n.RedirectUri);
                        var id = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);
                        
                        var userInfo = await EndpointAndTokenHelper.CallUserInfoEndpoint(response.AccessToken);
                        JToken roles;
                        try
                        {
                           roles = userInfo.Value<JValue>(Thinktecture.IdentityModel.Client.JwtClaimTypes.Role).ToObject<JToken>();
                        }catch{
                            roles = userInfo.Value<JArray>(Thinktecture.IdentityModel.Client.JwtClaimTypes.Role).ToObject<JToken>();
                        }

                        foreach (var role in roles)
                        {
                            id.AddClaim(new Claim(
                            Thinktecture.IdentityModel.Client.JwtClaimTypes.Role,
                            role.ToString()));
                        }


                        var issuerClaim = n.AuthenticationTicket.Identity
                            .FindFirst(Thinktecture.IdentityModel.Client.JwtClaimTypes.Issuer);
                        var subjectClaim = n.AuthenticationTicket.  Identity
                            .FindFirst(Thinktecture.IdentityModel.Client.JwtClaimTypes.Subject);


                        id.AddClaim(new Claim(IdLdapConstants.Claims.UniqueUserKey,
                            issuerClaim.Value + "_" + subjectClaim.Value));

                        id.AddClaim(new Claim(IdLdapConstants.Claims.Subject, subjectClaim.Value));
                        id.AddClaim(new Claim(IdLdapConstants.Claims.AccessToken, response.AccessToken));
                        id.AddClaim(new Claim(IdLdapConstants.Claims.ExpiresAt, DateTime.Now.AddSeconds(response.ExpiresIn).Ticks.ToString()));
                        id.AddClaim(new Claim(IdLdapConstants.Claims.RefreshToken, response.RefreshToken));
                        id.AddClaim(new Claim(IdLdapConstants.Claims.IdToken, n.ProtocolMessage.IdToken));
                        id.AddClaim(new Claim(IdLdapConstants.Claims.LogoutUri, IdLdapConstants.Proc.Url));

                        var username = userInfo.Value<string>(Thinktecture.IdentityModel.Client.JwtClaimTypes.PreferredUserName).ToString();
                        id.AddClaim(new Claim(IdLdapConstants.Claims.PreferredUserName, username));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            new ClaimsIdentity(id.Claims, n.AuthenticationTicket.Identity.AuthenticationType),
                            n.AuthenticationTicket.Properties);
                    },

                    RedirectToIdentityProvider = n =>
                    {
                        // if signing out, add the id_token_hint
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst(IdLdapConstants.Claims.IdToken);
                            var logouturi = n.OwinContext.Authentication.User.FindFirst(IdLdapConstants.Claims.LogoutUri);

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                                n.ProtocolMessage.PostLogoutRedirectUri = logouturi.Value;
                            }
                        }
                        return Task.FromResult(0);
                    },
                }
            });


            //
            // Somehow somewhere along the middleware pipeline, a sucessfull CORS preflight
            // returning HTTP status code 204 (no content), some browser (sebut saja namanya 'Chrome')
            // considers 204 as preflight failure and wont continue with the actual request.
            //
            // This middleware forces CORS preflight request to return proper responses
            //
            app.Use(async (context, next) =>
            {
                _log.Debug("CORS middleware 2 starting...");
                await next();
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 200;
                }
                _log.Debug("CORS middleware 2 completed.");

            });

        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\Certificates\{1}",
                AppDomain.CurrentDomain.BaseDirectory, IdLdapConstants.AppConfiguration.IdentityCertificateFullname), IdLdapConstants.AppConfiguration.IdentityCertificatePassword);
        }
    }

}
