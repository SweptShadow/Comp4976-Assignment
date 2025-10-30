using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add the backend (ObituaryApp) project
var backend = builder.AddProject<Projects.ObituaryApp>("backend");

// Add the frontend (Blazor WebAssembly) project
builder.AddProject<Projects.frontend>("frontend")
       .WithReference(backend); // frontend depends on backend

builder.Build().Run();
