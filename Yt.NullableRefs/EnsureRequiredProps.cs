using System.Reflection;
using System.Runtime.CompilerServices;

public class EnsureRequiredProps : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.FirstOrDefault();
        if (request is null)
            return await next(context);
 
        // Find all properties on the current type with the 'required' keyword
        // Find all values on the current request that are null
        // Transform into a Dictionary for ValidationProblem
 
        var nullFailures = request.GetType().GetProperties()
            .Where(x => x.GetCustomAttribute<RequiredMemberAttribute>() is not null)
            .Where(x => x.GetValue(request) is null)
            .ToDictionary(x => x.Name, x => new[] { $"{x.Name} is required. It can't be deserialized to null." });
 
        if (nullFailures.Any())
            return TypedResults.ValidationProblem(nullFailures);
 
        return await next(context);
    }
}