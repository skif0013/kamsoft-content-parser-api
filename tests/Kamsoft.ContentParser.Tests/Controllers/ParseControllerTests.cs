using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Kamsoft.ContentParser.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Kamsoft.ContentParser.Tests.Controllers;

public class ParseControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ParseControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private static string ToBase64(string text) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

    [Fact]
    public async Task ParseCSV_ReturnsSuccess()
    {
        var csv = "Name,Age\nJohn,30\nJane,25";
        var payload = new { type = "CSV", content = ToBase64(csv) };

        var response = await _client.PostAsJsonAsync("/api/v1/parse-content", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ParseResponse>();
        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
        body.RowCount.Should().Be(2);
    }

    [Fact]
    public async Task ParseINTERNAL_JSON_ReturnsSuccess()
    {
        var json = """[{"name":"John","age":"30"}]""";
        var payload = new { type = "INTERNAL_JSON", content = ToBase64(json) };

        var response = await _client.PostAsJsonAsync("/api/v1/parse-content", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ParseResponse>();
        body!.RowCount.Should().Be(1);
    }

    [Fact]
    public async Task EmptyContent_ReturnsBadRequest()
    {
        var payload = new { type = "CSV", content = "" };

        var response = await _client.PostAsJsonAsync("/api/v1/parse-content", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvalidBase64_ReturnsBadRequest()
    {
        var payload = new { type = "CSV", content = "!!!not-base64!!!" };

        var response = await _client.PostAsJsonAsync("/api/v1/parse-content", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvalidCSVContent_ReturnsBadRequest()
    {
        var payload = new { type = "CSV", content = ToBase64("Name,Age") };

        var response = await _client.PostAsJsonAsync("/api/v1/parse-content", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvalidJSONContent_ReturnsBadRequest()
    {
        var payload = new { type = "INTERNAL_JSON", content = ToBase64("{invalid") };

        var response = await _client.PostAsJsonAsync("/api/v1/parse-content", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
