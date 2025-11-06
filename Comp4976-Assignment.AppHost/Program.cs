using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server database with persistent volume
var sqlServerDb = builder.AddSqlServer("theserver")
                         .WithEnvironment("ACCEPT_EULA", "Y")
                         .WithDataVolume("sqlserver_data") // Persist data across restarts
                         .AddDatabase("sqldata");

// Add the backend (ObituaryApp) project
var backend = builder.AddProject<Projects.ObituaryApp>("backend")
                     .WithReference(sqlServerDb); // backend depends on SQL Server

// Add the frontend (Blazor WebAssembly) project
var frontend = builder.AddProject<Projects.frontend>("frontend")
                      .WithReference(backend); // frontend depends on backend

// Pass backend URL to frontend and frontend URL to backend for CORS
frontend.WithEnvironment("BackendUrl", backend.GetEndpoint("https"));
backend.WithEnvironment("FrontendUrl", frontend.GetEndpoint("http"));

// OLD SQLite Configuration (commented out for reference)
// var backend = builder.AddProject<Projects.ObituaryApp>("backend");
// var frontend = builder.AddProject<Projects.frontend>("frontend")
//                       .WithReference(backend);

builder.Build().Run();
