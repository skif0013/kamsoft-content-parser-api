using FluentAssertions;
using Kamsoft.ContentParser.Api.Services.Parsers;

namespace Kamsoft.ContentParser.Tests.Parsers;

public class InternalJsonContentParserTests
{
    private readonly InternalJsonContentParser _sut = new();

    [Fact]
    public void SupportedType_Returns_INTERNAL_JSON()
    {
        _sut.SupportedType.Should().Be("INTERNAL_JSON");
    }

    [Fact]
    public void Parse_EmptyContent_ThrowsJsonException()
    {
        var act = () => _sut.Parse(string.Empty);
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Parse_InvalidJson_ThrowsJsonException()
    {
        var act = () => _sut.Parse("{not valid");
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void Parse_EmptyArray_ThrowsFormatException()
    {
        var act = () => _sut.Parse("[]");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_NullJson_ThrowsFormatException()
    {
        var act = () => _sut.Parse("null");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_SingleObject_ReturnsCorrectData()
    {
        var json = """[{"name":"John","age":"30"}]""";
        var result = _sut.Parse(json);

        result.Should().HaveCount(1);
        result[0]["name"].Should().Be("John");
        result[0]["age"].Should().Be("30");
    }

    [Fact]
    public void Parse_MultipleObjects_ReturnsAllRows()
    {
        var json = """[{"name":"John"},{"name":"Jane"},{"name":"Bob"}]""";
        var result = _sut.Parse(json);

        result.Should().HaveCount(3);
    }

    [Fact]
    public void Parse_NumericValues_ConvertedToString()
    {
        var json = """[{"count":42,"ratio":3.14}]""";
        var result = _sut.Parse(json);

        result[0]["count"].Should().Be("42");
        result[0]["ratio"].Should().Be("3.14");
    }

    [Fact]
    public void Parse_BooleanValues_ConvertedToString()
    {
        var json = """[{"active":true}]""";
        var result = _sut.Parse(json);

        result[0]["active"].Should().Be("True");
    }
}
