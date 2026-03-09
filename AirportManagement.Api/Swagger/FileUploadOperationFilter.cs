using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AirportManagement.Api.Swagger
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) || p.ParameterType == typeof(IFormFile[]))
                .ToArray();

            if (!fileParameters.Any())
                return;

            var properties = fileParameters.ToDictionary(
                p => p.Name,
                p => p.ParameterType == typeof(IFormFile)
                    ? new OpenApiSchema { Type = "string", Format = "binary" }
                    : new OpenApiSchema { Type = "array", Items = new OpenApiSchema { Type = "string", Format = "binary" } }
            );

            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = properties
                        }
                    }
                }
            };
        }
    }
}

