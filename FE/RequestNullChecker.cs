using System.Reflection;
using System.Runtime.CompilerServices;
using FastEndpoints;
using FluentValidation.Results;

namespace FE;

/// <summary>
/// Stops clients from posting null values directly when properties are marked as required.
/// </summary>
public class RequestNullChecker : IGlobalPreProcessor
{
    public async Task PreProcessAsync(IPreProcessorContext ctx, CancellationToken ct)
    {
        if (ctx.Request is null)
            return;

        var unallowedNullFailures = ctx.Request.GetType().GetProperties()
            .Where(x => x.GetCustomAttribute<RequiredMemberAttribute>() is not null)
            .Where(x => x.GetValue(ctx.Request) is null)
            .Select(x => new ValidationFailure(x.Name, $"{x.Name} is a required property. It can't be deserialzed to null."))
            .ToList();

        if (unallowedNullFailures.Any())
            await ctx.HttpContext.Response.SendErrorsAsync(unallowedNullFailures);
    }
}