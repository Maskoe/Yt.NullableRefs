using FastEndpoints;
using FastEndpoints.Swagger;
using FE;

var bld = WebApplication.CreateBuilder();
bld.Services.AddFastEndpoints();
bld.Services.SwaggerDocument(options =>
{
    options.DocumentSettings = settings =>
    {
        settings.SchemaSettings.SchemaProcessors.Add(new RequiredSchemaProcessor());
    };
});

var app = bld.Build();

app.UseFastEndpoints(options =>
{
    options.Endpoints.Configurator = ep =>
    {
        ep.PreProcessor<RequestNullChecker>(Order.Before);
    };
});
app.UseSwaggerGen();

app.Run();

public class FullName : Endpoint<FullNameReq, FullNameRes>
{
    public override void Configure()
    {
        Post("/api/fullname");
        AllowAnonymous();
    }

    public override async Task<FullNameRes> ExecuteAsync(FullNameReq req, CancellationToken ct)
    {
        var fullName = req.FirstName.ToUpper() + " " + req.LastName.ToUpper();
        
        return new FullNameRes { FullName = fullName };
    }
}

public class FullNameReq
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int what { get; set; }
}

public class FullNameRes
{
    public required string FullName { get; set; }
}