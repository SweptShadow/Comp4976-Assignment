using System.Net.Http.Json;
using frontend.Auth;
using frontend.Models;

namespace frontend.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string email, string password, CancellationToken ct = default);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly JwtAuthenticationStateProvider _authProvider;

        public AuthService(HttpClient http, JwtAuthenticationStateProvider authProvider)
        {
            _http = http;
            _authProvider = authProvider;
        }

        public async Task<bool> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            try
            {
                var req = new LoginRequest { Email = email, Password = password };
                var resp = await _http.PostAsJsonAsync("api/Auth/login", req, ct);
                if (!resp.IsSuccessStatusCode)
                    return false;

                var body = await resp.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: ct);
                if (body == null || string.IsNullOrWhiteSpace(body.Token))
                    return false;

                await _authProvider.MarkUserAsAuthenticatedAsync(body.Token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            await _authProvider.MarkUserAsLoggedOutAsync();
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var state = await _authProvider.GetAuthenticationStateAsync();
            return state.User.Identity?.IsAuthenticated == true;
        }
    }
}
