using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.Http.Headers;
using WebApi.Services;

namespace WebApi.Helpers
{
    public class BearerTokenValidationFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();

            if (allowAnonymous)
                return;

            var svc = context.HttpContext.RequestServices;
            var tokenManager = svc.GetService<ITokenManager>();

            if (tokenManager == null)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Content = "Internal Server Error"
                };

            }

            var request = context.HttpContext.Request;

            if (!request.Headers.ContainsKey("Authorization"))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = "Error with Authentication Header"
                };

                return;
            }

            AuthenticationHeaderValue authorization = AuthenticationHeaderValue.Parse(request.Headers["Authorization"]);

            if (authorization == null || authorization.Scheme != "Bearer" || string.IsNullOrEmpty(authorization.Parameter))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = "Error with Authentication Header"
                };

                return;
            }

            bool isTokenValid = tokenManager.ValidateJWTToken(authorization.Parameter);

            if (!isTokenValid)  
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Content = "JWT Token is unvalid !!!"
                };

                return;
            }

        }
    }
}
