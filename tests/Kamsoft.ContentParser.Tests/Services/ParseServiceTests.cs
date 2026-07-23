using System.Text;
using FluentAssertions;
using Kamsoft.ContentParser.Api.Domain.Enums;
using Kamsoft.ContentParser.Api.Domain.Models;
using Kamsoft.ContentParser.Api.Services;

namespace Kamsoft.ContentParser.Tests.Services;

public class ParseServiceTests
{
    private readonly ParseService _sut;

    public ParseServiceTests()
    {
        var factory = new ContentParserStrategyFactory(new Api.Domain.Interfaces.IContentParser[]
        {
            new Api.Services.Parsers.CsvContentParser(),
            new Api.Services.Parsers.InternalJsonContentParser()
        });
        _sut = new ParseService(factory);
    }

    private static string ToBase64(string text) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

    [Fact]
    public void Parse_EmptyContent_ReturnsFailure()
    {
        var request = new ParseRequest { Type = ContentType.CSV, Content = "" };

        var result = _sut.Parse(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("required");
    }

    [Fact]
    public void Parse_InvalidBase64_ReturnsFailure()
    {
        var request = new ParseRequest { Type = ContentType.CSV, Content = "!!!not-base64!!!" };

        var result = _sut.Parse(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Base64");
    }

    [Fact]
    public void Parse_CSV_ReturnsSuccess()
    {
        var csv = "Name,Age\nJohn,30\nJane,25";
        var request = new ParseRequest { Type = ContentType.CSV, Content = ToBase64(csv) };

        var result = _sut.Parse(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Success.Should().BeTrue();
        result.Value.RowCount.Should().Be(2);
        result.Value.Data.Should().HaveCount(2);
        result.Value.Data[0]["Name"].Should().Be("John");
    }

    [Fact]
    public void Parse_INTERNAL_JSON_ReturnsSuccess()
    {
        var json = """[{"name":"John","age":"30"}]""";
        var request = new ParseRequest { Type = ContentType.INTERNAL_JSON, Content = ToBase64(json) };

        var result = _sut.Parse(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.RowCount.Should().Be(1);
        result.Value.Data[0]["name"].Should().Be("John");
    }

    [Fact]
    public void Parse_InvalidCSV_ReturnsFailure()
    {
        var csv = "Name,Age"; // header only, no data
        var request = new ParseRequest { Type = ContentType.CSV, Content = ToBase64(csv) };

        var result = _sut.Parse(request);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Parse_InvalidJSON_ReturnsFailure()
    {
        var json = "{not valid json";
        var request = new ParseRequest { Type = ContentType.INTERNAL_JSON, Content = ToBase64(json) };

        var result = _sut.Parse(request);

        result.IsSuccess.Should().BeFalse();
    }
}
