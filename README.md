# COMP 4976 - Web App Development Assignment

## Project Overview
This is an ASP.NET Core application featuring:
- **Backend**: ASP.NET Core MVC + RESTful Web API with JWT authentication
- **Frontend**: Blazor WebAssembly (Separate project)
- **Database**: SQL Server 2022 in Docker (migrated from SQLite)
- **Orchestration**: .NET Aspire for local development
- **Deployment**: Azure App Service with CI/CD pipelines

## 🚀 Major Update: SQLite → SQL Server Migration

The application has been **successfully migrated from SQLite to SQL Server 2022** running in Docker with .NET Aspire orchestration!

### Key Improvements
✅ Enterprise-grade SQL Server database  
✅ Data persistence across application restarts  
✅ Docker containerization for consistent environments  
✅ .NET Aspire orchestration for service management  
✅ Transient fault handling with retry logic  
✅ Complete documentation of migration process  

**For detailed migration information**, see: [`SqliteToSQLServer.md`](./SqliteToSQLServer.md)

### Quick Start (with SQL Server)

1. **Ensure Docker Desktop is running**

2. **Start the Aspire AppHost**:
   ```powershell
   cd Comp4976-Assignment.AppHost
   dotnet run
   ```

3. **Access services**:
   - **Aspire Dashboard**: https://localhost:17132
   - **Backend API**: Visible in Aspire dashboard
   - **Frontend**: Visible in Aspire dashboard

4. **Data is persistent**: Changes survive Aspire restarts! ✅

### Database Access

**Using Azure Data Studio** (Recommended):
1. Download: https://learn.microsoft.com/en-us/azure-data-studio/
2. Connection string:
   ```
   Server=localhost,<port>;Database=sqldata;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True
   ```
3. Find the port in Aspire dashboard or via `docker ps`

## Project Structure
```
Comp4976-Assignment/
├── README.md
├── SqliteToSQLServer.md                          # ← Migration documentation
├── DOTNETASPIRE.md                               # ← Aspire setup guide
├── Comp4976-Assignment.sln
│
├── Comp4976-Assignment.AppHost/                  # ← Aspire orchestration (NEW)
│   ├── Program.cs                                # Defines services & dependencies
│   ├── appsettings.json
│   └── Comp4976-Assignment.AppHost.csproj
│
├── Comp4976-Assignment.ServiceDefaults/          # ← Shared service config (NEW)
│   └── Extensions.cs
│
├── backend/
│   └── ObituaryApp/                              # ASP.NET Core Backend
│       ├── ObituaryApp.csproj
│       ├── Program.cs                            # Now configured for SQL Server
│       ├── appsettings.json                      # SQL Server connection string
│       ├── appsettings.Production.json
│       ├── Controllers/
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   └── SeedData.cs
│       ├── Extensions/
│       │   └── SwaggerFileUploadFilter.cs        # ← File upload Swagger fix
│       ├── Migrations/                           # ← Fresh SQL Server migration
│       ├── Models/
│       ├── Services/
│       ├── Views/
│       └── wwwroot/
│
└── frontend/                                     # Blazor WebAssembly Frontend
    ├── frontend.csproj
    ├── Program.cs
    ├── App.razor
    ├── _Imports.razor
    ├── Auth/
    ├── Layout/
    ├── Models/
    ├── Pages/
    ├── Services/
    └── wwwroot/
```

## Development Progress

### Phase 1: Backend Foundation ✅ COMPLETED
- [x] Created solution file (`Comp4976-Assignment.sln`)
- [x] Created ASP.NET Core MVC project with .NET 9.0
- [x] Installed all required NuGet packages (EF Core, Identity, JWT, Swagger)
- [x] Created database models (Obituary, ApplicationUser)
- [x] Configured Entity Framework Core with SQLite
- [x] Implemented ASP.NET Core Identity authentication
- [x] Added JWT Bearer authentication for API security
- [x] Scaffolded Identity account pages (Register, Login, Logout)
- [x] Implemented JwtService for token generation
- [x] Added SeedData for initial roles and test users
- [x] Created full CRUD API endpoints
- [x] Implemented authorization (creator/admin only for edit/delete)
- [x] Added photo upload handling
- [x] Deployed to Azure App Service
- [x] Set up CI/CD pipelines with Azure DevOps

### Phase 2: Database Migration (SQLite → SQL Server) ✅ COMPLETED
- [x] Created Aspire AppHost project for service orchestration
- [x] Configured SQL Server 2022 Docker image in Aspire
- [x] Updated backend dependencies (removed SQLite, kept SQL Server)
- [x] Modified Program.cs to use SQL Server with `UseSqlServer()`
- [x] Added connection string fallback logic (tries "sqldata", then "DefaultConnection")
- [x] Implemented transient fault handling with `EnableRetryOnFailure()`
- [x] Enhanced database initialization logging
- [x] Removed all SQLite migrations
- [x] Created fresh SQL Server migration with proper data types
- [x] **Added Docker persistent volume for data persistence** ✅
- [x] Fixed data loss issue (data now survives Aspire restarts)
- [x] Updated appsettings.json and appsettings.Production.json
- [x] Created comprehensive migration documentation (`SqliteToSQLServer.md`)
- [x] Verified all changes with successful build (4.7s)
- [x] Tested data persistence across multiple restart cycles

### Phase 3: Frontend Development 🔄 IN PROGRESS
- [x] Created Blazor WebAssembly project
- [x] Implemented authentication pages (Login, Register)
- [x] Created obituary listing and details pages
- [x] Integrated with backend API endpoints
- [ ] Complete all frontend pages
- [ ] Deploy frontend to Azure Static Web Apps

## Requirements Progress (Backend)

### Assignment 1 - Backend (ASP.NET Core MVC + Web API)

**Core Functionality** ✅ COMPLETE
- ✅ User authentication (register, login, logout) with ASP.NET Identity
- ✅ Database with Entity Framework Core (Now: SQL Server with migrations)
- ✅ Obituary model with required fields (name, DOB, DOD, biography, photo)
- ✅ Full CRUD API endpoints (Create, Read, Update, Delete)
- ✅ Authorization (edit/delete restricted to creator or admin)
- ✅ Photo upload handling (server-side storage)
- ✅ Swagger/OpenAPI documentation for API
- ✅ JWT authentication for API security

**Database** ✅ MIGRATED & ENHANCED
- ✅ ~~SQLite~~ → **SQL Server 2022** (Docker-based)
- ✅ EF Core migrations (proper SQL Server schema types)
- ✅ Connection resilience with retry logic
- ✅ Data persistence with Docker volumes

**Deployment** ✅ COMPLETE
- ✅ Azure App Service deployment
- ✅ CI/CD pipelines with Azure DevOps
- ✅ Automated testing and deployment

### Assignment 2 - Frontend (Blazor WebAssembly)

**In Progress** 🔄
- ✅ Blazor WebAssembly project created
- ✅ Authentication integration with backend JWT
- ✅ Obituary listing page with pagination
- ✅ Obituary details page
- ✅ Create/Edit obituary functionality
- [ ] Delete confirmation dialog
- [ ] Search and filtering
- [ ] File upload UI for photos
- [ ] Deploy to Azure Static Web Apps

## Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| **Framework** | .NET / ASP.NET Core | 9.0 |
| **Backend** | ASP.NET Core MVC + Web API | Latest |
| **Frontend** | Blazor WebAssembly | Latest |
| **Database** | SQL Server | 2022 (Docker) |
| **Authentication** | ASP.NET Core Identity + JWT | Built-in |
| **ORM** | Entity Framework Core | 9.0.9 |
| **Orchestration** | .NET Aspire | 8.2.2 |
| **API Docs** | Swagger/OpenAPI | Swashbuckle |
| **Deployment** | Azure App Service | Cloud |

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────┐
│                   .NET Aspire AppHost                    │
│              (Service Orchestration & Management)        │
└──────────────────┬──────────────────────────────────────┘
                   │
        ┌──────────┼──────────┐
        │          │          │
        ▼          ▼          ▼
    ┌────────┐  ┌────────┐  ┌──────────────┐
    │Frontend│  │Backend │  │SQL Server    │
    │ (WASM) │  │ (MVC)  │  │  (Docker)    │
    │Blazor │  │ + API  │  │   + Volume   │
    └────────┘  └────────┘  └──────────────┘
        │          │              │
        └──────────┴──────────────┘
              JWT Auth
              API Calls
         Database Access
```

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Docker Desktop (running)
- Visual Studio 2022 or VS Code with C# extensions
- Optional: Azure Data Studio for database management

### Installation

```bash
# Clone the repository
git clone <repository-url>
cd Comp4976-Assignment

# Restore NuGet packages
dotnet restore
```

### Running the Application

#### Option 1: With Aspire (SQL Server + Orchestration) - RECOMMENDED
```powershell
# Navigate to AppHost
cd Comp4976-Assignment.AppHost

# Run the Aspire orchestration
dotnet run

# Then open: https://localhost:17132
# All services (backend, frontend, SQL Server) managed automatically!
```

#### Option 2: Backend Only (ASP.NET Core)
```bash
cd backend/ObituaryApp

# Run once
dotnet run

# Or run in watch mode (recommended for development)
dotnet watch run
```

The application will be available at: https://localhost:5001

### Database Management

**Viewing SQL Server data** with Azure Data Studio:
1. Download: https://learn.microsoft.com/en-us/azure-data-studio/
2. Get SQL Server port from Aspire dashboard or `docker ps`
3. Connection: `localhost,<port>` | User: `sa` | Password: `YourStrong!Passw0rd`
4. Database: `sqldata`

**Manage Docker volumes**:
```powershell
# List all volumes
docker volume ls

# View volume details
docker volume inspect sqlserver_data

# Remove volume (careful - loses all data!)
docker volume rm sqlserver_data
```

## 🌐 Main Endpoints

### With Aspire (Recommended)
- **Aspire Dashboard**: https://localhost:17132 (service management)
- **Backend API**: Click "backend" service in Aspire dashboard
- **Frontend**: Click "frontend" service in Aspire dashboard

### Backend Services (Direct Access)
Replace `<port>` with actual port from `docker ps` or Aspire dashboard:

- [Swagger UI](http://localhost:<port>/swagger) - API explorer
- [Login Page](http://localhost:<port>/Identity/Account/Login) - Identity authentication
- [Register Page](http://localhost:<port>/Identity/Account/Register) - New user signup
- [Obituaries List](http://localhost:<port>/Obituaries) - MVC view
- [Obituaries API](http://localhost:<port>/api/Obituaries) - RESTful API

### Example API Calls

**Get all obituaries**:
```bash
curl https://localhost:5001/api/Obituaries
```

**Create obituary** (requires JWT token):
```bash
curl -X POST https://localhost:5001/api/Obituaries \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <JWT_TOKEN>" \
  -d '{
    "fullName": "John Doe",
    "dateOfBirth": "1950-01-15",
    "dateOfDeath": "2024-11-05",
    "biography": "Beloved community member..."
  }'
```

## Important Notes

✅ **Data Persistence**: All data saved to obituaries persists across Aspire restarts thanks to Docker persistent volumes  
✅ **Authentication**: Both ASP.NET Core Identity (MVC) and JWT (API) are configured  
✅ **CORS**: Configured to allow frontend access to backend API  
✅ **Swagger**: Includes JWT Bearer token support for testing API endpoints  

## Troubleshooting

### "Connection refused" errors
- Ensure Docker Desktop is running
- Check `docker ps` to verify SQL Server container is up
- Wait 10-15 seconds for SQL Server to fully start

### "Database does not exist"
- First backend connection triggers automatic migration
- Check Aspire logs for migration status
- Verify persistent volume: `docker volume ls`

### "Port already in use"
- Aspire auto-assigns ports to avoid conflicts
- Check current port in Aspire dashboard
- Or stop all containers: `docker stop $(docker ps -a -q)`

## Documentation

- 📖 **SQLite to SQL Server Migration**: [`SqliteToSQLServer.md`](./SqliteToSQLServer.md)
- 📖 **.NET Aspire Setup**: [`DOTNETASPIRE.md`](./DOTNETASPIRE.md)

## Project Assignment Scope

- **Assignment 1** ✅: Backend (ASP.NET Core MVC + Web API + Identity + EF Core + JWT) + Azure deployment
- **Assignment 2** 🔄: Blazor WebAssembly frontend consuming the API

---

**Last updated**: November 5, 2025  
**Database**: SQL Server 2022 (Docker)  
**Framework**: .NET 9.0 with Aspire 8.2.2


Note: Some endpoints (Create, Edit, Delete) require authentication. Use the seeded account to test.

---
*Last updated: October 10, 2025*