using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ObituaryApp.Data;
using ObituaryApp.Models;
using ObituaryApp.Services; // ← Uncomment when JwtService is created
using DotNetEnv;

// Load .env file if it exists
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Add CORS for Blazor frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        var origins = new List<string>
        {
            "http://localhost:5000",
            "http://localhost:5001",
            "https://localhost:5001",
            "http://localhost:5232",
            "https://localhost:5232"
        };

        var dynamicFrontend = builder.Configuration["FrontendUrl"];
        if (!string.IsNullOrWhiteSpace(dynamicFrontend))
        {
            origins.Add(dynamicFrontend.TrimEnd('/'));
        }

        policy.WithOrigins(origins.ToArray())
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();

// Configure Entity Framework with SQL Server (migrated from SQLite)
// NEW SQL Server configuration via Aspire with retry logic
// Get connection string from Aspire service defaults (injected as "sqldata")
// Fallback to "DefaultConnection" for local testing without Aspire
var sqlServerConnString = builder.Configuration.GetConnectionString("sqldata")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'sqldata' or 'DefaultConnection' not found.");

Console.WriteLine($"[Startup] Using SQL Server connection: {sqlServerConnString}");
Console.WriteLine($"[Startup] Environment: {builder.Environment.EnvironmentName}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(sqlServerConnString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    }));
// 👆 "Use MY ApplicationDbContext with SQL Server - connect via Aspire with retry logic"

// OLD SQLite Configuration (commented out for reference)
// var defaultConn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=obituary.db";
// var sqliteConn = defaultConn;
// try
// {
//     const string relativeToken = "Data Source=obituary.db";
//     if (defaultConn.Contains(relativeToken, StringComparison.OrdinalIgnoreCase))
//     {
//         var absPath = Path.Combine(builder.Environment.ContentRootPath, "obituary.db");
//         sqliteConn = $"Data Source={absPath}";
//     }
// }
// catch { }
// 
// Console.WriteLine($"[Startup] Using SQLite connection: {sqliteConn}");
// 
// builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(sqliteConn));

// Register Identity (cookie authentication for web app)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password requirements for user accounts
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()  // 👆 "Store users in MY database"
.AddDefaultUI()
.AddDefaultTokenProviders(); // Add default token providers so Identity can generate confirmation and reset tokens

// Add custom services
builder.Services.AddScoped<IJwtService, JwtService>();
// Blob storage service (Azure Blob). Provide configuration in appsettings.json under "AzureBlob:ConnectionString" and "AzureBlob:ContainerName".
builder.Services.AddSingleton<ObituaryApp.Services.IBlobService, ObituaryApp.Services.BlobService>();

// Add AI service for GitHub Models
builder.Services.AddHttpClient<IAiService, GitHubModelsAiService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register JWT authentication (for API endpoints)
// DO NOT override the default scheme globally, to avoid conflict with Identity
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong");

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        // JWT token validation rules
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
// Note: Use [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] on API controllers/actions to require JWT
// Identity (cookie) authentication will be used for MVC and Razor pages by default

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Obituary API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Add support for file upload parameters
    c.OperationFilter<ObituaryApp.Extensions.SwaggerFileUploadFilter>();
});

// Add custom services (will be created in Step 5)
builder.Services.AddScoped<IJwtService, JwtService>();  // ← Uncomment when JwtService is created

builder.AddServiceDefaults();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Obituary API v1"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable CORS before authentication
app.UseCors("AllowBlazor");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// To enable routing for Razor Pages (including Identity area pages), uncomment the next line:
app.MapRazorPages();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("[Startup] Attempting to migrate database...");
        context.Database.Migrate();
        logger.LogInformation("[Startup] Database migration completed successfully.");

        // Seed data
        logger.LogInformation("[Startup] Seeding database...");
        await SeedData.Initialize(scope.ServiceProvider);
        logger.LogInformation("[Startup] Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        var logger2 = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger2.LogError(ex, "[Startup] Error during database initialization. Application will continue but may not function properly.");
    }
}

// OLD SQLite initialization (commented out for reference)
// using (var scope = app.Services.CreateScope())
// {
//     try
//     {
//         var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//
//         // For SQLite, use EnsureCreated which is more reliable than Migrate
//         context.Database.EnsureCreated();
//
//         // Seed data
//         await SeedData.Initialize(scope.ServiceProvider);
//     }
//     catch (Exception ex)
//     {
//         var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//         logger.LogError(ex, "An error occurred while initializing the database.");
//         // Continue startup even if seeding fails
//     }
// }

// OLD SQLite initialization (commented out for reference)
// using (var scope = app.Services.CreateScope())
// {
//     try
//     {
//         var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//
//         // For SQLite, use EnsureCreated which is more reliable than Migrate
//         context.Database.EnsureCreated();
//
//         // Seed data
//         await SeedData.Initialize(scope.ServiceProvider);
//     }
//     catch (Exception ex)
//     {
//         var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//         logger.LogError(ex, "An error occurred while initializing the database.");
//         // Continue startup even if seeding fails
//     }
// }

app.Run();
