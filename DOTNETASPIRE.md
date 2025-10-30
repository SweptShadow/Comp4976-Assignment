# Quick Dev Setup — run, verify, and scaffold Aspire

Follow these steps to run the projects locally, verify they load, and install/ scaffold .NET Aspire (keep SQLite for now). All commands below are PowerShell-friendly and should be run from the solution root unless noted otherwise.

1) Backend (MVC / API)

  - Start the backend in watch mode:

```powershell
cd backend/ObituaryApp
dotnet watch
```

  - Verify:
    - Open the homepage (MVC) at http://localhost:5151/ (or the port shown in the output).
    - Open Swagger at http://localhost:5151/swagger and call at least one endpoint, e.g. GET /api/obituaries.

2) Frontend (Blazor WASM)

  - Start the frontend in watch mode (run in a new terminal):

```powershell
cd frontend
dotnet watch
```

  - Verify:
    - Open the browser to the address shown in the frontend `dotnet watch` output — your Blazor page should load (it can be static for now).

3) Docker Desktop

  - Open Docker Desktop once to make sure it’s installed and running. We’ll need Docker in Phase C.

Add .NET Aspire (Keep SQLite for now) ---------------------------------

1) Install Aspire workloads (any terminal)

```powershell
dotnet workload update
dotnet workload install aspire
dotnet workload list
```

You should see an entry like:

```
aspire    Installed    9.x.x
```

2) Scaffold Aspire projects in your solution root (the directory that contains `Comp4976-Assignment.sln`):

```powershell
dotnet new aspire --force
```

This will create the Aspire scaffold which includes (example names):

- YourSolution.AppHost/
- YourSolution.ServiceDefaults/
- YourSolution.sln   (the solution file will be updated/re-written)

Then confirm the solution builds:

```powershell
dotnet build
```

3) Add your existing projects to the new solution

Example commands (run from solution root):

```powershell
dotnet sln add ./frontend/frontend.csproj
dotnet sln add ./backend/ObituaryApp/ObituaryApp.csproj
```

Confirm build again:

```powershell
dotnet build
```

4) Align AppHost project settings

Open `Comp4976-Assignment.AppHost/Comp4976-Assignment.AppHost.csproj` and make these changes where appropriate:

- Change target framework to `net9.0`:

```xml
<TargetFramework>net9.0</TargetFramework>
```

- Add the Aspire AppHost SDK declaration under the top-level Project element (near the top of the file):

```xml
<Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />
```

- Optionally add a matching package reference for the AppHost runtime (keep the version consistent with the SDK):

```xml
<PackageReference Include="Aspire.Hosting.AppHost" Version="9.0.0" />
```

After these edits, run:

```powershell
dotnet build
```

5) Align ServiceDefaults project settings

Open `Comp4976-Assignment.ServiceDefaults/Comp4976-Assignment.ServiceDefaults.csproj` and ensure the target framework is set to `net9.0`:

```xml
<TargetFramework>net9.0</TargetFramework>
```

(If you already have `net9.0` in that file, no change is required.)

After editing, confirm build:

```powershell
dotnet build
```

5) In solution explorer, align ServiceDefault -- with frontend/backend versions: 


In Comp4976-Assignment.ServiceDefaults/Comp4976-Assignment.ServiceDefaults.csproj:

Change target: 
```xml
<TargetFramework>net9.0</TargetFramework>
```

** Confirm build success **
```powershell
dotnet build
```


6) Add references from AppHost to backend and frontend 
From the solution root, run:

A) Add backend reference
```powershell
dotnet add ./Comp4976-Assignment.AppHost/Comp4976-Assignment.AppHost.csproj reference ./backend/ObituaryApp/ObituaryApp.csproj
```

B) Add frontend reference
```powershell
dotnet add ./Comp4976-Assignment.AppHost/Comp4976-Assignment.AppHost.csproj reference ./frontend/frontend.csproj
```

C) Add ServiceDefaults reference to backend
```powershell
dotnet add ./backend/ObituaryApp/ObituaryApp.csproj reference ./Comp4976-Assignment.ServiceDefaults/Comp4976-Assignment.ServiceDefaults.csproj
```


** Confirm build success **
```powershell
dotnet build
```

7) Edit backend Program.cs (Entry point)

In `backend/ObituaryApp/Program.cs`, find the line `var app = builder.Build();` and add:

```csharp
// Add service defaults & Aspire components.
builder.AddServiceDefaults();
```

8) Configure Aspire AppHost (Project wiring) -- Tell Apsire how to start your backend and front end together, set up discover so your blazor app can read the API using http://backend/

In `Comp4976-Assignment.AppHost\Program.cs` (Apphost folder, Program.cs file), add:

```csharp
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add the backend (ObituaryApp) project
var backend = builder.AddProject<Projects.ObituaryApp>("backend");

// Add the frontend (Blazor WebAssembly) project
builder.AddProject<Projects.frontend>("frontend")
  .WithReference(backend); // frontend depends on backend

builder.Build().Run();
```

8) Run and verify (start Aspire)
** Make sure you have .NET 8.0.21 **

From your solution root (same folder as your .sln):

```powershell
cd "C:\Users\brian\Courses\Term-4\COMP-4976-Web-App-Dev(MS)\Assignments\Comp4976-Assignment1\Comp4976-Assignment\Comp4976-Assignment.AppHost"
dotnet watch
```

Match the formatting 