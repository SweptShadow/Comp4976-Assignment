# Upload Controller Swagger Fix

## Overview

The `UploadController.UploadPhoto` endpoint was causing a `SwaggerGeneratorException` that prevented the entire Swagger UI from loading. The issue occurred because Swashbuckle (the Swagger library) does not natively support generating OpenAPI documentation for `IFormFile` parameters combined with the `[FromForm]` attribute.

This document outlines all changes made to resolve the issue.

## Problem

When navigating to `https://localhost:7269/swagger/index.html`, the following error was displayed:

```
SwaggerGeneratorException
Failed to generate Operation for action - ObituaryApp.Controllers.UploadController.UploadPhoto (ObituaryApp).
Error reading parameter(s) for action as [FromForm] attribute used with IFormFile.
```

This prevented the entire Swagger API documentation from loading.

## Solution

A custom Swagger operation filter was implemented to properly handle `IFormFile` parameters, and the `[FromForm]` attribute was removed from the method signature.

## Files Modified

### 1) Created: `backend/ObituaryApp/Extensions/SwaggerFileUploadFilter.cs`

**Purpose**: Custom Swagger operation filter that handles file upload endpoints properly.

**Content**:
```csharp
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ObituaryApp.Extensions
{
    /// <summary>
    /// Custom Swagger operation filter to handle IFormFile parameters in file upload endpoints.
    /// This fixes the Swashbuckle issue where [FromForm] with IFormFile causes generation errors.
    /// </summary>
    public class SwaggerFileUploadFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formFileParams = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) || 
                           (p.ParameterType.IsGenericType && 
                            p.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                            p.ParameterType.GetGenericArguments()[0] == typeof(IFormFile)))
                .ToList();

            if (!formFileParams.Any())
                return;

            // Clear auto-generated parameters for file uploads
            operation.Parameters.Clear();

            // Add file parameter with proper OpenAPI schema
            foreach (var formFileParam in formFileParams)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = formFileParam.Name,
                    In = ParameterLocation.Query,
                    Description = "File to upload",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    }
                });
            }

            // Update request body to use form data
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "multipart/form-data",
                        new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = formFileParams.ToDictionary(
                                    p => p.Name ?? "file",
                                    p => new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    }
                                ),
                                Required = formFileParams.Select(p => p.Name ?? "file").ToHashSet()
                            }
                        }
                    }
                },
                Required = true
            };
        }
    }
}
```

### 2) Modified: `backend/ObituaryApp/Program.cs`

**Change**: Registered the custom Swagger operation filter in the Swagger configuration.

**Before**:
```csharp
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
```

**After**:
```csharp
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
```

### 3) Modified: `backend/ObituaryApp/Controllers/UploadController.cs`

**Change**: Removed the `[FromForm]` attribute from the `UploadPhoto` method parameter.

**Before**:
```csharp
public async Task<IActionResult> UploadPhoto([FromForm] IFormFile file)
```

**After**:
```csharp
public async Task<IActionResult> UploadPhoto(IFormFile file)
```

## Why This Works

1. **Custom Operation Filter**: The `SwaggerFileUploadFilter` intercepts the Swagger generation process for any endpoint with `IFormFile` parameters.

2. **Proper Schema Generation**: Instead of letting Swashbuckle's default parameter handling try to process the `IFormFile`, the filter generates the correct OpenAPI schema for multipart/form-data uploads.

3. **Removed Attribute Conflict**: By removing `[FromForm]`, ASP.NET Core still correctly binds the file from the request (it's smart about file uploads), but Swashbuckle no longer encounters the unsupported attribute combination that was causing the error.

## Verification

After making these changes, navigate to:

```
https://localhost:7269/swagger/index.html
```

The Swagger UI should load successfully, and the `/api/Upload/photo` endpoint should display with the proper file upload schema.

## Additional Notes

- The `SwaggerFileUploadFilter` handles both single `IFormFile` and `IEnumerable<IFormFile>` parameters.
- The filter is automatically applied to all file upload endpoints in the API.
- No functional changes were made to the upload logic itself; only the Swagger documentation generation was fixed.
