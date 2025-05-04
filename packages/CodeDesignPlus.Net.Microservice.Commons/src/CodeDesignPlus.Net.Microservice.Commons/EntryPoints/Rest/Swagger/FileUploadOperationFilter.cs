using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Operation filter to handle file uploads in Swagger UI.
/// </summary>
public class FileUploadOperationFilter : IOperationFilter
{
    /// <summary>
    /// Applies the file upload operation filter to the specified OpenApiOperation.
    /// </summary>
    /// <param name="operation">The OpenApiOperation to apply the filter to.</param>
    /// <param name="context">The context of the operation filter.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParameters = context.MethodInfo.GetParameters().Where(p => p.ParameterType == typeof(IFormFile));

        foreach (var parameter in fileParameters)
        {
            operation.Parameters.Remove(operation.Parameters.First(p => p.Name == parameter.Name));
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                        {
                            [parameter.Name] = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            }
                        }
                        }
                    }
                }
            };
        }
    }
}