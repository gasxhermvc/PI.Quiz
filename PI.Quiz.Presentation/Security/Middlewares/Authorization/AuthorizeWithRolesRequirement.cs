using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PI.Quiz.Presentation.Security.Middlewares.Authorization
{
    public class AuthorizeWithRolesRequirement : IAuthorizationRequirement
    {
        public string[] Roles { get; }

        public AuthorizeWithRolesRequirement(params string[] roles)
        {
            Roles = roles;
        }
    }

    public class RoleHandler : AuthorizationHandler<AuthorizeWithRolesRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizeWithRolesRequirement requirement)
        {
            if (!requirement.Roles.Any(role => context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == role)))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeWithPolicyAttribute : AuthorizeAttribute
    {
        public AuthorizeWithPolicyAttribute(string policyName) : base(policyName)
        {
        }
    }
}
