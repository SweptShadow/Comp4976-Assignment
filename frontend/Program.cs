using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using frontend;
using frontend.Auth;
using frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Authorization & Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ITokenStorage, TokenStorage>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddScoped<AuthMessageHandler>();

// HttpClient for API with auth handler (uses factory so inner handler is assigned)
var baseUrl = builder.Configuration["BackendUrl"] ?? "http://localhost:5151";
builder.Services
    .AddHttpClient("ApiClient", client => client.BaseAddress = new Uri(baseUrl))
    .AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));

// Auth service
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();
