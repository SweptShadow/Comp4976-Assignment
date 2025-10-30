using System.Net.Http.Headers;
using System.Text.Json;
using frontend.Services;
using Microsoft.AspNetCore.Components;

namespace frontend.Auth
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly ITokenStorage _tokenStorage;
        private readonly JwtAuthenticationStateProvider _authProvider;
        private readonly NavigationManager _nav;

        public AuthMessageHandler(ITokenStorage tokenStorage, JwtAuthenticationStateProvider authProvider, NavigationManager nav)
        {
            _tokenStorage = tokenStorage;
            _authProvider = authProvider;
            _nav = nav;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenStorage.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token) && !IsExpired(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else if (!string.IsNullOrWhiteSpace(token) && IsExpired(token))
            {
                // Token expired: log out and redirect to login with returnUrl
                await _authProvider.MarkUserAsLoggedOutAsync();
                NavigateToLoginWithReturn();
            }

            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                await _authProvider.MarkUserAsLoggedOutAsync();
                NavigateToLoginWithReturn();
            }
            return response;
        }

        private void NavigateToLoginWithReturn()
        {
            var current = _nav.ToBaseRelativePath(_nav.Uri);
            var returnUrl = string.IsNullOrEmpty(current) ? "/" : $"/{current}";
            _nav.NavigateTo($"/login?returnUrl={Uri.EscapeDataString(returnUrl)}", forceLoad: false);
        }

        private static bool IsExpired(string jwt)
        {
            try
            {
                var parts = jwt.Split('.');
                if (parts.Length < 2) return true;
                var payload = parts[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var doc = JsonDocument.Parse(jsonBytes);
                if (!doc.RootElement.TryGetProperty("exp", out var expEl)) return true;
                var expSeconds = expEl.ValueKind == JsonValueKind.Number ? expEl.GetInt64() : long.Parse(expEl.GetString() ?? "0");
                var exp = DateTimeOffset.FromUnixTimeSeconds(expSeconds);
                // allow small clock skew of 30s
                return DateTimeOffset.UtcNow >= exp.AddSeconds(-30);
            }
            catch { return true; }
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
