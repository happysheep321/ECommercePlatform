using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerce.BuildingBlocks.Authentication.Attributes
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var permissions = context.User.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();

            var mvcContext = context.Resource as AuthorizationFilterContext;

            // 直接用 .NET 8 新的 GetEndpoint() 扩展方法
            var endpoint = mvcContext?.HttpContext?.GetEndpoint();
            var policyName = endpoint?.Metadata
                .GetMetadata<AuthorizeAttribute>()?.Policy;

            if (requirement.Permission == "*" ||
                (!string.IsNullOrEmpty(policyName) && permissions.Contains(policyName)))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}