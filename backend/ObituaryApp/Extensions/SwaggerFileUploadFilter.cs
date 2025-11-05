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
