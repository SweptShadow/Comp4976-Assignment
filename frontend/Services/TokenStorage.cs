using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace frontend.Services
{
    public interface ITokenStorage
    {
        Task SetTokenAsync(string token);
        Task<string?> GetTokenAsync();
        Task ClearTokenAsync();
    }

    public class TokenStorage : ITokenStorage
    {
        private const string Key = "auth_token";
        private readonly IJSRuntime _js;

        public TokenStorage(IJSRuntime js)
        {
            _js = js;
        }

        public async Task SetTokenAsync(string token)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", Key, token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _js.InvokeAsync<string?>("localStorage.getItem", Key);
        }

        public async Task ClearTokenAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", Key);
        }
    }
}

