using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Reston.Pinata.WebService.Helper;
using Reston.EProc.Web.Base.ViewModels;

namespace Reston.EProc.Web.Base.Attributes
{
    public class RequestHandlerAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            if (actionContext.ControllerContext.Controller is ApiController controller)
            {

                var actionArgs = actionContext.ActionArguments;
                foreach (var actionArg in actionArgs)
                {
                    var argName = actionArg.Key;
                    var argValue = actionArg.Value;

                    if (argValue is BaseRequest request)
                    {
                        // Check if the ViewModel has a Header property
                        var headerProperty = request.GetType().GetProperty("Header");
                        if (headerProperty != null)
                        {
                            // Check if the Header property has a UserId property
                            var userIdProperty = headerProperty.PropertyType.GetProperty("UserId");
                            if (userIdProperty != null)
                            {
                                // Assign the current user's (if any) Subject claim to the UserId property
                                if (controller.User != null) {
                                    var currentPrincipal = new AppUser(controller.User as ClaimsPrincipal);
                                    userIdProperty.SetValue(headerProperty.GetValue(request), currentPrincipal.Subject);
                                }
                            }
                        }
                    }
                }
            }

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }

        private string GetUserIdFromClaims(HttpRequestMessage request)
        {
            var principal = request.GetRequestContext().Principal as ClaimsPrincipal;
            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value;
        }
    }
}
