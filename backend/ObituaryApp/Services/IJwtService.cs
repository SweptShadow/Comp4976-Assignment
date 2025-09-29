using ObituaryApp.Models;

namespace ObituaryApp.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(ApplicationUser user, IList<string> roles);
    }
}