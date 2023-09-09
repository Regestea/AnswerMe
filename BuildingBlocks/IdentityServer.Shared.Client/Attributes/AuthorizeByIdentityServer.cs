using IdentityServer.Shared.Client.GrpcServices;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer.Api.Protos;

namespace IdentityServer.Shared.Client.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]

    public class AuthorizeByIdentityServer : ActionFilterAttribute, IAsyncActionFilter
    {

        private readonly string? requiredRoles;

        public AuthorizeByIdentityServer(string? requiredRoles = null)
        {
            this.requiredRoles = requiredRoles;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            
            var authorizationGrpcServices =
                context.HttpContext.RequestServices.GetService<AuthorizationGrpcServices>() ??
                throw new ArgumentNullException(nameof(AuthorizationGrpcServices));

            var tokenCache = context.HttpContext.RequestServices.GetService<ICacheRepository>() ??
                             throw new ArgumentNullException(nameof(ICacheRepository));


            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }

            var jwtToken = authorizationHeader.ToString().Replace("Bearer ", "");

            ValidateTokenResponse? response;

            // try to get token and roles from cache
            response = await tokenCache.GetAsync<ValidateTokenResponse>(jwtToken);

            if (response == null)
            {
                response = await authorizationGrpcServices.ValidateTokenAsync(jwtToken);
                if (response.Valid)
                {
                    await tokenCache.SetAsync(jwtToken, response, TimeSpan.FromHours(3));
                }
            }

            if (!response.Valid)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }

            if (requiredRoles == null)
            {
                await next();
                return;
            }

            //check user role for required roles
            var roleList = response.Roles.Split(",");
            var requiredRoleList = requiredRoles.Split("|");

            foreach (var requiredRoleGroup in requiredRoleList)
            {
                var requiredRoleSubList = requiredRoleGroup.Split(",");
                var hasRequiredRole = true;

                foreach (var requiredRole in requiredRoleSubList)
                {
                    if (!roleList.Any(x => x == requiredRole))
                    {
                        hasRequiredRole = false;
                        break;
                    }
                }

                if (hasRequiredRole)
                {
                    await next();
                    return;
                }
            }

            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            return;
        }
    }
}