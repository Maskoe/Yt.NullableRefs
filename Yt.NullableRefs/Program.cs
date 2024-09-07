var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(x => x.SchemaFilter<RequiredSchemaFilter>());

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/fullname", (FullNameReq req) =>
{
    var fullName = req.FirstName.ToUpper() + " " + req.LastName.ToUpper();

    return new FullNameRes { FullName = fullName };
}).WithOpenApi()
.AddEndpointFilter<EnsureRequiredProps>();

app.Run();

public class FullNameReq
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}

public class FullNameRes
{
    public required string FullName { get; set; }
}