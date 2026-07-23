using Kamsoft.ContentParser.Api.Domain.Enums;

namespace Kamsoft.ContentParser.Api.Domain.Interfaces;

public interface IContentParserStrategyFactory
{
    Result<IContentParser> GetParser(ContentType contentType);
}
