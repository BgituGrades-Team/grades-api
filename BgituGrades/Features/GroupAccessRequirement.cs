using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BgituGrades.Features
{
    public class GroupAccessRequirement : IAuthorizationRequirement { }

    public class GroupAccessHandler : AuthorizationHandler<GroupAccessRequirement>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        GroupAccessRequirement requirement)
        {
            if (context.User.IsInRole("ADMIN") || context.User.IsInRole("EDIT"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var groupClaim = context.User.FindFirst("group_id")?.Value;
            if (context.Resource is HubInvocationContext)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.Resource is HttpContext httpContext)
            {
                var queryGroupIds = httpContext.Request.Query["groupIds"]
                    .ToString()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => id.Trim())
                    .ToList();

                if (queryGroupIds.Count == 0)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                if (groupClaim != null && queryGroupIds.All(id => id == groupClaim))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
