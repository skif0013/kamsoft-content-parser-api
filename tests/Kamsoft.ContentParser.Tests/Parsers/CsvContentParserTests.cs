using FluentAssertions;
using Kamsoft.ContentParser.Api.Services.Parsers;

namespace Kamsoft.ContentParser.Tests.Parsers;

public class CsvContentParserTests
{
    private readonly CsvContentParser _sut = new();

    [Fact]
    public void SupportedType_Returns_CSV()
    {
        _sut.SupportedType.Should().Be("CSV");
    }

    [Fact]
    public void Parse_EmptyContent_ThrowsFormatException()
    {
        var act = () => _sut.Parse(string.Empty);
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_HeaderOnly_ThrowsFormatException()
    {
        var act = () => _sut.Parse("Name,Age");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_SingleRow_ReturnsCorrectData()
    {
        var csv = "Name,Age\nJohn,30";
        var result = _sut.Parse(csv);

        result.Should().HaveCount(1);
        result[0]["Name"].Should().Be("John");
        result[0]["Age"].Should().Be("30");
    }

    [Fact]
    public void Parse_MultipleRows_ReturnsAllRows()
    {
        var csv = "Name,Age\nJohn,30\nJane,25\nBob,40";
        var result = _sut.Parse(csv);

        result.Should().HaveCount(3);
    }

    [Fact]
    public void Parse_QuotedFieldsWithCommas_ParsesCorrectly()
    {
        var csv = "Name,Address\nJohn,\"123 Main St, Apt 4\"";
        var result = _sut.Parse(csv);

        result[0]["Address"].Should().Be("123 Main St, Apt 4");
    }

    [Fact]
    public void Parse_EscapedQuotes_ParsesCorrectly()
    {
        var csv = "Name,Bio\nJohn,\"He said \"\"hello\"\"\"";
        var result = _sut.Parse(csv);

        result[0]["Bio"].Should().Be("He said \"hello\"");
    }

    [Fact]
    public void Parse_MissingValues_FillsEmptyStrings()
    {
        var csv = "Name,Age\nJohn,";
        var result = _sut.Parse(csv);

        result[0]["Age"].Should().Be(string.Empty);
    }
}
