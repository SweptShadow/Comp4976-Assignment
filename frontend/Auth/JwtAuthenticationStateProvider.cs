using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using frontend.Services;

namespace frontend.Auth
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenStorage _tokenStorage;

        public JwtAuthenticationStateProvider(ITokenStorage tokenStorage)
        {
            _tokenStorage = tokenStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenStorage.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticatedAsync(string token)
        {
            await _tokenStorage.SetTokenAsync(token);
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            await _tokenStorage.ClearTokenAsync();
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            // JWT format: header.payload.signature
            var segments = jwt.Split('.');
            if (segments.Length < 2) yield break;
            var payload = segments[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes) ?? new();

            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Value is JsonElement el)
                {
                    if (el.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in el.EnumerateArray())
                        {
                            yield return new Claim(kvp.Key, item.ToString());
                        }
                    }
                    else
                    {
                        yield return new Claim(kvp.Key, el.ToString());
                    }
                }
                else
                {
                    yield return new Claim(kvp.Key, kvp.Value?.ToString() ?? "");
                }
            }
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            base64 = base64.Replace('-', '+').Replace('_', '/');
            return Convert.FromBase64String(base64);
        }
    }
}

