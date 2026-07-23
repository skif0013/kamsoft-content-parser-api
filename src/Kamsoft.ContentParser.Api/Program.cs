using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>

var app = builder.Build();

app.UseStaticFiles();

app.MapControllers();

app.Run();

