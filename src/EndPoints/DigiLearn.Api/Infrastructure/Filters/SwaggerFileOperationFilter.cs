using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DigiLearn.Api.Infrastructure.Filters
{
  /// <summary>
  /// Swagger operation filter to properly document file upload endpoints
  /// Converts IFormFile properties to file upload parameters in Swagger UI
  /// </summary>
  public class SwaggerFileOperationFilter : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      var formFileParameters = context.MethodInfo.GetParameters()
          .Where(p => p.ParameterType == typeof(IFormFile) ||
                     p.ParameterType == typeof(IFormFile[]) ||
                     HasFormFileProperty(p.ParameterType))
          .ToList();

      if (!formFileParameters.Any())
        return;

      // Clear existing parameters
      operation.Parameters?.Clear();

      var requestBody = new OpenApiRequestBody
      {
        Content = new Dictionary<string, OpenApiMediaType>
        {
          ["multipart/form-data"] = new OpenApiMediaType
          {
            Schema = new OpenApiSchema
            {
              Type = "object",
              Properties = new Dictionary<string, OpenApiSchema>(),
              Required = new HashSet<string>()
            }
          }
        }
      };

      foreach (var parameter in context.MethodInfo.GetParameters())
      {
        AddPropertiesToSchema(parameter.ParameterType,
            requestBody.Content["multipart/form-data"].Schema,
            parameter.GetCustomAttribute<RequiredAttribute>() != null);
      }

      operation.RequestBody = requestBody;
    }

    private bool HasFormFileProperty(Type type)
    {
      return type.GetProperties()
          .Any(p => p.PropertyType == typeof(IFormFile) || p.PropertyType == typeof(IFormFile[]));
    }

    private void AddPropertiesToSchema(Type type, OpenApiSchema schema, bool isRequired)
    {
      foreach (var property in type.GetProperties())
      {
        var propertyName = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
        var propertyType = property.PropertyType;

        if (propertyType == typeof(IFormFile))
        {
          schema.Properties[propertyName] = new OpenApiSchema
          {
            Type = "string",
            Format = "binary",
            Description = GetPropertyDescription(property)
          };

          if (property.GetCustomAttribute<RequiredAttribute>() != null)
          {
            schema.Required.Add(propertyName);
          }
        }
        else if (propertyType == typeof(IFormFile[]))
        {
          schema.Properties[propertyName] = new OpenApiSchema
          {
            Type = "array",
            Items = new OpenApiSchema
            {
              Type = "string",
              Format = "binary"
            },
            Description = GetPropertyDescription(property)
          };

          if (property.GetCustomAttribute<RequiredAttribute>() != null)
          {
            schema.Required.Add(propertyName);
          }
        }
        else if (propertyType == typeof(string))
        {
          schema.Properties[propertyName] = new OpenApiSchema
          {
            Type = "string",
            Description = GetPropertyDescription(property)
          };

          if (property.GetCustomAttribute<RequiredAttribute>() != null)
          {
            schema.Required.Add(propertyName);
          }
        }
        else if (propertyType == typeof(int) || propertyType == typeof(int?))
        {
          schema.Properties[propertyName] = new OpenApiSchema
          {
            Type = "integer",
            Format = "int32",
            Description = GetPropertyDescription(property)
          };

          if (property.GetCustomAttribute<RequiredAttribute>() != null && propertyType == typeof(int))
          {
            schema.Required.Add(propertyName);
          }
        }
        else if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
        {
          schema.Properties[propertyName] = new OpenApiSchema
          {
            Type = "string",
            Format = "uuid",
            Description = GetPropertyDescription(property)
          };

          if (property.GetCustomAttribute<RequiredAttribute>() != null && propertyType == typeof(Guid))
          {
            schema.Required.Add(propertyName);
          }
        }
        else if (propertyType.IsEnum)
        {
          schema.Properties[propertyName] = new OpenApiSchema
          {
            Type = "string",
            Enum = Enum.GetNames(propertyType)
                  .Select(name => new Microsoft.OpenApi.Any.OpenApiString(name))
                  .Cast<Microsoft.OpenApi.Any.IOpenApiAny>()
                  .ToList(),
            Description = GetPropertyDescription(property)
          };

          if (property.GetCustomAttribute<RequiredAttribute>() != null)
          {
            schema.Required.Add(propertyName);
          }
        }
      }
    }

    private string GetPropertyDescription(PropertyInfo property)
    {
      var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
      var description = displayAttribute?.Description ?? displayAttribute?.Name ?? property.Name;

      var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
      if (requiredAttribute != null)
      {
        description += " (Required)";
      }

      return description;
    }
  }
}
