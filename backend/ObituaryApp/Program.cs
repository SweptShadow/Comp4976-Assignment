using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ObituaryApp.Data;
using ObituaryApp.Models;
using ObituaryApp.Services; // ← Uncomment when JwtService is created

var builder = WebApplication.CreateBuilder(args);

// Add CORS for Blazor frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "http://localhost:5001", "https://localhost:5001")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();

// Configure Entity Framework (Code First Database)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// 👆 "Use MY ApplicationDbContext with SQLite - connect to obituary.db file"

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
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // For SQLite, use EnsureCreated which is more reliable than Migrate
        context.Database.EnsureCreated();

        // Seed data
        await SeedData.Initialize(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
        // Continue startup even if seeding fails
    }
}

app.Run();