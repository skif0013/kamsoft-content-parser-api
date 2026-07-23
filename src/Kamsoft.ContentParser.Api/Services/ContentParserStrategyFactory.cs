using Kamsoft.ContentParser.Api.Domain;
using Kamsoft.ContentParser.Api.Domain.Enums;
using Kamsoft.ContentParser.Api.Domain.Interfaces;

namespace Kamsoft.ContentParser.Api.Services;

public class ContentParserStrategyFactory : IContentParserStrategyFactory
{
    private readonly Dictionary<ContentType, IContentParser> _parsers;

    public ContentParserStrategyFactory(IEnumerable<IContentParser> parsers)
    {
        _parsers = parsers.ToDictionary(p => Enum.Parse<ContentType>(p.SupportedType), p => p);
    }

    public Result<IContentParser> GetParser(ContentType contentType)
    {
        if (_parsers.TryGetValue(contentType, out var parser))
            return Result<IContentParser>.Success(parser);

        var supported = string.Join(", ", _parsers.Keys);
        return Result<IContentParser>.Failure($"Unsupported type: {contentType}. Supported types: {supported}");
    }
}
