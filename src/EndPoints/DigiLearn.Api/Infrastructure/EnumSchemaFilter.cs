using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DigiLearn.Api.Infrastructure
{
  public class EnumSchemaFilter : ISchemaFilter
  {
    //public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    //{
    //  if (schema?.Enum?.Count > 0)
    //  {
    //    var enumNames = context.Type.GetEnumNames();
    //    schema.Enum.Clear();
    //    foreach (var enumName in enumNames)
    //    {
    //      schema.Enum.Add(OpenApiAnyFactory.CreateFromJson($"\"{enumName}\""));
    //    }
    //  }
    //}
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
      if (context.Type.IsEnum)
      {
        schema.Type = "string";
        schema.Enum = Enum.GetNames(context.Type)
            .Select(name => new OpenApiString(name))
            .ToList<IOpenApiAny>();
      }
    }
  }
}
