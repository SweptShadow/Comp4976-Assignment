# COMP 4976 - Web App Development Assignment

## Project Overview
This is an ASP.NET Core MVC application for managing obituaries.

## Project Structure
```
Comp4976-Assignment/
├── README.md
├── Comp4976-Assignment.sln
└── backend/
    └── ObituaryApp/
        ├── ObituaryApp.csproj
        ├── Program.cs
        ├── Controllers/
        ├── Data/
        ├── Extensions/
        ├── Migrations/
        ├── Models/
        ├── Views/
        └── wwwroot/
```

## Development Progress

### Completed Tasks
- [x] Created solution file (`Comp4976-Assignment.sln`)
- [x] Created ASP.NET Core MVC project (`ObituaryApp`) with .NET 9.0
  - Command used: `dotnet new mvc -n ObituaryApp -f net9.0` (run from `/backend` directory)
- [x] Project structure initialized with standard MVC folders
- [x] Installed NuGet packages for Entity Framework, Identity, JWT, and API documentation
  - Commands executed from `/backend/ObituaryApp` directory:
    ```bash
    dotnet add package Microsoft.EntityFrameworkCore.Sqlite
    dotnet add package Microsoft.EntityFrameworkCore.Tools
    dotnet add package Microsoft.EntityFrameworkCore.Design
    dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
    dotnet add package Microsoft.AspNetCore.Identity.UI
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
    dotnet add package System.IdentityModel.Tokens.Jwt
    dotnet add package Swashbuckle.AspNetCore
    dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson
    dotnet restore
    ```
- [x] Installed ASP.NET code generator tool globally
  - Command used: `dotnet tool install -g dotnet-aspnet-codegenerator`
- [x] Added package for code generation design
  - Command used: `dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design`
- [x] Built the entire solution successfully
  - Command used: `dotnet build` (run from solution root directory)
- [x] Created Obituary model in `Models/Obituary.cs`
  - Implements required fields: Full name, Date of birth, Date of death, Biography/tribute text, Photo upload
  - Includes data annotations for validation and display names
  - Added audit fields (CreatedBy, CreatedDate, ModifiedDate)
- [x] Created ApplicationUser model in `Models/ApplicationUser.cs`
  - Extends IdentityUser with FirstName and LastName properties
  - Includes computed FullName property
- [x] Created ApplicationDbContext in `Data/ApplicationDbContext.cs`
  - Inherits from IdentityDbContext for user management
  - Configured Obituaries DbSet for database table
  - Added Fluent API configurations for entity properties
- [x] Configured services in `Program.cs`
  - Set up Entity Framework with SQLite database connection
  - Configured ASP.NET Core Identity with ApplicationUser
  - Added JWT Bearer authentication with token validation
  - Integrated Swagger/OpenAPI documentation with JWT support
  - Added Newtonsoft.Json support for controllers
- [x] Implemented JWT service in `Services/JwtService.cs` and interface in `Services/IJwtService.cs`
  - Provides method to generate JWT tokens for authenticated users
  - Uses configuration from `appsettings.json` for secret, issuer, audience, and expiry
  - Injected via DI for use in controllers and authentication flows
- [x] Created SeedData class in `Data/SeedData.cs`
  - Seeds initial roles (admin, user) and test users into the database
  - Ensures database is created and ready for authentication
  - Can be called at startup to initialize data for development/testing
- [x] Updated `Program.cs` to:
  - Register `IJwtService` and `JwtService` for dependency injection
  - Call `SeedData.Initialize` at startup to seed roles and users
  - Finalize authentication, authorization, and Swagger setup
- [x] Updated the database using Entity Framework Core migrations
  - Command used: `dotnet ef database update` (run from `/backend/ObituaryApp` directory)
- [x] Scaffolded Identity account pages (Register, Login, Logout) using ASP.NET code generator
  - Command used: `dotnet aspnet-codegenerator identity -dc ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout" -f`
  - Added pages in `Areas/Identity/Pages/Account`
- [x] Fixed authentication scheme conflict in `Program.cs`
  - Removed global JWT default scheme override
  - Documented correct setup for Identity (cookie) and JWT (API) authentication
  - Added comments to clarify usage and prevent future errors
- [x] Image upload handling implemented (server-side): uploaded photos are saved to `wwwroot/uploads` and `PhotoPath` stores the relative path; Create/Edit/Details views updated accordingly

## Requirements Progress (Backend)

### Completed
- User authentication (register, login, logout) with ASP.NET Identity
- Database seeded with admin and user accounts
- Obituary model with required fields (name, DOB, DOD, biography, photo path)
- Identity UI integrated in layout (dynamic login/register/logout)
- JWT authentication configured for API security
- Solution/project structure and version control (GitHub) set up
- Implemented full CRUD for obituary entries (create, edit, delete)
- Restrict edit/delete to creator or admin
- Add obituary listing with pagination and search
- Complete RESTful API endpoints for CRUD and JSON responses
- Add photo upload handling

### In Progress / Next Steps
- ✅ Azure deployment completed! App is live at: https://obituary-app.azurewebsites.net
- Azure Blob Storage for production image storage (currently using local wwwroot/uploads)
- Azure DevOps CI/CD pipeline integration
- Double checking all requirements for Assignment 1
- Assignment 2: Blazor WebAssembly frontend (separate project)

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code

### Running the Application
```bash
# Navigate to the project directory
cd backend/ObituaryApp

# Run once
dotnet run

# Run in watch mode (recommended for development)
dotnet watch run
```

The application will be available at: http://localhost:5151

## 🌐 Main Endpoints (Port 5151)

You can reach these endpoints directly in your browser (replace with your actual port if different):

- [Home Page (MVC)](http://localhost:5151/)
- [Swagger UI (API Explorer)](http://localhost:5151/swagger)
- [Login Page (Identity)](http://localhost:5151/Identity/Account/Login)
- [Register Page (Identity)](http://localhost:5151/Identity/Account/Register)
- [Logout Page (Identity)](http://localhost:5151/Identity/Account/Logout)
- [Obituaries List (MVC)](http://localhost:5151/Obituaries)
- [Obituaries API (example)](http://localhost:5151/api/Obituaries)

### Scope
- **Assignment 1**: Backend (ASP.NET Core MVC + Web API + Identity + EF Core + JWT) + Azure deployment with CI/CD pipelines
- **Assignment 2**: Blazor WebAssembly frontend consuming the API


Note: Some endpoints (Create, Edit, Delete) require authentication. Use the seeded account to test.

---
*Last updated: October 10, 2025*