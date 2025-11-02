using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add the backend (ObituaryApp) project
var backend = builder.AddProject<Projects.ObituaryApp>("backend");

// Add the frontend (Blazor WebAssembly) project
var frontend = builder.AddProject<Projects.frontend>("frontend")
                      .WithReference(backend); // frontend depends on backend

// Pass backend URL to frontend and frontend URL to backend for CORS
frontend.WithEnvironment("BackendUrl", backend.GetEndpoint("https"));
backend.WithEnvironment("FrontendUrl", frontend.GetEndpoint("http"));

builder.Build().Run();
