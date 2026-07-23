using System.Text.Json;
using System.Text.Json.Serialization;
using Kamsoft.ContentParser.Api.Domain.Interfaces;
using Kamsoft.ContentParser.Api.Services;
using Kamsoft.ContentParser.Api.Services.Parsers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IContentParser, CsvContentParser>();
builder.Services.AddSingleton<IContentParser, InternalJsonContentParser>();
builder.Services.AddSingleton<IContentParserStrategyFactory, ContentParserStrategyFactory>();
builder.Services.AddScoped<ParseService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Content Parser API",
        Version = "v1",
        Description = "Generic content parser for CSV and INTERNAL_JSON formats"
    });
});

var app = builder.Build();

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Content Parser API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Content Parser API";
    options.DefaultModelsExpandDepth(-1);
    options.HeadContent = "<link rel='stylesheet' type='text/css' href='/css/swagger-dark.css'>";
});

app.MapControllers();

app.Run();

public partial class Program { }
