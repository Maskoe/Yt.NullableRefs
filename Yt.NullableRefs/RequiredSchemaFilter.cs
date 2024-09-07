using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class RequiredSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Find all properties on the current type with the 'required' keyword
        var requiredClrProps = context.Type.GetProperties()
            .Where(x => x.GetCustomAttribute<RequiredMemberAttribute>() is not null)
            .ToList();
 
        // Find all the matching properties in the openAPI schema (adjust for case differences)
        var requiredJsonProps = schema.Properties
            .Where(j => requiredClrProps.Any(p => p.Name == j.Key || p.Name == ToPascalCase(j.Key)))
            .ToList();
 
        // Set properties as required
        schema.Required = requiredJsonProps.Select(x => x.Key).ToHashSet();
 
        // Optionally set them non nullable too.
        foreach (var requiredJsonProp in requiredJsonProps)
            requiredJsonProp.Value.Nullable = false;
    }
 
    private string ToPascalCase(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }
 

}