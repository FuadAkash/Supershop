using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using Supershop.Models;

namespace Supershop.Authorization
{
    public class AdminAuthorization : AuthorizeAttribute, IAuthorizationRequirement
    {
        public const string PolicyName = "AdminOnly";

        public AdminAuthorization()
        {
            AuthenticationSchemes = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
            Policy = PolicyName;
        }
    }

    public class AdminRequirement : IAuthorizationRequirement
    {
        public string RequiredType { get; }

        public AdminRequirement(string requiredType)
        {
            RequiredType = requiredType;
        }
    }

    public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                return Task.CompletedTask;
            }

            var userTypeClaim = context.User.FindFirst(c => c.Type == "UserType");
            if (userTypeClaim != null && userTypeClaim.Value == requirement.RequiredType)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class OfficerAuthorization : AuthorizeAttribute, IAuthorizationRequirement
    {
        public const string PolicyName = "AdminAndMTofficerOnly";

        public OfficerAuthorization()
        {
            AuthenticationSchemes = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
            Policy = PolicyName;
        }
    }

    public class OfficerRequirement : IAuthorizationRequirement
    {
        public string[] RequiredTypes { get; }

        public OfficerRequirement(params string[] requiredTypes)
        {
            RequiredTypes = requiredTypes;
        }
    }

    public class OfficerAuthorizationHandler : AuthorizationHandler<OfficerRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OfficerRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                return Task.CompletedTask;
            }

            var userTypeClaim = context.User.FindFirst(c => c.Type == "UserType");
            if (userTypeClaim != null && requirement.RequiredTypes.Contains(userTypeClaim.Value))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
