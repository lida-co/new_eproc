using IdentityModel.Client;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.StaticFiles;
using Model.Helper;
using Newtonsoft.Json.Linq;
using NLog;
using Owin;
using Reston.EProc.Web.Helper;
//using Reston.Pinata.Model.IdentityRepository;
using Reston.Pinata.Model;
using Reston.Pinata.Model.Repository;
using Reston.Pinata.WebService.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IdentityModel.Tokens;
using System.Linq;
//using System.Web.Helpers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Webservice.Helper.Util;

//using Reston.Pinata.Model.PengadaanRepository;

//[assembly: OwinStartup(typeof(StartUp), "Configuration")]
namespace Reston.Pinata.WebService
{
    public partial class Startup
    {
        static Logger _log = LogManager.GetCurrentClassLogger();

        public void Configuration(IAppBuilder app)
        {
            //idldp
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>
            {
                {"role", System.Security.Claims.ClaimTypes.Role}
            };
            //AntiForgeryConfig.UniqueClaimTypeIdentifier = IdLdapConstants.Claims.UniqueUserKey;

            // Add logging middleware
            // This middleware captures and logs exceptions before rethrowing
            // This middleware should not alter the pipeline in anyway besides logging exceptions
            app.Use(async (context, next) =>
            {

                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    if (!ex.GetType().Equals(typeof(System.OperationCanceledException)))
                    {
                        var requestMethod = context.Request.Method;
                        var requestUri = context.Request.Uri;
                        _log.Error(ex, "--- An exception has occurred during normal course of request handling: {RequestMethod} {RequestUri} ---", requestMethod, requestUri);
                        if (ex.InnerException != null)
                        {
                            _log.Error(ex.InnerException, "--- Inner exception ---");
                        }
                        throw ex;
                    }
                    // else, ignore OperationCancelledException
                }

            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                SlidingExpiration = true,
                ExpireTimeSpan = System.TimeSpan.FromMinutes(150)

            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = IdLdapConstants.Proc.ClientId,
                Authority = IdLdapConstants.Id.Url,
                RedirectUri = IdLdapConstants.Proc.Url,
                PostLogoutRedirectUri = IdLdapConstants.Proc.Url,
                SignInAsAuthenticationType = IdLdapConstants.SignInAsAuthenticationType.Cookies,
                UseTokenLifetime = false,
                
                ResponseType = string.Format("{0} {1} {2}", IdLdapConstants.ResponseType.Code,
                                            IdLdapConstants.ResponseType.IdToken,
                                            IdLdapConstants.ResponseType.Token),

                Scope = string.Format("{0} {1} {2} {3}",
                                       IdLdapConstants.Scope.Openid, IdLdapConstants.Scope.Profile,
                                       IdLdapConstants.Scope.Roles, IdLdapConstants.Scope.OfflineAccess),

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
                            "proc",
                            "secret");

                        var response = await tokenClient.RequestAuthorizationCodeAsync(n.Code, n.RedirectUri);
                        var id = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);

                        var userInfo = await EndpointAndTokenHelper.CallUserInfoEndpoint(response.AccessToken);


                        JToken roles;
                        try
                        {
                            roles = userInfo.Value<JValue>(Thinktecture.IdentityModel.Client.JwtClaimTypes.Role).ToObject<JToken>();
                        }
                        catch
                        {
                            roles = userInfo.Value<JArray>(Thinktecture.IdentityModel.Client.JwtClaimTypes.Role).ToObject<JToken>();
                        }

                        foreach (var role in roles)
                        {
                            id.AddClaim(new Claim(Thinktecture.IdentityModel.Client.JwtClaimTypes.Role, role.ToString()));
                        }


                        var issuerClaim = n.AuthenticationTicket.Identity
                            .FindFirst(Thinktecture.IdentityModel.Client.JwtClaimTypes.Issuer);
                        var subjectClaim = n.AuthenticationTicket.Identity
                            .FindFirst(Thinktecture.IdentityModel.Client.JwtClaimTypes.Subject);

                        id.AddClaim(new Claim(IdLdapConstants.Claims.UniqueUserKey,
                            issuerClaim.Value + "_" + subjectClaim.Value));

                        id.AddClaim(new Claim(IdLdapConstants.Claims.Subject, subjectClaim.Value));
                        id.AddClaim(new Claim(IdLdapConstants.Claims.AccessToken, response.AccessToken));
                        id.AddClaim(new Claim(IdLdapConstants.Claims.ExpiresAt, DateTime.Now.AddSeconds(response.ExpiresIn).ToLocalTime().ToString()));
                        id.AddClaim(new Claim(IdLdapConstants.Claims.RefreshToken, response.RefreshToken));
                        id.AddClaim(new Claim(IdLdapConstants.Claims.IdToken, n.ProtocolMessage.IdToken));
                        id.AddClaim(new Claim(IdLdapConstants.Claims.LogoutUri, IdLdapConstants.Proc.Url));
                        var preferredUserName = subjectClaim.Value;
                        var preferredUserNameClaim = userInfo[IdLdapConstants.Claims.PreferredUserName];
                        if (preferredUserNameClaim != null)
                        {
                            preferredUserName = preferredUserNameClaim.ToString();
                        }
                        id.AddClaim(new Claim(IdLdapConstants.Claims.PreferredUserName, preferredUserName));


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


            // Add this middleware to ensure that all request coming in to
            // static html files also requires to pass authentication
            app.Use((owinContext, next) => {

                var request = owinContext.Request;
                var response = owinContext.Response;
                var requestUri = request.Uri;

                if (
                    (
                        (requestUri.AbsolutePath.EndsWith("/") || requestUri.AbsolutePath.EndsWith(".html"))
                        && 
                        !requestUri.AbsolutePath.EndsWith("lacak.html") 
                        && 
                        !requestUri.AbsolutePath.EndsWith("registrasiv2.html")
                        && 
                        !requestUri.AbsolutePath.EndsWith("kebijakan-pendaftaran.html")
                    )
                    && (request.User == null || !request.User.Identity.IsAuthenticated)
                    )
                {
                    // To signal the authentication middleware that this request needs
                    // to be authenticated, simply returns HTTP 401 Unauthorized response.
                    // Note that some authentication middlewares must be configured to
                    // 'actively' redirects user to whatever authentication URL it's configured with
                    // if that is not specified, this request will simply yield the HTTP 401.
                    response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    
                    // Then we...short-circuit the request (WTF???)
                    // How can the OIDCAuthMware handles the 401 if we short ciruit the request??.
                    // But it works!!
                    return Task.CompletedTask;
                }
                return next();

            });

            //ConfigureAuth(app); //commented by jimbis

            //var contentDir = ConfigurationManager.AppSettings["www.rootDir"];
            //var physicalFileSystem = new PhysicalFileSystem(@"D:\project mandiri\mockup html");

            // var physicalFileSystem = new PhysicalFileSystem(@"..\..\..\WebService\mockup");
            // var physicalFileSystem = new PhysicalFileSystem(@"..\Reston.Pinata\WebService\mockup");

            //var physicalFileSystem = new PhysicalFileSystem(@"..\..\..\WebService\mockup");
            string viewPath = System.Configuration.ConfigurationManager.AppSettings["VIEW_BASE_PATH"];
            var physicalFileSystem = new PhysicalFileSystem(viewPath);//
            //var physicalFileSystem = new PhysicalFileSystem(@"..\PROC\mockup");//mtf serper

            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem

            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[]
			{
				"index.html"
			};

            app.UseFileServer(options);
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, Reston.Pinata.Model.IdentityMigrations.IdentityConfiguration>());
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, Reston.Pinata.Model.Migrations.Configuration>());
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<PengadaanContext, Reston.Pinata.Model.PengadaanMigrations.PengadaanConfiguration>()); 

            HttpConfiguration config = new HttpConfiguration();

            config.Filters.Add(new ValidateCsrfAttribute());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);

            if (PengadaanConstants.RunSeeder == true)
            {
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, Reston.Pinata.Model.Migrations.Configuration>());
            }
            /*
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                SlidingExpiration = true,
                ExpireTimeSpan = System.TimeSpan.FromMinutes(15),
                LoginPath = new PathString("/login.html")
            });
		    */


        }


    }



}
