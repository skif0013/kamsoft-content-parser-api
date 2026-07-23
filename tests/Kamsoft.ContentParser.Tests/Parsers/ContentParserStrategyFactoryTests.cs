using FluentAssertions;
using Kamsoft.ContentParser.Api.Domain.Enums;
using Kamsoft.ContentParser.Api.Services;
using Kamsoft.ContentParser.Api.Services.Parsers;

namespace Kamsoft.ContentParser.Tests.Parsers;

public class ContentParserStrategyFactoryTests
{
    private readonly ContentParserStrategyFactory _sut;

    public ContentParserStrategyFactoryTests()
    {
        _sut = new ContentParserStrategyFactory(new Api.Domain.Interfaces.IContentParser[]
        {
            new CsvContentParser(),
            new InternalJsonContentParser()
        });
    }

    [Fact]
    public void GetParser_CSV_ReturnsCsvParser()
    {
        var result = _sut.GetParser(ContentType.CSV);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CsvContentParser>();
    }

    [Fact]
    public void GetParser_INTERNAL_JSON_ReturnsJsonParser()
    {
        var result = _sut.GetParser(ContentType.INTERNAL_JSON);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<InternalJsonContentParser>();
    }

    [Fact]
    public void GetParser_UnsupportedType_ReturnsFailure()
    {
        var act = () => Enum.Parse<ContentType>("XML");
        act.Should().Throw<ArgumentException>();
    }
}
