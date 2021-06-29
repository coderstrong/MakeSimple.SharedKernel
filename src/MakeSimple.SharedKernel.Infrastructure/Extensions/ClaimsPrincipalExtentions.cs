using System;
using System.Linq;
using System.Security.Claims;

namespace MakeSimple.SharedKernel.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtentions
    {
        public static string CovertToString(this ClaimsPrincipal user)
        {
            if (user != null && user.Claims != null && user.Claims.Any())
            {
                var claims = user.Claims.GroupBy(e => e.Type)
                        .ToDictionary(g => g.Key.Split("/").Last(), g => g.Select(c => c.Value).ToArray());
                return string.Join(", ", claims.Select(c => $"{c.Key}: {string.Join(", ", c.Value)}").ToArray());
            }
            return string.Empty;
        }
    }
}
