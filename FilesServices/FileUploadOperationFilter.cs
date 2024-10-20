using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace FileService.API
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formFileParams = context.MethodInfo
                .GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile))
                .ToList();

            if (formFileParams.Any())
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                            {
                                { "file", new OpenApiSchema { Type = "string", Format = "binary" } },
                                { "username", new OpenApiSchema { Type = "string" } },
                                { "mobile", new OpenApiSchema { Type = "string" } },
                                { "email", new OpenApiSchema { Type = "string" } }
                            },
                                Required = new HashSet<string> { "file" }
                            }
                        }
                    }
                };
            }
        }

    }
}