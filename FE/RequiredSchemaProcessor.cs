using System.Reflection;
using System.Runtime.CompilerServices;
using NJsonSchema.Generation;

namespace FE;

/// <summary>
/// Handles turning the c# required keyword into actual required properties in the generated swagger schema.
/// </summary>
public class RequiredSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var requiredProps = context.ContextualType.Type.GetProperties()
            .Where(x => x.GetCustomAttribute<RequiredMemberAttribute>() is not null)
            .ToList();

        var requiredJsonProps = context.Schema.Properties
            .Where(j => requiredProps.Any(p => p.Name == ToPascalCase(j.Key)))
            .ToList();

        foreach (var requiredJsonProp in requiredJsonProps)
        {
            requiredJsonProp.Value.IsRequired = true;
            requiredJsonProp.Value.IsNullableRaw = false;
        }
    }

    public static string ToPascalCase(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        // Ensure the first character is uppercase; no changes to the rest of the string.
        return char.ToUpper(str[0]) + str.Substring(1);
    }
}