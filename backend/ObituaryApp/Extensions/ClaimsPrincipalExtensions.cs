/**
 * Provides extension methods for ClaimsPrincipal.
 
 * Used to retrieve the current logged-in user's Id for ownership checks.
 */
using System.Security.Claims;

namespace ObituaryApp.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
