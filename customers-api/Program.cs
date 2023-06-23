using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.AddServer(new Microsoft.OpenApi.Models.OpenApiServer { Url = "/" }); // add absolute URL
// });

var app = builder.Build();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        var scheme = httpReq.Scheme;
        var host = httpReq.Host.Value;
        var basePath = httpReq.PathBase.Value.TrimEnd('/');
        var serverUrl = $"{scheme}://{host}{basePath}";
        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = serverUrl } };
    });
});
app.UseSwaggerUI();


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

var customers = new List<Customer>()
{
    new Customer(1, "John Doe"),
    new Customer(2, "Jane Doe"),
    new Customer(3, "John Smith"),
    new Customer(4, "Jane Smith")
};

app.MapGet("customers", () => 
{
    return customers;
})
.WithName("GetCustomers")
.WithDescription("Get all customers")
.WithOpenApi();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), ".well-known")),
    RequestPath = "/.well-known"
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record Customer(int Id, string Name);